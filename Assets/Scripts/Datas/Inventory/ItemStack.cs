using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ItemStack
{
    private Item _item;
    private int _numberOfItems;
    private ItemDefinitionSO _itemDefinition;

    [JsonProperty]
    public Item Item
    {
        get => _item;
        set => _item = value;
    }
    [JsonProperty]
    public int NumberOfItems {  // 设置物品数量,非可堆叠物品数量最多为1
        get => _numberOfItems;
        set
        {
            value = value < 0 ? 0 : value;
            _numberOfItems = IsStackable ? value : 1;
        }
    }

    [JsonIgnore]
    public ItemDefinitionSO ItemDefinition
    {
        get => _itemDefinition;
        set => _itemDefinition = value;
    }

    [JsonIgnore]
    public bool IsStackable => _itemDefinition?.stackable ?? false; // 若为 null，默认不可堆叠

    [JsonIgnore]
    public int ID => _item.ID;
    
    public ItemStack(){}
    
    private async Task<ItemDefinitionSO> GetItemDefinitionAsync(int itemId)
    {
        return await AdvancedItemManager.Instance.LoadItemDefinitionAsync(itemId);
    }
}
