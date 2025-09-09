using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlackCat_UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class UI_InventoryController : MonoBehaviour
{
    private static UI_InventoryController instance;
    public static UI_InventoryController Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    private InventoryData _inventoryData;
    private UI_Inventory _uiInventory;
    private VirtualInventory  _virtualInventory;
    private int _curSelectIndex = 0;
    private int _slotNumber;
    private Transform _PlayerTransform;
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _uiInventory = UIManager.Instance.GetInventoryUI();
        if (_uiInventory == null)
        {
            Debug.Log("UI_inventoryNotFound");
        }
        //_slotNumber = _uiInventory.CalculateMaxShowCellNum();
        
        InitData(0);
        _slotNumber =  _uiInventory.InitScroll(_inventoryData.SlotNumber);
        
        InitRuntimeInventory(_slotNumber, _inventoryData);
  
        _uiInventory.clickItemAction += OnClickScrollItemAction;
        _uiInventory.drapItemAction += DropInventoryItem;
        _uiInventory.MoveItemAction += MoveToTargetSlot;
        _uiInventory.CheckDragDownAction += OnCheckDragDown;
        _uiInventory.OnInventoryOpened += BindVirtualViewSlot;
        _uiInventory.OnInventoryClosed += UnBindVirtualViewSlot;
        _uiInventory.RebindViewToModelAction += RebindViewToModel;
        _uiInventory.LoadItemDefinitionAction += LoadItemDefinition;
    }


    private void InitData(int slotNumber)
    {
        _inventoryData = new InventoryData();
        string slotDataPath = Application.persistentDataPath + InventoryDataPath;
        _inventoryData.Initialize(slotDataPath, slotNumber);
    }

    private void InitRuntimeInventory(int slotNumber, InventoryData inventoryData)
    {
        _virtualInventory = new VirtualInventory(inventoryData, slotNumber);
    }

    public void BindVirtualViewSlot(List<UI_InventorySlot> ui_slot, int startDataIdx = 0)  // View初始化完成后绑定model回调
    {
        for (int i = 0; i < ui_slot.Count; i++)
        {
            VirtualInventorySlot  virtualInventorySlot = _virtualInventory.Slots[i];
            UI_InventorySlot uiSlot = ui_slot[i];
            virtualInventorySlot.StateChanged += uiSlot.UpdateViewDisplay;
        }
    }

    public void UnBindVirtualViewSlot(List<UI_InventorySlot> ui_slot)
    {
        for (int i = 0; i < ui_slot.Count; i++)
        {
            VirtualInventorySlot  virtualInventorySlot = _virtualInventory.Slots[i];
            UI_InventorySlot uiSlot = ui_slot[i];
            virtualInventorySlot.StateChanged -= uiSlot.UpdateViewDisplay;
        }
    }

    // ScrollView滚动，为其重新绑定数据，并会触发UI更新
    private void RebindViewToModel(int startDataIdx = 0)  
    {
        _virtualInventory.BindItemData(_inventoryData, startDataIdx);
    }
    
    //private async void OnItemUpdate(UI_InventorySlot slot, int dataIdx)  // 在controller获取数据，交给view更新
    //{
    //    var slotData = GetItemData(dataIdx);  // 拿到动态数据
    //    var oldSlotData = GetItemData(slot.DataListIndex);
    //    if (!slotData.hasItem)  // 没有物体则不进行显示
    //    {
    //        slot.ClearCellDisplay();
    //        return;
    //    }
     //   await LoadItemDefinition(slotData, slot);
     //   
     //   slot.DataListIndex = dataIdx;
    //    slot.UpdateViewDisplay(slotData, slotData.ItemStack.ItemDefinition);  // 根据动态静态数据更新显示
    //   slot.UpdateCellSelect(dataIdx == _curSelectIndex);
    //}
    
    // 异步加载静态资源，并传入完成时回调函数
    private async void LoadItemDefinition(ItemStack itemStack, UnityAction<ItemStack, ItemDefinitionSO> callback)
    {
        var definition = await GetOrLoadItemDefinitionAsync(itemStack);
        callback?.Invoke(itemStack, definition);
    }

    private bool OnCheckDragDown(UI_InventorySlot targetSlot ,UI_InventorySlot originData)
    {
        var originItem = GetItemData(originData.DataListIndex).ItemStack;
        var targetSlotData = GetItemData(targetSlot.DataListIndex);
        return targetSlotData.CanAcceptItem(originItem);
    }

    private void MoveToTargetSlot(UI_InventorySlot targetSlot ,UI_InventorySlot originSlot)
    {
        var originItem = GetItemData(originSlot.DataListIndex);
        var targetSlotData = GetItemData(targetSlot.DataListIndex);
        var originItemStack = originItem.ItemStack;
        originItem.Clear();
        targetSlotData.AddItem(originItemStack);
    }
    
    private void OnClickScrollItemAction(int slotIndex)
    {
        if (slotIndex == _curSelectIndex)
        {
            return;
        }
            
        _curSelectIndex = slotIndex;

        //UpdateSelectItemInfo();

        _uiInventory.UpdateScrollView(true);
    }

    private void DropInventoryItem(UI_InventorySlot slot)
    {
        int dataIdx = slot.DataListIndex;
        ItemStack item = _inventoryData.InventorySlotData[dataIdx].ItemStack;
        _inventoryData.InventorySlotData[dataIdx].Clear();
        //OnItemUpdate(slot, dataIdx);
        DropGameItem(item);
    }
    
    private void DropGameItem(ItemStack item)
    {
        Debug.Log($"丢弃 {item.ItemDefinition.name}");
        UnityEventCenter.Instance.TriggerEvent("DropGameItem", item);
    }

    private InventorySlotData GetItemData(int index)
    {
        return _inventoryData.InventorySlotData[index];
    }
    
    private async UniTask<ItemDefinitionSO> GetOrLoadItemDefinitionAsync(ItemStack itemStack)
    {
        if (itemStack.ItemDefinition != null)
            return itemStack.ItemDefinition;

        var definition = await AdvancedItemManager.Instance.LoadItemDefinitionAsync(itemStack.Item.ID);
        if (definition != null)
        {
            itemStack.ItemDefinition = definition;
        }
        return definition;
    }

    public bool CanAcceptItem(ItemStack item)
    {
        return _inventoryData.CanAcceptItem(item);
    }
    

    public void AddItem(ItemStack item)
    {
        _inventoryData.AddItem(item);
    }
    
    private void OnDestroy()
    {
        
        _uiInventory.clickItemAction -= OnClickScrollItemAction;
        _uiInventory.drapItemAction -= DropInventoryItem;
        _uiInventory.MoveItemAction -= MoveToTargetSlot;
        _uiInventory.CheckDragDownAction -= OnCheckDragDown;
        _uiInventory.OnInventoryOpened -= BindVirtualViewSlot;
        _uiInventory.OnInventoryClosed -= UnBindVirtualViewSlot;
        _uiInventory.RebindViewToModelAction -= RebindViewToModel;
        _uiInventory.LoadItemDefinitionAction -= LoadItemDefinition;
    }
    
    private const string InventoryDataPath = "/Inventory/inventory_data.json";
}
