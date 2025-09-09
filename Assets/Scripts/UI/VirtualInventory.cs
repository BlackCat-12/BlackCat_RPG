using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualInventory
{
    private int _SlotNumber;
    List<VirtualInventorySlot> _slots;
    
    public List<VirtualInventorySlot> Slots => _slots;

    public VirtualInventory(InventoryData inventoryData, int slotNumber)
    {
        _slots = new List<VirtualInventorySlot>();
        InitVirtualSlot(inventoryData, slotNumber);
    }

    // 创建和ui_slot对应数量的virtualSlot，用来追踪更新数据更新
    private void InitVirtualSlot(InventoryData inventoryData, int slotNumber)
    {
        for (int i = 0; i < slotNumber; i++)
        {
            InventorySlotData itemData = inventoryData.InventorySlotData[i];
            VirtualInventorySlot virtualInventorySlot = new VirtualInventorySlot(itemData, i);
            _slots.Add(virtualInventorySlot);
        }
    }

    public void BindItemData(InventoryData inventoryData, int startDataIdx = 0)
    {
        int offset = startDataIdx - 0;  // 起始偏移值相减
        for (int i = 0; i < _slots.Count; i++)
        {
            int dataIndex = i + offset;
            if (dataIndex >= inventoryData.InventorySlotData.Count)  // 之后的格子没有item数据
            { 
                _slots[i].InventorySlot = null;
            }
            else
            {
                _slots[i].InventorySlot = inventoryData.InventorySlotData[i + offset];
            }
        }
    }
}
