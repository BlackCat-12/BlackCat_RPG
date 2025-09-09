using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

public class InventoryCreator
{
    private static readonly int[] availableItemIds =
    {
        101,102,103,104,105,
        201,
        301,
        401,
    };
    
    [MenuItem("CMD/Create Inventory")]
    public static void CreateAndSaveInventory()
    {
        // 1. 创建背包数据
        InventoryData inventoryData = CreateInventoryData();
        
        // 2. 序列化为JSON
        string json = JsonConvert.SerializeObject(inventoryData, Formatting.Indented);
        
        // 3. 保存到文件
        string filePath = Application.persistentDataPath + "/Inventory/inventory_data.json";
        string directory = Path.GetDirectoryName(filePath);
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllText(filePath, json);
        
        // 4. 刷新Unity资源数据库
        AssetDatabase.Refresh();
        
        Debug.Log($"背包数据已保存到: {filePath}");
        Debug.Log($"JSON内容:\n{json}");
    }
    
    private static InventoryData CreateInventoryData()
    {
        InventoryData inventoryData = new InventoryData
        {
            InventorySlotData = new List<InventorySlotData>()
        };
        
        // 创建5个不同的Slot
        for (int i = 0; i < 100; i++)
        {
            InventorySlotData slot = CreateSlotData(i);
            inventoryData.InventorySlotData.Add(slot);
        }
        
        return inventoryData;
    }
    
    private static InventorySlotData CreateSlotData(int slotIndex)
    {
        ItemStack stack = CreateStack();

        InventorySlotData slot = new InventorySlotData(stack);
        return slot;
    }

    private static ItemStack CreateStack()
    {
        ItemStack itemStack = new ItemStack();
        Item item = new Item();
        itemStack.Item = item;
        
        int id = availableItemIds[Random.Range(0, availableItemIds.Length)];
        item.ID = id;
        item.Level = Random.Range(0, 10);
        item.Star = Random.Range(0, 5);
        if (id / 100 == 1)  // 如果可堆叠
        {
            itemStack.NumberOfItems = Random.Range(1, 8);
        }
        else
        {
            itemStack.NumberOfItems = 1;
        }
        return  itemStack;
    }
    
    
    private const string InventoryDataPath = "/Inventory/inventory_data.json";
    private const string InventoryItemDataPath = "/Inventory/inventory_Item_data.json";
}