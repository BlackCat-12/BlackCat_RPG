using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Definition", fileName = "Item_")]
public class ItemDefinitionSO : ScriptableObject
{
    [Header("基础信息")]
    public int id;
    public string addressKey; // 手动设置的地址标识符
    public string itemName;
    public string description;
    public ItemType type;
    public int starRank;
    
    [Header("显示设置")]
    public Sprite icon;
    public GameObject prefab; // 3D模型或世界中的表现
    
    [Header("属性")]
    public bool stackable = false;
    public int maxStackSize = 1;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        // 在Editor中自动生成或验证地址
        if (string.IsNullOrEmpty(addressKey))
        {
            addressKey = $"Item_{id:000}";
        }
    }
#endif
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Material,
    Ornament,
}