using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Data for single inventory slot
/// </summary>
public class InventorySlotData : INotifyPropertyChanged
{
    public int Id;

    private ItemPair _itemPair;

    public ItemPair ItemPair
    {
        get
        {
            return _itemPair;
        }
        set
        {
            if (_itemPair != value)
            {
                _itemPair = value;
                NotifyPropertyChangedEvent(this);
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChangedEvent(object sender, [CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
    }

    public InventorySlotData(int id, ItemPair pair)
    {
        Id = id;
        ItemPair = pair;
        Debug.Log($"[InventorySlotData] : Created (id : {id})");
    }
}
