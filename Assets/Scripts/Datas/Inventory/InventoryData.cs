using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

public class InventoryData
{
    private List<InventorySlotData> _inventorySlotData;
    private int _size = 999; 
    
    public event UnityAction OnDataChanged; 
    
    public int SlotNumber => _inventorySlotData.Count;

    [JsonProperty]
    public List<InventorySlotData> InventorySlotData
    {
        get => _inventorySlotData;
        set => _inventorySlotData = value;
    }

    [JsonProperty]
    public int Size
    {
        get => _size;
        set => _size = value;
    }
    public void Initialize(string jsonPath, int slotNumber)
    {
        InventorySlotData = new List<InventorySlotData>();
        LoadFromJson(jsonPath);
        //CreateEmptySlot(slotNumber);
    }

    private void CreateEmptySlot(int slotNumber)
    {
        if (_inventorySlotData.Count < slotNumber)
        {
            // 计算需要添加的数量
            int slotsToAdd = slotNumber - _inventorySlotData.Count;
        
            for (int i = 0; i < slotsToAdd; i++)
            {
                // 创建新的空Slot并添加到列表
                _inventorySlotData.Add(new InventorySlotData(null));
            }
        }
    }
    // 从JSON加载数据
    public void LoadFromJson(string jsonPath)
    {
        string jsonData = ResourceService.Instance.LoadJsonData(jsonPath);
        Debug.Log(jsonPath);
        if (jsonData == null)
        {
            Debug.LogError("无法加载背包数据");
            return;
        }
        InventorySlotData = JsonConvert.DeserializeObject<InventoryData>(jsonData).InventorySlotData;
        OnDataChanged?.Invoke();
    }


    public bool CanAcceptItem(ItemStack itemStack)
    {
        InventorySlotData slot = FindSlot(itemStack);
        if (slot == null && _inventorySlotData.Count == _size)  // 如果找不到放置槽位并且当前背包槽位已达上限，则返回false
        {
            return false;
        }
        return true;
    }

    public InventorySlotData FindSlot(ItemStack itemStack)
    {
        InventorySlotData slot;
        if (itemStack.IsStackable) // 查找可堆叠物品的栏位，且优先堆叠
        {
            slot = _inventorySlotData.FirstOrDefault(slot => slot.hasItem && slot.ItemStack.ID == itemStack.ID);
            if (slot == null)
            {
                slot = _inventorySlotData.FirstOrDefault(slot => !slot.hasItem && slot.CanAcceptItem(itemStack));
            }
        }
        else  // 非可堆叠物品直接找到一个空位
        {
            slot = _inventorySlotData.FirstOrDefault(slot => !slot.hasItem && slot.CanAcceptItem(itemStack));
        }  
        return slot;
    }
    
    public void AddItem(ItemStack itemStack)
    {
        InventorySlotData relevantSlot = FindSlot(itemStack);
        if (relevantSlot != null)  
        {
            Debug.Log("找到可供放置的slot");
            if (relevantSlot.hasItem)  // 可堆叠物品
            {
                relevantSlot.NumberOfItems += itemStack.NumberOfItems;
            }
            else
            {
                relevantSlot.ItemStack = itemStack;  // 不可堆叠物品，或可堆叠物品首次放置
            }
        }
        else
        {        
            Debug.Log($"新建slot，并加入item{itemStack.ID}");
            InventorySlotData newSlotData = new InventorySlotData(itemStack);  // 无槽位则再创建一个槽位
            _inventorySlotData.Add(newSlotData);
        }
    }
    
    // 查找Item
    public bool HasItem(ItemStack itemStack, bool CheckNumberOfItem = false)
    {
        var itemSlot = FindSlot(itemStack);
        if(itemSlot == null) return false;
        if (!CheckNumberOfItem) return true; // 存在物品，当前不需要检查数量，则直接返回
        if (itemStack.IsStackable)  // 可堆叠，且需要检查数量，则数量大于目标值返回true
        {
            return itemSlot.NumberOfItems > itemStack.NumberOfItems;
        }
        return _inventorySlotData.Count(slot => slot.ItemStack.ID == itemStack.Item.ID) >= itemStack.NumberOfItems;  // 不可堆叠则返回计数
    }
}
