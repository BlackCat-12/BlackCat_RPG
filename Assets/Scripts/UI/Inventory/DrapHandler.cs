using BlackCat_UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory_Drag
{
    public class DrapHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        [SerializeField] private Image _image;
        public Transform _parentTransform;
        public Vector2 _localPos;
        public RectTransform _inventoryRect; 
        
        public event UnityAction DropItemAction;
        private void Start()
        {
            _inventoryRect = transform.parent.GetComponent<UI_InventorySlot>().GetScrollRect();
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _parentTransform =  transform.parent;
            _localPos = transform.localPosition;
            transform.SetParent(UIManager.Instance.CanvasTransform[CanvasConst.InventoryCanvas]);
            _image.raycastTarget = false;
            
            transform.SetAsLastSibling();  // 移动到当前父物体列表中的最后一个
        }
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 localMousePos;
            bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _inventoryRect, 
                eventData.position, 
                null, 
                out localMousePos
            );
            //Debug.Log($"Inventory Rect: {_inventoryRect.rect}");
            Debug.Log($"Mouse Position: {localMousePos}");
            Debug.Log(_inventoryRect.rect.Contains(localMousePos));
            if (!_inventoryRect.rect.Contains(localMousePos))
            {
                DropItemAction?.Invoke();
            }
            
            _image.raycastTarget = true;
            transform.SetParent(_parentTransform);
            transform.localPosition = _localPos;
        }
    }
}