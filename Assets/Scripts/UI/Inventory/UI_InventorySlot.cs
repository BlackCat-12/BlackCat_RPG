using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Drag;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text _numberOfItems;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _selectIcon;
    [SerializeField] private Button _selectButton;
    
    [SerializeField] private DrapHandler  _drapHandler;

    private RectTransform  _rectTransform;
    private int _dataListIndex;
    private RectTransform _scrollRect;
    private UI_Inventory _uiInventory;
    
    private UnityAction<int> btnClickAction;
    public UnityAction<UI_InventorySlot> DropItemAction; 
    public event Func<UI_InventorySlot, UI_InventorySlot, bool> OnCheckDragDown;
    public UnityAction<UI_InventorySlot, UI_InventorySlot> MoveToTargetSlotAction; 
    public UnityAction<ItemStack, UnityAction<ItemStack,ItemDefinitionSO>> LoadItemDefinitionAction;
    public UnityAction<ItemStack,ItemDefinitionSO> OnLoadCompleted;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(() =>
        {
            btnClickAction?.Invoke(_dataListIndex);
        });

        _drapHandler.DropItemAction += DropItem;
        
        // 添加更新订阅
        OnLoadCompleted += LoadItemDefinitionComplete;
    }

    public void DropItem()
    {
        Debug.Log("DropItem");
        DropItemAction?.Invoke(this);
    }

    private void OnDisable()
    {
        btnClickAction = null;
        _drapHandler.DropItemAction -= DropItem;
        
        // 取消更新订阅
        OnLoadCompleted -= LoadItemDefinitionComplete;
    }

    public void Init(RectTransform parent, UI_Inventory  uiInventory)
    {
        _scrollRect = parent;
        _uiInventory = uiInventory;
    }

    public int DataListIndex
    {
        get => _dataListIndex;
        set => _dataListIndex = value;
    }
    public RectTransform GetScrollRect()
    {
        return _scrollRect;
    }

    public void AddButtonClickListener(UnityAction<int> callback)
    {
        btnClickAction = callback;
    }

    public void AddDropItemAction(UnityAction<UI_InventorySlot> callback)
    {
        DropItemAction = callback;
    }

    public void AddMoveToTargetSlotAction(UnityAction<UI_InventorySlot, UI_InventorySlot> callback)
    {
        MoveToTargetSlotAction = callback;
    }

    public void AddDragDragDownCheck(Func<UI_InventorySlot, UI_InventorySlot, bool>  callback)
    {
        OnCheckDragDown = callback;
    }

    public void AddLoadItemDefinitionAction(UnityAction<ItemStack, UnityAction<ItemStack,ItemDefinitionSO>> callback)
    {
        LoadItemDefinitionAction = callback;
    }

    public bool CheckDragDown(UI_InventorySlot slot)
    {
        return OnCheckDragDown.Invoke(this, slot);
    }
    
    public void UpdateCellPos(Vector2 pos)  // 更新slot显示位置
    {
        _rectTransform.anchoredPosition = pos;
    }
    
    public void UpdateViewDisplay(object sender, InventorySlotData itemData)
    {
        if (itemData == null || itemData.ItemStack == null)
        {
            ClearCellDisplay();
            return;
        }
        ItemStack itemStack = itemData.ItemStack;

        if (itemStack.ItemDefinition == null)  // 异步加载静态数据
        {
            LoadItemDefinitionAction?.Invoke(itemStack, OnLoadCompleted);
            return;
        }

        UpdateDisplayWithItemDefinition(itemStack);
    }

    private void UpdateDisplayWithItemDefinition(ItemStack item)
    {
        var itemDefinition = item.ItemDefinition;
    
        _icon.enabled = true;
        _icon.sprite = itemDefinition.icon;

        bool isStackable = itemDefinition.stackable;
        _numberOfItems.enabled = isStackable;
        _numberOfItems.text = isStackable ? item.NumberOfItems.ToString() : string.Empty;
    }

    public void LoadItemDefinitionComplete(ItemStack itemStack, ItemDefinitionSO itemDefinitionSO)
    {
        if (itemDefinitionSO == null)
        {
            Debug.LogError($"ItemDefinition not found for ID: {itemDefinitionSO?.id}");
            ClearCellDisplay();
            return;
        }
        UpdateDisplayWithItemDefinition(itemStack);
    }

    private void ClearCellDisplay()
    {
        _icon.enabled = false;
        _icon.sprite = null;
        _numberOfItems.enabled = false;
        _numberOfItems.text = string.Empty;
    }
    
    public void UpdateCellSelect(bool select)
    {
        _selectIcon.enabled = select;
    }
    
    private async Task<ItemDefinitionSO> GetItemDefinitionAsync(int itemId)
    {
        return await AdvancedItemManager.Instance.LoadItemDefinitionAsync(itemId);
    }

    public void MoveToTargetSlot(UI_InventorySlot originData)
    {
        MoveToTargetSlotAction?.Invoke(this, originData);
    }
}
