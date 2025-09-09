using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        UI_InventoryController inventoryController = UI_InventoryController.Instance;
        Debug.Log(other.name);
        // 若物体不是掉落物，或是背包未找到插入槽位则返回
        if (!other.TryGetComponent<GameItem>(out var item) || !inventoryController.CanAcceptItem(item.ItemStack)) return;
        inventoryController.AddItem(item.Pick());
    }
}
