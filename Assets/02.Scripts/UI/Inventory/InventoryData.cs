using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// inventory slot collection
/// </summary>
public class InventoryData : INotifyCollectionChanged<InventorySlotData>
{
    private static InventoryData _instance;
    public static InventoryData Instance
    {
        get
        {
            if (_instance == null)
                _instance = new InventoryData();
            return _instance;
        }
    }
    public InventoryViewModel ViewModel;

    private List<InventorySlotData> _slotDataList;
    public List<InventorySlotData> SlotDataList
    {
        get
        {
            return _slotDataList;
        }
        set
        {
            _slotDataList = value;
            CollectionChanged?.Invoke();
        }
    }

    public event Action<InventorySlotData> ItemAdded;
    public event Action<InventorySlotData> ItemRemoved;
    public event Action CollectionChanged;

    private InventorySlotData _tmpSlotData1, _tmpSlotData2;

    public InventoryData()
    {
        ViewModel = new InventoryViewModel(this);
        _slotDataList = new List<InventorySlotData>();

        for (int i = 0; i < InventorySettings.TOTAL_SLOTS; i++)
            Add();

        // todo -> Load slot data from DB
        Debug.Log("[InventoryData] : Created");
    }

    /// <summary>
    /// Add new slot
    /// </summary>
    public void Add()
    {
        _tmpSlotData1 = new InventorySlotData(_slotDataList.Count, ItemPair.Empty);
        SlotDataList.Add(_tmpSlotData1);
        ItemAdded?.Invoke(_tmpSlotData1);
    }

    /// <summary>
    /// Remove slot
    /// </summary>
    public void RemoveAt(int id)
    {
        _tmpSlotData1 = SlotDataList[id];
        SlotDataList.Remove(_tmpSlotData1);
        ItemRemoved?.Invoke(_tmpSlotData1);
    }

    /// <summary>
    /// Add item to slot
    /// </summary>
    /// <param name="itemPair">item data to add</param>
    public void AddItem(ItemPair itemPair)
    {
        _tmpSlotData1 = SlotDataList.Find(data => data.ItemPair.Code == itemPair.Code);

        if (_tmpSlotData1 != null)
        {
            if (_tmpSlotData1.ItemPair + itemPair != ItemPair.Error)
                _tmpSlotData1.ItemPair += itemPair;
            else
                Debug.LogError("[InventoryData] : Failed to add item");
        }
        else if (TryGetEmptySlotData(out _tmpSlotData1))
        {
            _tmpSlotData1.ItemPair = itemPair;
        }
        else
        {
            // inventory is full
        }
    }

    /// <summary>
    /// Remove item (slot of id has priority)
    /// when item number of slot is lack, finds other slots has same itemCode and tries remove.
    /// </summary>
    /// <param name="id">removing prioriy slot id</param>
    /// <param name="itemPair">target item amout to remove</param>
    public bool RemoveItem(int id, ItemPair itemPair)
    {
        InventorySlotData tmpSlotData = SlotDataList.Find(data => data.Id == id);
        ItemPair tmpResult = tmpSlotData.ItemPair - itemPair;
        if (tmpSlotData != null &&
            tmpResult != ItemPair.Error)
        {
            if (tmpResult.Num > 0)
            {
                tmpSlotData.ItemPair = tmpResult;
                return true;
            }
            else if (tmpResult.Num == 0)
            {
                tmpSlotData.ItemPair = ItemPair.Empty;
                return true;
            }
            else
            {
                bool OK = RemoveItem(new ItemPair(tmpResult.Code, -tmpResult.Num));

                if (OK)
                    tmpSlotData.ItemPair = tmpResult;

                return OK;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// when item number of slot is lack, finds other slots has same itemCode and tries remove.
    /// </summary>
    /// <param name="itemPair">target item amout to remove</param>
    public bool RemoveItem(ItemPair itemPair)
    {
        InventorySlotData tmpSlotData = SlotDataList.Find(data => data.ItemPair.Code == itemPair.Code);
        ItemPair tmpResult = tmpSlotData.ItemPair - itemPair;
        if (tmpSlotData != null &&
            tmpResult != ItemPair.Error)
        {
            if (tmpResult.Num > 0)
            {
                tmpSlotData.ItemPair = tmpResult;
                return true;
            }
            else if (tmpResult.Num == 0)
            {
                tmpSlotData.ItemPair = ItemPair.Empty;
                return true;
            }
            else
            {
                bool OK = RemoveItem(new ItemPair(tmpResult.Code, -tmpResult.Num));

                if (OK)
                    tmpSlotData.ItemPair = tmpResult;

                return OK;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// when other slot exist, does swap data.
    /// nor, just move previous data.
    /// </summary>
    /// <param name="prevID"></param>
    /// <param name="afterID"></param>
    public void SwapSlot(int prevID, int afterID)
    {
        _tmpSlotData1 = SlotDataList.Find(data => data.Id == prevID);
        _tmpSlotData2 = SlotDataList.Find(data => data.Id == afterID);

        if (_tmpSlotData2 == null)
        {
            _tmpSlotData1.Id = afterID;
        }
        else
        {
            ItemPair prev = _tmpSlotData1.ItemPair;
            ItemPair after = _tmpSlotData2.ItemPair;
            _tmpSlotData1.ItemPair = after;
            _tmpSlotData2.ItemPair = prev;
        }
    }

    private bool TryGetEmptySlotData(out InventorySlotData emptySlotData)
    {
        emptySlotData = SlotDataList.Find(slotData => slotData.ItemPair == ItemPair.Empty);
        return emptySlotData != null;
    }
}
