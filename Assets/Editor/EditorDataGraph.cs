using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Help to draw graph on custom editor. 
/// 
/// </summary>
public class EditorDataGraph
{
    public string Title { get; private set; }
	private float _timeMark;
	public float ElapsedTime => IsPaused ? PausedTime : Time.time - _timeMark;
	public float PausedTime { get; private set; }
	public bool IsPaused { get; private set; }
	public bool Pause
    {
        set
        {
			if (value)
				PausedTime = ElapsedTime;
			IsPaused = value;
        }
    }

    public float GridLinesX = 1;
    public float GridLinesY = 1;
    public int GraphResolution = GRAPH_RESOLUTION_DEFAULT;
    const int GRAPH_RESOLUTION_DEFAULT = 12;
    const int GRAPH_RESOLUTION_MIN = 1;
    const int GRAPH_RESOLUTION_MAX = 120;
	public struct GraphColors
	{
		public Color Background;
		public Color Outline;
		public Color GridLine;
		public Color Function;
		public Color CustomLine;
	}
	public GraphColors Colors;

	public float MinX => ElapsedTime < GraphResolution ? 0 : ElapsedTime - GraphResolution;
	public float MaxX => ElapsedTime;
	public float MinY = 0.0f;
	public float MaxY = 0.0f;
	
	private Rect _rect; // Chart bounds
	private float _rangeX => Mathf.Abs(MaxX - MinX);
	private float _rangeY => Mathf.Abs(MaxY - MinY);
	
	// Vertex buffers
	private Vector3[] _rectVertices = new Vector3[4];
	private Vector3[] _lineVertices = new Vector3[2];
    
	public enum LineType
    {
		Normal,
		Dotted
    }
	private struct LineColorPair
	{
		public float Position;
		public Color Color;
	}
	private List<LineColorPair> _linesX = new List<LineColorPair>();
	private List<LineColorPair> _linesY = new List<LineColorPair>();
	private List<LineColorPair> _linesXDotted = new List<LineColorPair>();
	private List<LineColorPair> _linesYDotted = new List<LineColorPair>();

	private struct LabelInfo
    {
		public float X, Y;
		public Color Color;
		public Vector2 Size;
		public string Label;
    }

	private List<LabelInfo> _labels = new List<LabelInfo>();

	private List<float> _timeHistory = new List<float>();
	private class Data
    {
		public string tag;
		public List<DataPair> pairs;
		public Color Color;
    }

	private struct DataPair
    {
		public float Time;
		public float Value;
    }
	private List<Data> _dataList = new List<Data>();
	public delegate void MouseEvent(float x, float y);
	private List<MouseEvent> _clickEvents = new List<MouseEvent>();
	public Vector2 ClickPoint;

	public EditorDataGraph(string _title)
	{
		_timeMark = Time.time;
		Title = _title;

		// Default graph colors
		Colors = new GraphColors
		{
			Background = new Color(0.15f, 0.15f, 0.15f, 1f),
			Outline = new Color(0.15f, 0.15f, 0.15f, 1f),
			GridLine = new Color(0.5f, 0.5f, 0.5f),
			Function = Color.red,
			CustomLine = Color.white
		};
	}

	//==================================================================================================
	//************************************** Public Methods ********************************************
	//==================================================================================================

	public void Draw()
    {
		Draw(256, 256);
    }

	/// <summary>
	/// Call this on OnGUI()
	/// </summary>
	public void Draw(float width, float height)
    {
		using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
			GUILayout.Label(Title);

		using (new GUILayout.HorizontalScope())
		{
			GUILayout.Space(EditorGUI.indentLevel * 15f);
			_rect = GUILayoutUtility.GetRect(width, height);
		}

		OnMouseDrag();
		OnMouseDown();
		OnRepaint();
		DrawResolutionSlider();
    }

	public void AddClickEvent(MouseEvent e)
	{
		if (!_clickEvents.Contains(e))
			_clickEvents.Add(e);
	}

	public void RemoveClickEvent(MouseEvent e)
	{
		if (_clickEvents.Contains(e))
			_clickEvents.Remove(e);
	}

	public void RemoveAllClickEvents()
	{
		_clickEvents.Clear();
	}

    #region Line
    /// <summary>
    /// Add a vertical line with the default color.
    /// </summary>
    /// <param name="value">Position of the line in graph coord.</param>
    public void AddLineX(float value, LineType lineType)
	{
		AddLineX(value, Colors.CustomLine, lineType);
	}

	/// <summary>
	/// Add a vertical line.
	/// </summary>
	/// <param name="value">Position of the line in graph coord.</param>
	/// <param name="color">Color of the line.</param>
	public void AddLineX(float value, Color color, LineType lineType)
	{
		foreach (var pair in _linesX)
		{
			if (pair.Position == value)
				return;
		}

        switch (lineType)
        {
            case LineType.Normal:
				_linesX.Add(new LineColorPair { Position = value, Color = color });
				break;
            case LineType.Dotted:
				_linesXDotted.Add(new LineColorPair { Position = value, Color = color });
				break;
            default:
                break;
        }
        
	}

	/// <summary>
	/// Add a horizontal line with the default color.
	/// </summary>
	/// <param name="value">Position of the line in chart coord.</param>
	public void AddLineY(float value, LineType lineType)
	{
		AddLineY(value, Colors.CustomLine, lineType);
	}

	/// <summary>
	/// Add a horizontal line.
	/// </summary>
	/// <param name="value">Position of the line in chart coord.</param>
	/// <param name="color">Color of the line.</param>
	public void AddLineY(float value, Color color, LineType lineType)
	{
		foreach (var pair in _linesY)
		{
			if (pair.Position == value)
				return;
		}

        switch (lineType)
        {
            case LineType.Normal:
				_linesY.Add(new LineColorPair { Position = value, Color = color });
				break;
            case LineType.Dotted:
				_linesYDotted.Add(new LineColorPair { Position = value, Color = color });
				break;
            default:
                break;
        }
	}

	public void ClearAllLines()
	{
		_linesX.Clear();
		_linesY.Clear();
		_linesXDotted.Clear();
		_linesYDotted.Clear();
	}
    #endregion

    #region Label

	public void AddLabel(float x, float y, string text)
    {
		_labels.Add(new LabelInfo()
		{
			X = x,
			Y = y,
			Color = Color.white,
			Size = new Vector2(50.0f,30.0f),
			Label = text
		});
	}

	public void AddLabel(float x, float y, Color color, Vector2 size, string text)
    {
		_labels.Add(new LabelInfo()
		{
			X = x,
			Y = y,
			Color = color,
			Size = size,
			Label = text
		});
	}

	public void ClearAllLabels()
    {
		_labels.Clear();
    }
    #endregion

    #region Data
    public void CreateData(string tag, Color color)
    {
		if (string.IsNullOrEmpty(tag))
			return;

		_dataList.Add(new Data
		{
			tag = tag,
			pairs = new List<DataPair>(),
			Color = color
		});
    }

	public void RemoveData(string tag)
	{
        if (string.IsNullOrEmpty(tag))
            return;

		Data data = _dataList.Find(data => data.tag == tag);
		if (data != null)
			_dataList.Remove(data);
    }

    public void ChangeColor(string tag, Color color)
    {
		Data data = _dataList.Find(x => x.tag == tag);
		if (data != null)
			data.Color = color;
    }

	public void AddDataValue(string tag, float value)
    {
        if (string.IsNullOrEmpty(tag))
            return;

        Pause = IsPaused; // Refresh Paused time when it was paused

		_dataList.Find(data => data.tag == tag)
			.pairs
			.Add(new DataPair { Time = ElapsedTime, Value = value });

		RefreshMinMaxY(value);
		RecordTime(ElapsedTime);
    }

	public bool AddDataValueWhenDifferent(string tag, float value)
    {
		if (value != GetLastDataValue(tag))
        {
			AddDataValue(tag, value);
			return true;
        }
		return false;
    }

	public bool IsDataExist(string tag)
	{
		return _dataList.Find(x => x.tag == tag) != null;
	}

	public void ClearAllData()
    {
        foreach (var data in _dataList)
        {	
			_timeMark = Time.time;
			PausedTime = 0.0f;
			data.pairs.Clear();
		}
		_timeHistory.Clear();
    }

	public float GetLastDataValue(string tag)
    {
        try
        {
			return _dataList.Find(data => data.tag == tag)
					.pairs
					.Last()
					.Value;
		}
        catch
        {
			return 0.0f;
        }
    }

	public float GetYValue(string tag, float x)
    {
		return _dataList.Find(data => data.tag == tag)
				.pairs
				.FindLast(dataPair => dataPair.Time < x).Value;
    }

	public float[] GetAllYValues(float x)
	{
		float[] result = new float[_dataList.Count];
		Rect tmpRect;
		float tmpY;
		for (int i = 0; i < _dataList.Count; i++)
		{
			tmpY = _dataList[i].pairs.FindLast(pair => pair.Time < x).Value;
			tmpRect = new Rect(CoordTransformToGraph(x, tmpY) - Vector3.up * 20.0f, new Vector2(50.0f, 30.0f));
			result[i] = tmpY;
		}
		return result;
	}
    #endregion

    #region Shift-Frame
	public void ShiftFrameLeft()
    {
		ClickPoint.x = _timeHistory.FindLast(t => t < ClickPoint.x);
		foreach (var e in _clickEvents)
			e(ClickPoint.x, ClickPoint.y);
	}
	public void ShiftFrameRight()
	{
		float x = _timeHistory.Find(t => t > ClickPoint.x);

		if (x <= 0)
			x = CoordTransformToChartForX(MaxX);

        ClickPoint.x = x;
		foreach (var e in _clickEvents)
			e(ClickPoint.x, ClickPoint.y);
	}
	#endregion

	//==================================================================================================
	//************************************** Private Methods *******************************************
	//==================================================================================================

	private void DrawAllData()
    {
        foreach (Data data in _dataList)
        {
			DataPair previous = new DataPair();			
			float x1 = 0.0f, x2 = 0.0f, y1 = 0.0f, y2 = 0.0f;

            foreach (DataPair pair in data.pairs)
            {
				if (pair.Time >= MinX &&
					pair.Time <= MaxX)
                {
					DataPair current = pair;
					x1 = previous.Time;
					x2 = current.Time;
					y1 = previous.Value;
					y2 = current.Value;
					DrawLine(x1, y1, x2, y1, data.Color, 2.0f);
					DrawLine(x2, y1, x2, y2, data.Color, 2.0f);
					previous = current;
				}
			}

			DrawLine(previous.Time, previous.Value, MaxX, previous.Value, data.Color, 2.0f); // finish line
        }
    }

	private void DrawAllLabels()
    {
		GUIStyle tmpStyle;
        foreach (var labelInfo in _labels)
        {
			Rect tmpRect = new Rect(CoordTransformToGraph(labelInfo.X, labelInfo.Y) - Vector3.up * 20.0f, labelInfo.Size);
			tmpStyle = new GUIStyle(EditorStyles.label);
			tmpStyle.normal.textColor = labelInfo.Color;
			GUI.Label(tmpRect, labelInfo.Label, tmpStyle);
		}
	}

	private void OnMouseDrag()
	{
		// Handle MouseDown events
		if (Event.current.type == EventType.MouseDrag)
		{
			if (_rect.Contains(Event.current.mousePosition))
			{
				Vector2 mousePos = (Event.current.mousePosition - _rect.position);
				Vector2 unitPos = new Vector2(
					mousePos.x / _rect.width * _rangeX + MinX,
					(1f - (mousePos.y / _rect.height)) * _rangeY + MinY
				);

				foreach (var e in _clickEvents)
					e(unitPos.x, unitPos.y);

				ClickPoint = new Vector2(unitPos.x, unitPos.y);
			}
		}
	}

	private void OnMouseDown()
    {
		// Handle MouseDown events
		if (Event.current.type == EventType.MouseDown)
		{
			if (_rect.Contains(Event.current.mousePosition))
			{
				Vector2 mousePos = (Event.current.mousePosition - _rect.position);
				Vector2 unitPos = new Vector2(
					mousePos.x / _rect.width * _rangeX + MinX,
					(1f - (mousePos.y / _rect.height)) * _rangeY + MinY
				);

				foreach (var e in _clickEvents)
					e(unitPos.x, unitPos.y);

				ClickPoint = new Vector2(unitPos.x, unitPos.y);
			}
		}
	}

	private void OnRepaint()
    {
		if (Event.current.type == EventType.Repaint)
        {
			//DrawRect(minX, minY, maxX, maxY, Colors.Background, Colors.Outline);
			
			DrawRect(MinX, MinY, MaxX, MaxY, Colors.Background, Colors.Outline);
			DrawGridLines();
			DrawLines();
			DrawAllData();
			DrawAllLabels();
		}
	}

	private void DrawCrossLine(float x, float y)
	{
		DrawLine(x, -_rect.y / 2.0f, x, _rect.y / 2.0f, Color.white, 0.1f);
		DrawLine(-_rect.x / 2.0f, y, _rect.x / 2.0f, y, Color.white, 0.1f);

	}

	private void DrawGridLines()
	{
		Rect tmpRect;

		// XAxis helper lines
		if (GridLinesX > 0)
		{
			float multiplier = 1;
			while ((_rangeX / (GridLinesX * multiplier)) > (_rect.width / 2f))
				multiplier *= 2;
			
			for (int x = (int)MinX; x <= (int)MaxX; x += (int)(GridLinesX * multiplier))
            {
				DrawLine(x, MinY, x, MaxY, Colors.GridLine, 1);

				// XAxis helper values
				tmpRect = new Rect(CoordTransformToGraph(x, MinY) - Vector3.up * 20.0f, new Vector2(50.0f, 30.0f));
				GUI.Label(tmpRect, x.ToString());
			}
		}

		// YAxis helper lines
		if (GridLinesY > 0)
		{
			float multiplier = 1;
			while ((_rangeY / (GridLinesY * multiplier)) > (_rect.height / 2f))
				multiplier *= 2;

			
			for (int y = (int)MinY; y <= (int)MaxY; y += (int)(GridLinesY * multiplier))
            {
				DrawLine(MinX, y, MaxX, y, Colors.GridLine, 1);
			}	
		}

		// YAxis helper values
		Rect minYRect = new Rect(CoordTransformToGraph(MinX, MinY) - Vector3.up * 30.0f, new Vector2(50.0f, 30.0f));
		GUI.Label(minYRect, MinY.ToString());
		Rect maxYRect = new Rect(CoordTransformToGraph(MinX, MaxY), new Vector2(50.0f, 30.0f));
		GUI.Label(maxYRect, MaxY.ToString());
	}
	private void DrawLines()
	{
		// Vertical lines
		foreach (var line in _linesX)
		{
			DrawLine(line.Position, MinY, line.Position, MaxY, line.Color, 2);
		}
		// Horizontal lines
		foreach (var line in _linesY)
		{
			DrawLine(MinX, line.Position, MaxX, line.Position, line.Color, 2);
		}
		// Vertical lines
		foreach (var line in _linesXDotted)
		{
			DrawDottedLine(line.Position, MinY, line.Position, MaxY, line.Color, 2);
		}
		// Horizontal lines
		foreach (var line in _linesYDotted)
		{
			DrawDottedLine(MinX, line.Position, MaxX, line.Position, line.Color, 2);
		}
	}

	private void DrawResolutionSlider()
	{
		EditorGUILayout.Space();
		GraphResolution = EditorGUILayout.IntSlider(GraphResolution, GRAPH_RESOLUTION_MIN, GRAPH_RESOLUTION_MAX);
	}

	
	/// <summary>
	/// Transform time-value to x-y
	/// </summary>
	/// <param name="x"> time</param>
	/// <param name="y"> value </param>
	/// <returns></returns>
	Vector3 CoordTransformToGraph(float x, float y)
	{
		x = Mathf.Lerp(_rect.x, _rect.xMax, (x - MinX) / _rangeX);
		y = Mathf.Lerp(_rect.yMax, _rect.y, (y - MinY) / _rangeY);

		return new Vector3(x, y, 0);
	}

	float CoordTransformToChartForX(float x)
	{
		return Mathf.Lerp(_rect.x, _rect.xMax, (x - MinX) / _rangeX);
	}

	float CoordTranformToChartForY(float y)
	{
		return Mathf.Lerp(_rect.yMax, _rect.y, (y - MinY) / _rangeY);
	}

	void DrawLine(float x1, float y1, float x2, float y2, Color color, float width)
	{
		_lineVertices[0] = CoordTransformToGraph(x1, y1);
		_lineVertices[1] = CoordTransformToGraph(x2, y2);
		Handles.color = color;
		Handles.DrawAAPolyLine(width, _lineVertices);
	}

	void DrawDottedLine(float x1, float y1, float x2, float y2, Color color, float width)
    {
		_lineVertices[0] = CoordTransformToGraph(x1, y1);
		_lineVertices[1] = CoordTransformToGraph(x2, y2);
		Handles.color = color;
		Handles.DrawDottedLine(new Vector3(_lineVertices[0].x, _lineVertices[0].y, 0),
							   new Vector3(_lineVertices[1].x, _lineVertices[1].y, 0),
							   width);
	}

	void DrawRect(float x1, float y1, float x2, float y2, Color fill, Color line)
	{
		_rectVertices[0] = CoordTransformToGraph(x1, y1);
		_rectVertices[1] = CoordTransformToGraph(x2, y1);
		_rectVertices[2] = CoordTransformToGraph(x2, y2);
		_rectVertices[3] = CoordTransformToGraph(x1, y2);

		Handles.DrawSolidRectangleWithOutline(_rectVertices, fill, line);
	}

	private void RefreshMinMaxY(float value)
    {
		MinY = MinY < value ? MinY : value;
		MaxY = MaxY > value ? MaxY : value;
		MinY = MinY < MaxY ? MinY : MaxY - 1;
	}

	private void RecordTime(float time)
    {
		if (_timeHistory.Contains(time) == false)
			_timeHistory.Add(time);
	}
}
