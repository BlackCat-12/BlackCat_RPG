using System.Collections;
using System.Collections.Generic;
using Inventory_Drag;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDrapHandler : MonoBehaviour ,IDropHandler
{
    [SerializeField] private UI_InventorySlot _uiInventorySlot;
    
    public void OnDrop(PointerEventData eventData)
    {
        GameObject go = eventData.pointerDrag;
        var drapHandler = go.GetComponent<DrapHandler>();
       
        if (drapHandler != null)
        {
            Transform originSlotTransform =  go.GetComponent<DrapHandler>()._parentTransform;
            UI_InventorySlot originSlot = originSlotTransform.GetComponent<UI_InventorySlot>();
            if (originSlot == null) return;
            bool canAccept = _uiInventorySlot.CheckDragDown(originSlot);  // 查看是否可以移动到目标slot
            //Debug.Log($"Drop {originSlot.DataListIndex}  to {_uiInventorySlot.DataListIndex}");
            //Debug.Log(canAccept);
            if (!canAccept) return;
            _uiInventorySlot.MoveToTargetSlot(originSlot);  // 移动向目标slot
        }
    }
}
