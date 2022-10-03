using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Helps user can handle items in slot
/// </summary>
public class InventorySlotHandler : MonoBehaviour, IPointerDownHandler
{
    public bool IsBusy => _slot != null;

    [SerializeField] private Inventory _inventory;
    [SerializeField] private DropItemPanel _dropItemPanel;
    [SerializeField] private Image _icon;
    private InventorySlot _slot;

    // UI Raycast event
    [SerializeField] private GraphicRaycaster _raycaster;
    [SerializeField] private PointerEventData _pointerEventData;
    [SerializeField] private EventSystem _eventSystem;


    //==============================================================================
    //****************************** Public Methods ********************************
    //==============================================================================

    public void Handle(InventorySlot slot)
    {
        _slot = slot;
        _icon.sprite = slot.Icon;
        gameObject.SetActive(true);
    }

    public void Cancel()
    {
        _slot = null;
        _icon.sprite = null;
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // left down
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            _raycaster.Raycast(_pointerEventData, results);

            bool onOtherSlot = false;
            bool onCanvas = false;
            InventorySlot tmpSlot = null;

            foreach (var result in results)
            {
                // down on other slot
                if (result.gameObject.TryGetComponent(out tmpSlot) &&
                    tmpSlot != _slot)
                {
                    onOtherSlot = true;
                    break;
                }

                //Check All UI. (if not exist, drop item to field)
                if (result.gameObject.TryGetComponent<CanvasRenderer>(out CanvasRenderer tmpCanvasRenderer))
                {
                    if (tmpCanvasRenderer.gameObject != this.gameObject)
                        onCanvas = true;
                }
            }

            // swap slot when mouse down on other slot
            if (onOtherSlot)
            {
                if (_inventory.ViewModel.Swap.TryExecute(_slot, tmpSlot))
                    Cancel();
            }
            // drop item when mouse down on battle field
            else if (onCanvas == false)
            {
                tmpSlot = _slot;
                _dropItemPanel.SetOKButtonListener(() =>
                {
                    if (_inventory.ViewModel.Drop.TryExecute(tmpSlot, _dropItemPanel.GetInputNum()))
                    {
                        _dropItemPanel.gameObject.SetActive(false);
                    }
                });
                _dropItemPanel.gameObject.SetActive(true);

                Cancel();
            }
            else
            {
                Cancel();
            }
        }
        // right down
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Cancel();
        }
    }


    //==============================================================================
    //****************************** Private Methods *******************************
    //==============================================================================

    private void Awake()
    {
        _raycaster = transform.parent.GetComponent<GraphicRaycaster>();
        _eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Update()
    {
        Vector3 pos = Input.mousePosition;
        transform.position = pos;
    }
}
