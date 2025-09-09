using System;
using System.Collections;
using System.Collections.Generic;
using BlackCat_UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Inventory : StaticUIPanel
{
    [SerializeField] private RectTransform _scrollRectTransform;
    private int _totalNum;
    private ScrollRect _scrollRect;
    private float _height;
    private float _width;
    private RectTransform _contentRect;

    //cell info
    [SerializeField] private GameObject _ui_InventorySlot;
    [SerializeField]public float offsetX = 2f;
    [SerializeField]public float offsetY = 10f;
    private UI_InventorySlot _slot;
    private float _cellHeight;
    private float _cellWidth;

    [SerializeField] private float _cellStartOffsetX;
    [SerializeField] private float _cellStartOffsetY;
    // content能显示的最大行列
    private int _row;

    private int _column;

    //列表最多显示的cell个数
    private int _maxShowCellNum;

    //列表显示的cell列表，用于设置位置,只存了0-显示的个数这么多
    private List<UI_InventorySlot> _showCellItems;
    
    //总行数
    private int _totalRow;

    //上次显示的节点序号
    private int _preStartIndex = 0;
    
    public List<UI_InventorySlot> ShowCellItems => _showCellItems;
    
    // UIInventory订阅
    public UnityAction<UI_InventorySlot, int> updateItemAction;  // 当前格子索引，数据列表索引
    public UnityAction<int> clickItemAction;
    public UnityAction<UI_InventorySlot> drapItemAction;
    public UnityAction<UI_InventorySlot, UI_InventorySlot> MoveItemAction;
    public Func<UI_InventorySlot, UI_InventorySlot, bool> CheckDragDownAction;
    public UnityAction<int> RebindViewToModelAction;
    public UnityAction<ItemStack, UnityAction<ItemStack, ItemDefinitionSO>> LoadItemDefinitionAction;
    
    // Controller订阅
    public UnityAction<List<UI_InventorySlot>, int> OnInventoryOpened;
    public UnityAction<List<UI_InventorySlot>> OnInventoryClosed;
    
    private bool hasCreateSlot = false;


    void OnEnable()
    {
        Initialize();
    }

    void OnDisable()
    {
        OnInventoryClosed?.Invoke(_showCellItems);
    }
    
    public int InitScroll(int slotNumber)
    {
        _totalNum = slotNumber;
        transform.SetParent(UIManager.Instance.CanvasTransform[CanvasConst.InventoryCanvas], false);
        _scrollRect = _scrollRectTransform.GetComponent<ScrollRect>();
        _contentRect = _scrollRectTransform.GetComponent<ScrollRect>().content;
        _scrollRect.onValueChanged.AddListener(ScrollViewOnValueChanged);
        var rect = _scrollRectTransform.rect;   // 获取当前scroll的长宽
        _height = rect.height;
        _width = rect.width;
        _slot = _ui_InventorySlot.GetComponent<UI_InventorySlot>();
        var cellRect = _slot.GetComponent<RectTransform>().rect;  // 拿到cell格子的宽高
        _cellHeight = cellRect.height;
        _cellWidth = cellRect.width;

        //列
        _column = Mathf.FloorToInt(_width / (_cellWidth + offsetX));  // 列尽可能少，下取整
        //行
        _row = Mathf.CeilToInt(_height / (_cellHeight + offsetY));  // 行尽可能多，显示完全
      
        _maxShowCellNum = _column * (_row + 1);  // 多创建一行，防止突然创建一行的ui
            
        _totalRow = Mathf.CeilToInt((float)_totalNum / _column);  // 拿到总行数，上取整
        
        //设置content的大小
        var contentHeight = _totalRow * (_cellHeight + offsetY); //向上取整，并且乘以每一行的高
        var contentWidth = _contentRect.sizeDelta.x;
        var contentSize = new Vector2(contentWidth, contentHeight);
        _contentRect.sizeDelta = contentSize;
        
        return _maxShowCellNum;
    }


    public void Initialize()
    {
        if (!hasCreateSlot)  //如果是首次打开，先创建slot
        {
            CreateCell();
            OnInventoryOpened?.Invoke(_showCellItems, 0);
            UpdateScrollView(true);
            hasCreateSlot = true;
        }
        else  // 不是首次打开，初始化y轴位置为0，进行数据更新
        {  
            // 更新data
            Vector2 currentPos = _contentRect.anchoredPosition;

            currentPos.y = 0f;

            _contentRect.anchoredPosition = currentPos;
            OnInventoryOpened?.Invoke(_showCellItems, 0);  // 要在绑定完virtual slot和view后再进行data更新
            UpdateScrollView(true);
        }
        
    }

    private void ScrollViewOnValueChanged(Vector2 arg0)
    {
        UpdateScrollView();
    }
    
    public void UpdateScrollView(bool forceUpdate = false)
    {
        var y = _contentRect.anchoredPosition.y;
        //可能会小于0
        y = Mathf.Max(0, y);
        
        var moveRow = Mathf.FloorToInt(y / (_cellHeight + offsetY));  // 根据当前content的y滚过的距离计算滚过的row
        //视图范围内,移动的行数+视图显示的行数<=总行数
        int startIndex =  moveRow * _column; //起始序号 (行列下标从 0 开始)
        if (moveRow >= 0 && (moveRow + _row) <= _totalRow)
        {
            //和上次的起始序号不同才进行刷新
            if (!forceUpdate && startIndex == _preStartIndex)
            {
                return;
            }

            print(startIndex + ":" + _preStartIndex);

            //更新所有cell
            for (int i = startIndex; i < startIndex + _maxShowCellNum; i++)
            {
                var index = i;
                ScrollUpdateCell(index, startIndex);
            }
            _preStartIndex = startIndex;
        }

        RebindViewToData(startIndex);  // 更新data和view的绑定
    }

    private void RebindViewToData(int startIndex)
    {
        RebindViewToModelAction?.Invoke( startIndex);
    }

    void ScrollUpdateCell(int index, int startIndex)
    {
        //Debug.Log($"count : {_showCellItems.Count} index : {index} startIndex : {startIndex}");
        var scrollViewItem = _showCellItems[index - startIndex];  // 取出要更新的slot
        //超出总数的不显示
        if (index >= _totalNum)
        {
            scrollViewItem.gameObject.SetActive(false);
            return;
        }

        scrollViewItem.gameObject.SetActive(true);

        UpdateSlotPos(scrollViewItem, index);
    }
    
    public void CreateCell()  
    {
        //var showCell = Mathf.Min(_maxShowCellNum, _totalNum);  // 创建满屏，防止后续再添加新的uislot
        _showCellItems = new List<UI_InventorySlot>(_maxShowCellNum);  // 追踪当前显示的slot的列表
        for (int i = 0; i < _maxShowCellNum; i++)  // 分帧创建slot
        {
            var index = i;

            var go = GameObject.Instantiate(_ui_InventorySlot, _contentRect);
            go.name = $"Cell_{index}";
            var scrollItem = go.GetComponent<UI_InventorySlot>();
            UpdateSlotPos(scrollItem, index);
            scrollItem.AddButtonClickListener(clickItemAction);
            scrollItem.AddDropItemAction(drapItemAction);
            scrollItem.AddMoveToTargetSlotAction(MoveItemAction);
            scrollItem.AddDragDragDownCheck(CheckDragDownAction);
            scrollItem.AddLoadItemDefinitionAction(LoadItemDefinitionAction);
            
            go.SetActive(true);
            scrollItem.Init(_scrollRectTransform, this);  // 依赖注入
            
            _showCellItems.Add(scrollItem);
        }
    }
    
    void UpdateSlotPos(UI_InventorySlot scrollItem, int index)  // index可能大于data size，后面需要判断一下
    {
        scrollItem.UpdateCellPos(GetCellPos(index));
        scrollItem.DataListIndex = index;
        //updateItemAction?.Invoke(scrollItem, index);
    }
    
    // 根据物品数据的idx来更新他的显示位置，拖动时整体下移
    Vector2 GetCellPos(int index)
    {
        var curColumn = index % _column; //当前的列
        var curX = _cellStartOffsetX + curColumn * (_cellWidth + offsetX); //当前的x坐标

        var curRow = index / _column; //当前的行
        var curY = -_cellStartOffsetY - curRow * (_cellHeight + offsetY); //当前的y坐标

        var pos = new Vector2(curX, curY);
        return pos;
    }

    public override void Init()
    {
        
    }
}
