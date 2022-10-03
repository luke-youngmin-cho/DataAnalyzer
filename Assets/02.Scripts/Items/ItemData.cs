using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes
{
    Equipment,
    Spend,
    ETC
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Create ItemData")]
public class ItemData : ScriptableObject
{
    public ItemTypes ItemType;
    public string Name;
    public string Description;
    public Sprite Icon;
    public int Code
    {
        get
        {
            return HashCode.Combine<ItemTypes, string>(ItemType, Name);
        }
    }
}
