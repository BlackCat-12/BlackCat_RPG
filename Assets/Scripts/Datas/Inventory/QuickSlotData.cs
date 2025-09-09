using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotData : BaseSlotData
{
    public int SlotIndex;

    public QuickSlotData(ItemStack itemStack, int slotIndex) : base(itemStack)
    {
        SlotIndex = slotIndex;
    }
}
