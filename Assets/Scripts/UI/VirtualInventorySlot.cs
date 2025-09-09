using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualInventorySlot 
{
    private InventorySlotData _inventorySlotData;
    public int Index;
    
    public event EventHandler<InventorySlotData> StateChanged;

    public VirtualInventorySlot(InventorySlotData inventorySlotData, int idx)  
    {
        _inventorySlotData = inventorySlotData;  // 这里通知了view更新
        Index = idx;

        _inventorySlotData.StateChanged += DataStateChanged;
    }
    
    ~VirtualInventorySlot()  // 先调用析构函数再释放内存
    {
        _inventorySlotData.StateChanged -= DataStateChanged;
    }
    

    public InventorySlotData InventorySlot
    {
        get =>  _inventorySlotData;
        set
        {
            ChangeListen(value);
            NotifyStateChanged();
        }
    }

    public void DataStateChanged(object sender,  ItemStack e)
    {
        NotifyStateChanged();
    }

    // 滚动时，虚拟slot需要更改绑定的data slot，需要更新监听
    private void ChangeListen(InventorySlotData newSlotData)
    {
        if (_inventorySlotData != null)
        {
            _inventorySlotData.StateChanged -= DataStateChanged;
        }
        _inventorySlotData = newSlotData;
        if (newSlotData != null)
        {
            _inventorySlotData.StateChanged += DataStateChanged;
        }
            
    }
    
    private void NotifyStateChanged()
    {
        StateChanged?.Invoke(this, _inventorySlotData);
    }
}
