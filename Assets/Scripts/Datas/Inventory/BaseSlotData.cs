using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public abstract class BaseSlotData 
{

    private List<int> _allowedItemIds; // 允许的物品ID列表
    private ItemStack _itemStack;
    
    public event EventHandler<ItemStack> StateChanged;

    public BaseSlotData(ItemStack itemStack)
    {
        ItemStack = itemStack;
    }

    public BaseSlotData()
    {
        
    }
    
    [JsonProperty]
    public List<int> AllowedItemIds => _allowedItemIds;

    [JsonProperty]
    public ItemStack ItemStack
    {
        get => _itemStack;
        set
        {
            _itemStack = value;
            NotifyStateChanged();
        }
    }
    
    // ******************************************  非序列化数据 ******************************************
    [JsonIgnore]
    public int NumberOfItems
    {
        get => _itemStack.NumberOfItems;
        set
        {
            _itemStack.NumberOfItems = value;
            Debug.Log($"数量更新为 {_itemStack.NumberOfItems}");
            NotifyStateChanged();
        }
    }
    
    [JsonIgnore]
    public bool hasItem => ItemStack != null;
    
    [JsonIgnore]
    public bool isStackable => ItemStack != null && ItemStack.IsStackable;
       
    public virtual bool CanAcceptItem(ItemStack item)
    {
        if ((AllowedItemIds != null && AllowedItemIds.Count != 0) && !AllowedItemIds.Contains(item.ID)) // 默认全包含
        {
            return false;
        }
        if (!hasItem)  // 当前格子为空，可以放置
        {
            return true;
        }
        if (hasItem && isStackable && item.ID == ItemStack.ID) // 当前格子非空，但是当前物品可堆叠，且物品相同，可以放置
        {
            return true;
        }
        return false;
    }
    
    public void AddItem(ItemStack originItemStack)
    {
        if (!hasItem) // 为空直接放置
        {
            ItemStack =  originItemStack;
            return;
        }
        NumberOfItems += originItemStack.NumberOfItems;
    }
    
    public void Clear()  // 清空slot中的物品
    {
        ItemStack = null;
    }
    
    private void NotifyStateChanged()
    {
        StateChanged?.Invoke(this, ItemStack);
    }
}
