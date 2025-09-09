using System.Collections;
using System.Collections.Generic;
using BlackCat_UI;
using UnityEngine;
using UnityEngine.UI;

public class TestPanel : StaticUIPanel
{
    [SerializeField] private Button button;
    
    
    private void Awake()
    {
        Init();
    }
    public override void Init()
    {
        button.onClick.AddListener(ChangeToMain);
        transform.SetParent(UIManager.Instance.CanvasTransform[CanvasConst.StaticCanvas], false);
    }
    
    public void ChangeToMain()
    {
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
        UIManager.Instance.ClosePanel(UIConst.TestPanel);
    }
}
