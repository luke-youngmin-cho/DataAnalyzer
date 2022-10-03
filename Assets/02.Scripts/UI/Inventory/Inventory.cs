using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// Source binded with InventoryViewModel's source collection
/// </summary>
public class Inventory : MonoBehaviour
{
    public InventoryViewModel ViewModel;
    [SerializeField] private InventorySlotHandler _slotHandler;

    private PropertyBinder<InventoryViewModel> _binder;
    [BindPropertyTo("Source", SourceTag.InventoryData)]
    public IEnumerable<ItemPair> DependedSource { get; private set; }

    public List<InventorySlot> Slots = new List<InventorySlot>();
    [SerializeField] private InventorySlot _slotPrefab;

    private InventorySlot _tmpSlot;
    private void Awake()
    {
        StartCoroutine(E_Init());
    }

    IEnumerator E_Init()
    {
        yield return new WaitUntil(() => InventoryData.Instance != null &&
                                         InventoryData.Instance.ViewModel != null);

        ViewModel = InventoryData.Instance.ViewModel;

        _binder = new PropertyBinder<InventoryViewModel>(ViewModel, ViewModel.Source, SourceTag.InventoryData);

        for (int i = 0; i < InventorySettings.TOTAL_SLOTS; i++)
        {
            _tmpSlot = Instantiate(_slotPrefab, transform);
            _tmpSlot.Id = i;
            _tmpSlot.SlotHandler = _slotHandler;
            Slots.Add(_tmpSlot);
        }

        ViewModel.Source.CollectionChanged += SourceChanged;
        Debug.Log("[Inventory] : Initialized");
    }

    private void SourceChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        foreach (InventorySlotData slotData in args.NewItems)
        {
            Slots[slotData.Id].ItemCode = slotData.ItemPair.Code;
            Slots[slotData.Id].Num = slotData.ItemPair.Num;
        }
    }
}
