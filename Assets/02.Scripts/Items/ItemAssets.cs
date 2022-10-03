using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accesses to all item & itemData asset
/// todo -> change to use addressable asset
/// </summary>
public class ItemAssets : MonoBehaviour
{
    private static ItemAssets _instance;
    public static ItemAssets Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load<ItemAssets>("ItemAssets"));
                _instance.Init();
            }
            return _instance;
        }
    }


    [SerializeField] private List<Item> _itemList;
    [SerializeField] private List<ItemData> _itemDataList;

    private Dictionary<int, Item> _items = new Dictionary<int, Item>();
    private Dictionary<int, ItemData> _itemDataDictionary = new Dictionary<int, ItemData>();

    private void Init()
    {
        foreach (var item in _itemList)
        {
            _items.Add(item.Data.Code, item);
        }

        foreach (var item in _itemDataList)
        {
            _itemDataDictionary.Add(item.Code, item);
        }
    }

    public bool TryGetItemByCode(int code, out Item item)
        => _items.TryGetValue(code, out item);

    public Item GetItemByCode(int code)
        => _items[code];

    public bool TryGetItemDataByCode(int code, out ItemData itemData)
        => _itemDataDictionary.TryGetValue(code, out itemData);

    public bool GetItemDataByCode(int code)
        => _itemDataDictionary[code];
}