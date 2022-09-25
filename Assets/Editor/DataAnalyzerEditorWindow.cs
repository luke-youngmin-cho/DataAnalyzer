using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Object = UnityEngine.Object;
using Unity.VisualScripting;
using System.Text;

/// <summary>
/// Editor window drawing EditorDataGraph to visualize all fields & properties of target object.
/// All members are private.
/// </summary>
public class DataAnalyzerEditorWindow : EditorWindow
{
    List<MemberInfo> _targetMembers;
    List<MemberInfo> _targetMembersPool;
    Object _target;
    Object _targetMem;
    EditorDataGraph _dataGraph;

    // layout options
    GUILayoutOption[] size100x50 = new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(50) };
    GUILayoutOption[] size100x20 = new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(20) };

    // GUI event
    KeyCode input;
    float pressTimeMark;
    float pressTime = 0.5f;
    bool _doRefresh;

    // Buffers        
    const int BUFFER_SIZE = 30;
    int _usingBufferCount;
    MemberInfo[] _membersBuffer;
    Color[] _colorsBuffer;    
    GUIContent[] _dropDownLabelBuffer;

    // Dropdown
    GUIContent _dropDownLabelDefault = new GUIContent("Select member...");

    [MenuItem("Window/Analysis/DataAnalyzer")]
    static void Init()
    {
        DataAnalyzerEditorWindow window = GetWindow<DataAnalyzerEditorWindow>(typeof(DataAnalyzerEditorWindow));
     
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(2000, 1000);
        window.Show();
    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        DrawTitle();
        DrawTargetObjectField();

        if (SetUpTargetMembers() == false)
            return;

        RefreshDrawingMembers();
        GUIEvents();

        EditorGUILayout.BeginHorizontal();
        DrawPausedButton();
        DrawClearButton();
        EditorGUILayout.EndHorizontal();

        DrawGraph();        

        GUIUtility.ExitGUI();
    }
    
    void DrawTitle()
    {
        GUILayout.Label("DataAnalyzer", EditorStyles.boldLabel);
    }

    void DrawTargetObjectField()
    {
        _target = EditorGUILayout.ObjectField(_target, typeof(Object), true);
    }

    /// <summary>
    /// Refresh drawing members when targeted member fields or properties were changed.
    /// </summary>
    void RefreshDrawingMembers()
    {
        if (_doRefresh == false)
            return;

        float tmpValue;
        for (int i = 0; i < _usingBufferCount; i++)
        {
            if (_membersBuffer[i] != null)
            {
                if (_dataGraph.IsDataExist(_membersBuffer[i].Name) == false)
                {
                    tmpValue = ConvertToFloat(_membersBuffer[i]);
                    Debug.Log($"[DataAnalyzerWindow] : Get Field ({_membersBuffer[i].Name}) == {tmpValue}");
                    _colorsBuffer[i] = Color.green;
                    _dataGraph.CreateData(_membersBuffer[i].Name, _colorsBuffer[i]);
                    Debug.Log($"[DataAnalyzerWindow] : target field refreshed ({_membersBuffer[i].Name})");
                }
            }
        }

        _dataGraph.RemoveAllClickEvents();
        _dataGraph.AddClickEvent((x, y) =>
        {
            // draw line on clicked pos
            _dataGraph.ClearAllLines();
            _dataGraph.AddLineX(x, EditorDataGraph.LineType.Dotted);

            // draw labels for all values
            _dataGraph.ClearAllLabels();
            for (int i = 0; i < _usingBufferCount; i++)
            {
                int tmpBufferIdx = i;
                float yValue = _dataGraph.GetYValue(_membersBuffer[tmpBufferIdx].Name, x);
                _dataGraph.AddLabel(x: x,
                                    y: yValue,
                                    color: Color.cyan,
                                    size: new Vector2(200.0f, 30.0f),
                                    text: $"{_membersBuffer[tmpBufferIdx].Name} : {ConvertToString(yValue, _membersBuffer[tmpBufferIdx])}");
            }
            _dataGraph.AddLabel(x: x,
                                y: _dataGraph.MinY - 30.0f,
                                color: Color.magenta,
                                size: new Vector2(200.0f, 30.0f),
                                text: x.ToString());

            _dataGraph.Pause = true;
        });

        _doRefresh = false;
    }

    /// <summary>
    /// Handling GUI Events
    /// </summary>
    void GUIEvents()
    {
        // Event handling
        Event e = Event.current;

        if (e.isKey &&
            e.type == EventType.KeyDown &&
            e.keyCode != KeyCode.None)
        {
            // judge pressing
            if (Time.time - pressTimeMark > pressTime)
            {
                input = e.keyCode;
            }
            else
            {
                input = KeyEventEnum(e.keyCode).Current;
            }
        }
        else
        {
            input = KeyCode.None;
        }

        switch (input)
        {
            case KeyCode.LeftArrow:
                OnLeftDown();
                break;
            case KeyCode.RightArrow:
                OnRightDown();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Enumerator to handle the key input only once.
    /// </summary>
    /// <param name="current"> give Event.current.KeyCode </param>
    /// <returns></returns>
    IEnumerator<KeyCode> KeyEventEnum(KeyCode current)
    {
        if (input == current)
        {
            yield return input;
            input = KeyCode.None;
            yield return input;
        }
        else
        {
            yield return KeyCode.None;
        }            
    }


    void DrawPausedButton()
    {
        if (_dataGraph.IsPaused)
            _dataGraph.Pause = !GUILayout.Button("Continue", size100x50);
        else
            _dataGraph.Pause = GUILayout.Button("Pause", size100x50);
    }

    void DrawClearButton()
    {
        if (GUILayout.Button("Clear", size100x50))
        {
            _dataGraph.ClearAllData();
        }
    }

    void DrawGraph()
    {
        // Chart
        //-----------------------------------------------------------------------        
        _dataGraph.Colors.Background = Color.black;

        // Chart data 
        //-----------------------------------------------------------------------
        for (int i = 0; i < _usingBufferCount; i++)
        {
            _dataGraph.AddDataValueWhenDifferent(_membersBuffer[i].Name, ConvertToFloat(_membersBuffer[i]));
        }
        _dataGraph.Draw();

        // Members 
        //-------------------------------------------------------------------------
        Rect r;
        Color tmpColor = Color.clear;

        // draw all members selected
        for (int i = 0; i < _usingBufferCount; i++)
        {
            r = EditorGUILayout.BeginHorizontal();
            int tmpBufferIdx = i;
            if (EditorGUILayout.DropdownButton(_dropDownLabelBuffer[i], FocusType.Passive, size100x20))
            {
                GenericMenu menu = new GenericMenu();

                // default content item
                // when dropdown item clicked, 
                // Return the item to pool,
                // add selected item at "drawing memeber buffer" ,
                // shifts all remains , 
                // adjust buffer counts.
                menu.AddItem(content: _dropDownLabelDefault,
                             on     : false,
                             func   : (object parameter) =>
                                            {
                                                // Remove data
                                                _dataGraph.RemoveData(_membersBuffer[tmpBufferIdx].Name);

                                                // Return to pool
                                                _targetMembersPool.Add(_membersBuffer[tmpBufferIdx]);
                                                _usingBufferCount--;

                                                // shifts all remains
                                                for (int j = tmpBufferIdx; j < _usingBufferCount; j++)
                                                {                                                    
                                                    _dropDownLabelBuffer[j] = _dropDownLabelBuffer[j + 1];
                                                    _membersBuffer[j] = _membersBuffer[j + 1];
                                                    _colorsBuffer[j] = _colorsBuffer[j + 1];                                                    
                                                }
                                                
                                                _doRefresh = true;
                                            },
                             userData: null);

                // not selected target member content items
                for (int j = 0; j < _targetMembersPool.Count; j++)
                {
                    int tmpPoolIdx = j;
                    menu.AddItem(content:  new GUIContent(_targetMembersPool[tmpPoolIdx].Name),
                                 on     :  false,
                                 func   : (object parameter) =>
                                                {
                                                    // Get selected member from pool
                                                    MemberInfo member = (MemberInfo)parameter;
                                                    _targetMembersPool.Remove(member);

                                                    // Add selected member's data
                                                    _dataGraph.CreateData(member.Name, Color.green);

                                                    // Remove original member's data
                                                    _dataGraph.RemoveData(_membersBuffer[tmpBufferIdx].Name);

                                                    // Return original member to pool
                                                    _targetMembersPool.Add(_membersBuffer[tmpBufferIdx]);
                                                    _dropDownLabelBuffer[tmpBufferIdx] = new GUIContent(member.Name);

                                                    _membersBuffer[tmpBufferIdx] = member;

                                                    _doRefresh = true;
                                                },
                                 userData: _targetMembersPool[tmpPoolIdx]);
                }
                menu.DropDown(r);
            }

            // color
            tmpColor = EditorGUILayout.ColorField(_colorsBuffer[i], GUILayout.Width(60.0f));
            if (_colorsBuffer[i] != tmpColor)
            {
                _colorsBuffer[i] = tmpColor;
                _dataGraph.ChangeColor(_membersBuffer[i].Name, _colorsBuffer[i]);
            }

            // value
            string valueString;
            try
            {
                valueString = ConvertToString(_dataGraph.GetYValue(_membersBuffer[i].Name, _dataGraph.ClickPoint.x),
                                              _membersBuffer[i]);
            }
            catch
            {
                valueString = "NaN";
            }

            EditorGUILayout.TextField(valueString, GUILayout.Width(100.0f));

            EditorGUILayout.EndHorizontal();

        }

        // draw default dropdown to add member
        r = EditorGUILayout.BeginHorizontal();
        if (EditorGUILayout.DropdownButton(_dropDownLabelDefault, FocusType.Passive, size100x20))
        {
            GenericMenu menu = new GenericMenu();

            // default content item
            menu.AddItem(content : _dropDownLabelDefault,
                         on      : false,
                         func    : (object parameter) =>
                                        {
                                        },
                         userData: null);

            // not selected target member content items
            for (int i = 0; i < _targetMembersPool.Count; i++)
            {
                int tmpPoolIdx = i;
                menu.AddItem(content : new GUIContent(_targetMembersPool[tmpPoolIdx].Name),
                             on      : false,
                             func    : (object parameter) =>
                                            {
                                                // Get selected member from pool
                                                MemberInfo member = (MemberInfo)parameter;
                                                _targetMembersPool.Remove(member);

                                                // Add selected member's data
                                                _dataGraph.CreateData(member.Name, Color.green); 

                                                // Put to buffers
                                                _dropDownLabelBuffer[_usingBufferCount] = new GUIContent(member.Name);
                                                _membersBuffer[_usingBufferCount] = member;
                                                _colorsBuffer[_usingBufferCount] = Color.green;
                                                _usingBufferCount++;

                                                _doRefresh = true;
                                            },
                             userData: _targetMembersPool[tmpPoolIdx]);
            }
            menu.DropDown(r);
        }
        EditorGUILayout.EndHorizontal();
        
    }

    bool SetUpTargetMembers()
    {
        if (_target == null)
        {
            if (_targetMembers != null)
                _targetMembers = null;

            return false;
        }

        if (_target != _targetMem ||
            _targetMembers == null)
        {
            // Create data graph
            _dataGraph = new EditorDataGraph("Chart");

            // Get target' members and set buffers
            _targetMembers = new List<MemberInfo>();
            _targetMembers = MemeberFilterOnlyFieldsAndProperties(_target.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public));
            _targetMembersPool = new List<MemberInfo>(_targetMembers);
            _membersBuffer = new MemberInfo[BUFFER_SIZE];
            _colorsBuffer = new Color[BUFFER_SIZE];
            _dropDownLabelBuffer = new GUIContent[BUFFER_SIZE];
            _usingBufferCount = 0;

            _targetMem = _target;
            _doRefresh = true;
        }
        
        return true;
    }

    /// <summary>
    /// Convert memberInfo's value to float.
    /// </summary>
    /// <param name="memberInfo">target member</param>
    float ConvertToFloat(MemberInfo memberInfo)
    {
        Type type = null;
        object value = null;
        float converted = -1.0f;
        try
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    value = ((FieldInfo)memberInfo).GetValue(_target);
                    break;
                case MemberTypes.Property:
                    value = ((PropertyInfo)memberInfo).GetValue(_target);
                    break;
                default:
                    {
                        throw new ArgumentException($"[DataAnalayzer] : Converting {memberInfo.MemberType} is not implemented");
                    }
            }

            type = value.GetType();
            converted = (float)value;
        }
        catch
        {
            try
            {
                if (type == typeof(bool))
                    converted = (bool)value ? 1.0f : 0.0f;
                else if (type.IsEnum)
                    converted = (int)value;
                else
                {
                    Debug.LogWarning($"[DataAnalyzerWindow] : Converting type of {type.Name} is not implemented yet.");
                }   
            }
            catch
            {
                //Debug.LogError($"[DataAnalyzerWindow] : Wrong casting type of {type.Name");
            }
        }
        return converted;
    }

    /// <summary>
    /// Convert memberInfo's value to float.
    /// </summary>
    /// <param name="memberInfo">target member</param>
    /// <param name="converted">converted target member's value</param>
    /// <returns>true : success ,false : failed</returns>
    bool TryConvertToFloat(MemberInfo memberInfo, out float converted)
    {
        bool isOk = true;
        Type type = null;
        object value = null;
        converted = -1.0f;
        try
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    value = ((FieldInfo)memberInfo).GetValue(_target);
                    break;
                case MemberTypes.Property:
                    value = ((PropertyInfo)memberInfo).GetValue(_target);
                    break;
                default:
                    {
                        throw new ArgumentException($"[DataAnalayzer] : Converting {memberInfo.MemberType} is not implemented");
                    }
            }

            type = value.GetType();
            converted = (float)value;
        }
        catch
        {
            try
            {
                if (type == typeof(bool))
                    converted = (bool)value ? 1.0f : 0.0f;
                else if (type.IsEnum)
                    converted = (int)value;
                else
                {
                    isOk = false;
                    Debug.LogWarning($"[DataAnalyzerWindow] : Converting type of {type.Name} is not implemented yet.");
                }
            }
            catch
            {
                isOk = false;
                //Debug.LogError($"[DataAnalyzerWindow] : Wrong casting type of {type.Name");
            }
        }
        return isOk;
    }

    /// <summary>
    /// float to value of specific memeberInfo type
    /// </summary>
    /// <param name="value">target value</param>
    /// <param name="memberInfo">MemberInfo to get it's value type</param>
    /// <returns>converted string</returns>
    /// <exception cref="ArgumentException">the memberinfo type is not supported</exception>
    string ConvertToString(float value, MemberInfo memberInfo)
    {
        Type conversionType = null;
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                conversionType = ((FieldInfo)memberInfo).GetValue(_target).GetType();
                break;
            case MemberTypes.Property:
                conversionType = ((PropertyInfo)memberInfo).GetValue(_target).GetType();
                break;
            default:
                {
                    throw new ArgumentException($"[DataAnalayzer] : Converting {memberInfo.MemberType} is not implemented");
                }
        }

        //Debug.Log($"Converting... {value}, {conversionType.Name}, isEnum? {conversionType.IsEnum}");
        if (conversionType.IsEnum &&
            Enum.TryParse(conversionType, value.ToString(), out object obj))
        {
            return obj.ToString();
        }
        else
        {
            return Convert.ChangeType(value, conversionType).ToString();
        }
    }

    /// <summary>
    /// Filtering memberInfos only Fields and Properties
    /// </summary>
    /// <param name="members">target</param>
    /// <returns>Filtered list</returns>
    List<MemberInfo> MemeberFilterOnlyFieldsAndProperties(MemberInfo[] members)
    {
        List<MemberInfo> list = new List<MemberInfo>();
        float tmp;
        for (int i = 0; i < members.Length; i++)
        {
            if (members[i].MemberType == MemberTypes.Field ||
                members[i].MemberType == MemberTypes.Property)
            {
                if (TryConvertToFloat(members[i], out tmp))
                    list.Add(members[i]);
            }
                
        }
        return list;
    }

    /// <summary>
    /// for left-arrow key
    /// move cursor to previous frame
    /// </summary>
    void OnLeftDown()
    {
        _dataGraph.ShiftFrameLeft();
    }

    /// <summary>
    /// for right-arrow key
    /// move cursor to next frame
    /// </summary>
    void OnRightDown()
    {
        _dataGraph.ShiftFrameRight();
    }
}
