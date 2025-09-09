using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class InventoryItemData
{
    List<ItemStack> _items = new List<ItemStack>();

    [JsonProperty]
    public List<ItemStack> Items
    {
        get => _items;
        set => _items = value;
    }
}
