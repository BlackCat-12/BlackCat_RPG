using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace  BlackCat_UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _uiManager;
        
        private Transform _dynamicCanvasTransform;
        private Transform _staticCanvasTransform;
        private Dictionary<string, Transform> _CanvasTransformDict;
        private Dictionary<string, StaticUIPanel> _staticUIPanels;
        private Dictionary<string, StaticUIPanel> _activeStaticPanels;
        private Dictionary<string, DynamicUIPanel> _dynamicUIPanels;
        
        private const string _panelPath = "Prefabs\\Panels";

        public static UIManager Instance
        {
            get
            {
                if (_uiManager == null)
                {
                    _uiManager =  FindObjectOfType<UIManager>();
                    if (_uiManager == null)
                    {
                        GameObject go = new GameObject("UIManager");
                        _uiManager = go.AddComponent<UIManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _uiManager;
            }   
        }
        public Dictionary<string, Transform> CanvasTransform => _CanvasTransformDict;

        
        private void Awake()
        {
            _uiManager  = this;
            _staticUIPanels =  new Dictionary<string, StaticUIPanel>();
            _activeStaticPanels = new Dictionary<string, StaticUIPanel>();
            _dynamicUIPanels = new Dictionary<string, DynamicUIPanel>();
            _CanvasTransformDict = new Dictionary<string, Transform>();
            InitCanvasTransform();
            InitStaticPanels();
        }

        private void Start()
        {
       
        }
        
        public IBasePanel GetPanel(string name)
        {
            IBasePanel panel = null;
            if (_dynamicUIPanels.ContainsKey(name))
            {
                return _dynamicUIPanels[name];
            }
            if (_activeStaticPanels.ContainsKey(name))
            {
                return _activeStaticPanels[name];
            }
            return null;
        }

        public UI_Inventory GetInventoryUI()
        {
            return _staticUIPanels[UIConst.PackagePanel] as UI_Inventory;
        }
        
        public IBasePanel OpenPanel(string name)
        {
            IBasePanel panel = null;
            // 检查是否已打开
            bool isStatic = name[0] == 'S';

            if (isStatic)
            {
                return OpenStaticPanel(name);
            }
            else
            {
                return OpenDynamicPanel(name);
            }
        }

        public bool ClosePanel(string name)
        {
            bool isStatic = name[0] == 'S';
            if (isStatic)
            {
                if (!_activeStaticPanels.TryGetValue(name, out StaticUIPanel panel))
                {
                    Debug.LogWarning($"Panel {name} not found");
                    return false;
                }
                panel.ClosePanel();
                _activeStaticPanels.Remove(name);
                return true;
            }
            else
            {
                if (!_dynamicUIPanels.TryGetValue(name, out DynamicUIPanel panel))
                {
                    Debug.LogWarning($"Panel {name} not found");
                    return false;
                }
                panel.ClosePanel();
                _dynamicUIPanels.Remove(name);
                return true;
            }
        }

        private void InitStaticPanels()
        {
            foreach (var pair in UIConst.StaticPanel)
            {
                StaticUIPanel panel = LoadPanelFromPath(pair.Value) as StaticUIPanel;
                if (panel == null)
                {
                    Debug.Log($"Panel {pair.Key} 初始化加载失败");
                    continue;
                }
                _staticUIPanels.Add(pair.Key, panel);
            }
        }

        private StaticUIPanel OpenStaticPanel(string name)
        {
            if (_activeStaticPanels.ContainsKey(name))
            {
                return _activeStaticPanels[name];
            }

            if (!_staticUIPanels.TryGetValue(name, out StaticUIPanel panel))
            {
                Debug.Log("静态Panel打开失败" + name);
            }
            panel.OpenPanel();
            _activeStaticPanels.Add(name, panel);
            return panel;
        }

        private DynamicUIPanel OpenDynamicPanel(string name)
        {
            if (_dynamicUIPanels.ContainsKey(name))
            {
                return _dynamicUIPanels[name];
            }
            string path = UIConst.DynamicPanel[name];
            DynamicUIPanel panel = LoadPanelFromPath(path) as DynamicUIPanel;
            if (panel == null)
            {
                Debug.Log("动态panel加载失败 " + name);
            }
            _dynamicUIPanels.Add(name, panel);
            panel.OpenPanel();
            return panel;
        }
        private IBasePanel LoadPanelFromPath(string name)
        {
            string fullPath = Path.Combine(_panelPath, name); 
            string resourcePath = Path.Combine("Assets", "Resources", fullPath) + ".prefab";
            // 检查路径是否配置
            if (!File.Exists(resourcePath))
            {
                Debug.Log("路径不存在 " + resourcePath);
                return null;
            }
    
            GameObject panelGO = Resources.Load<GameObject>(fullPath);
            // 打开界面
            GameObject panelObject = GameObject.Instantiate(panelGO, _staticCanvasTransform, false);
            IBasePanel panel = panelObject.GetComponent<IBasePanel>();
            panel.Init();
            return panel;
        }

        private void InitCanvasTransform()
        {
            _CanvasTransformDict.Add("StaticCanvas", GameObject.Find("StaticCanvas").transform);
            _CanvasTransformDict.Add("DynamicCanvas", GameObject.Find("DynamicCanvas").transform);
            _CanvasTransformDict.Add("InventoryCanvas", GameObject.Find("InventoryCanvas").transform);
        }
    }

    public static class UIConst
    {
        public const string MainPanel = "S_MainPanel";
        public const string TestPanel = "S_TestPanel";
        
        public static readonly Dictionary<string, string> StaticPanel = new Dictionary<string, string>()
        {
            { "S_MainPanel", "StaticPanels/MainPanel" },
            {"S_TestPanel", "StaticPanels/TestPanel" },
            { "S_PackagePanel", "StaticPanels/PackagePanel" },
        };
        
        public const string PackagePanel = "S_PackagePanel";
        public static readonly Dictionary<string, string> DynamicPanel = new Dictionary<string, string>()
        {
           
        };
    }

    public static class CanvasConst
    {
        public const string StaticCanvas = "StaticCanvas";
        public const string DynamicCanvas = "DynamicCanvas";
        public const string InventoryCanvas = "InventoryCanvas";
    }
}

