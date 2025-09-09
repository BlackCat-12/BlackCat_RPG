using System;
using System.Collections;
using System.Collections.Generic;
using BlackCat_UI;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : StaticUIPanel
{
    [SerializeField] private Button button;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        button.onClick.AddListener(ChangeToTest);
        transform.SetParent(UIManager.Instance.CanvasTransform[CanvasConst.StaticCanvas], false);
    }

    public override void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    public override void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void ChangeToTest()
    {
        UIManager.Instance.OpenPanel(UIConst.TestPanel);
        UIManager.Instance.ClosePanel(UIConst.MainPanel);
    }
    
}
