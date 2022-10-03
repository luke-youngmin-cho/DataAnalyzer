using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// Gets notification from InventoryData
/// Gets commands from Inventory / InventorySlotHandler
/// Includes Commands to drop, swap, use item
/// </summary>
public class InventoryViewModel
{
    private InventoryData _inventoryData;

    #region Source
    public class InventorySource : ObservableCollection<InventorySlotData>
    {
        private InventoryData _inventoryData;
        public InventorySource(InventoryData inventoryData)
        {
            _inventoryData = inventoryData;

            _inventoryData.ItemAdded += (InventorySlotData slotData) =>
            {
                Add(slotData);
                slotData.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
                {
                    InventorySlotData tmp = (sender as InventorySlotData);
                    SetItem(tmp.Id, tmp);
                };
            };

            _inventoryData.ItemRemoved += (InventorySlotData slotData) => 
            { 
                Remove(slotData); 
            };
        }                
    }
    public InventorySource Source;
    #endregion

    #region Drop command
    public class DropCommand
    {
        private InventoryData _inventoryData;
        public event Action<bool> CanExecuteChanged;
        private bool _canExecute;
        private InventorySlotData _tmpData;

        public DropCommand(InventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        public bool CanExecute(InventorySlot slot, int numToDrop)
        {
            bool can = _canExecute;

            _tmpData = _inventoryData.SlotDataList.Find(slotData => slotData.Id == slot.Id &&
                                                                    slotData.ItemPair.Code == slot.ItemCode &&
                                                                    slotData.ItemPair.Num >= numToDrop);

            if (_tmpData != null)
            {
                can = true;
            }
            else
            {
                can = false;
            }

            if (can != _canExecute)
                CanExecuteChanged?.Invoke(can);

            _canExecute = can;
            return _canExecute;
        }

        public void Execute(InventorySlot slot, int numToDrop)
        {
            int itemCode = slot.ItemCode;
            if (_inventoryData.RemoveItem(slot.Id, new ItemPair(slot.ItemCode, numToDrop)))
            {
                Item itemDropped = GameObject.Instantiate(ItemAssets.Instance.GetItemByCode(itemCode),
                                                          GameObject.Find("Character").transform.position + Vector3.up * 0.5f,
                                                          Quaternion.identity);
                itemDropped.Num = numToDrop;
            }
                
        }

        public bool TryExecute(InventorySlot slot, int numToDrop)
        {
            if (CanExecute(slot, numToDrop))
            {
                Execute(slot, numToDrop);
                return true;
            }

            Debug.Log($"Drop item failed : not enough item");
            return false;
        }
    }
    public DropCommand Drop;
    #endregion

    #region Use command
    public class UseCommand
    {

        private InventoryData _inventoryData;
        public event Action<bool> CanExecuteChanged;
        private bool _canExecute;
        private InventorySlotData _tmpData;

        public UseCommand(InventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        public bool CanExecute(InventorySlot slot)
        {
            bool can = _canExecute;
            _tmpData = _inventoryData.SlotDataList.Find(data => data.Id == slot.Id);

            if (_tmpData != null &&
                _tmpData.Id == slot.Id &&
                _tmpData.ItemPair != ItemPair.Empty &&
                _tmpData.ItemPair != ItemPair.Error)
            {
                can = true;
            }
            else
            {
                can = false;
            }

            if (can != _canExecute)
                CanExecuteChanged(can);

            _canExecute = can;
            return _canExecute;
        }

        public void Execute(InventorySlot slot)
        {
            _tmpData = _inventoryData.SlotDataList.Find(data => data.Id == slot.Id);

            _inventoryData.RemoveItem(new ItemPair(slot.ItemCode, 1));
            ItemAssets.Instance.GetItemByCode(slot.ItemCode).Use();
        }

        public bool TryExecute(InventorySlot slot)
        {
            bool can = _canExecute;
            _tmpData = _inventoryData.SlotDataList.Find(data => data.Id == slot.Id);

            if (_tmpData != null &&
                _tmpData.Id == slot.Id &&
                _tmpData.ItemPair != ItemPair.Empty &&
                _tmpData.ItemPair != ItemPair.Error)
            {
                can = true;

                _inventoryData.RemoveItem(new ItemPair(slot.ItemCode, 1));
                ItemAssets.Instance.GetItemByCode(slot.ItemCode).Use();
            }
            else
            {
                can = false;
            }

            if (can != _canExecute)
                CanExecuteChanged(can);

            _canExecute = can;
            return _canExecute;
        }
    }
    public UseCommand Use;
    #endregion

    #region Swap command
    public class SwapCommand
    {
        private InventoryData _inventoryData;
        public event Action<bool> CanExecuteChanged;
        private bool _canExecute;

        public SwapCommand(InventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        public bool CanExecute(InventorySlot slotHandled, InventorySlot slotTarget)
        {
            bool can = _canExecute;

            if (slotHandled != null &&
                slotTarget != null)
            {
                can = true;
            }
            else
            {
                can = false;
            }

            if (can != _canExecute)
                CanExecuteChanged?.Invoke(can);

            _canExecute = can;
            return _canExecute;
        }

        public void Execute(InventorySlot slotHandled, InventorySlot slotTarget)
        {
            _inventoryData.SwapSlot(slotHandled.Id, slotTarget.Id);
        }

        public bool TryExecute(InventorySlot slotHandled, InventorySlot slotTarget)
        {
            if (CanExecute(slotHandled, slotTarget))
            {
                Execute(slotHandled, slotTarget);
                return true;
            }
            return false;
        }
    }
    public SwapCommand Swap;
    #endregion

    public InventoryViewModel(InventoryData inventoryData)
    {
        _inventoryData = inventoryData;
        _inventoryData.ViewModel = this;
        Source = new InventorySource(inventoryData);
        Drop = new DropCommand(inventoryData);
        Use = new UseCommand(inventoryData);
        Swap = new SwapCommand(inventoryData);
        Debug.Log("[InventoryViewModel] : Created");
    }
}
