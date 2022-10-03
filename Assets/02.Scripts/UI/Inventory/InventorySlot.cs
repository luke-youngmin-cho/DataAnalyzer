using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// single inventory slot ui
/// </summary>
public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public int Id;
    private int _itemCode;
    public int ItemCode
    {
        get
        {
            return _itemCode;
        }
        set
        {
            _itemCode = value;

            if (ItemAssets.Instance.TryGetItemDataByCode(value, out ItemData data))
                _image.sprite = data.Icon;
            else
                _image.sprite = null;
        }
    }
    private int _num;
    public int Num
    {
        get
        {
            return _num;
        }
        set
        {
            _num = value;

            if (_num <= 1)
                _text.text = "";
            else
                _text.text = _num.ToString();
        }
    }
    public Sprite Icon => _image.sprite;
    private Image _image;
    private TMP_Text _text;
    public InventorySlotHandler SlotHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left &&
            _num > 0 &&
            SlotHandler.IsBusy == false)
        {
            SlotHandler.Handle(this);
        }
    }

    private void Awake()
    {
        _image = transform.GetChild(0).GetComponent<Image>();
        _text = transform.GetChild(1).GetComponent<TMP_Text>();
    }    
}
