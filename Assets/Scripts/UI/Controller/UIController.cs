// UIController.cs
using UnityEngine;
using BlackCat_UI;
using Cinemachine;

public class UIController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private CinemachineInputProvider  _cinemachineInputProvider;
    
    private void OnEnable()
    {
        _inputReader.OpenInventoryEvent += HandleOpenMenuInput;
        _inputReader.CloseInventoryEvent += HandleCloseMenuInput;
        _inputReader.CancelEvent += HandleCancelInput;
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDisable()
    {
        _inputReader.OpenInventoryEvent -= HandleOpenMenuInput;
        _inputReader.CloseInventoryEvent -= HandleCloseMenuInput;
        _inputReader.CancelEvent -= HandleCancelInput;
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void HandleOpenMenuInput()
    {
        // 决策：只有在Gameplay状态下，按打开菜单键才打开背包
        if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
        {
            _uiManager.OpenPanel(UIConst.PackagePanel);
            GameStateManager.Instance.ChangeState(GameState.MenuOpen);
        }
    }

    private void HandleCloseMenuInput()
    {
        // 决策：只有在菜单打开状态下，按关闭键才有效
        if (GameStateManager.Instance.CurrentState == GameState.MenuOpen)
        {
            _uiManager.ClosePanel(UIConst.PackagePanel);
            GameStateManager.Instance.ChangeState(GameState.Gameplay);
        }
    }

    private void HandleCancelInput()
    {
        // 通用取消逻辑：根据当前状态决定行为
        switch (GameStateManager.Instance.CurrentState)
        {
            case GameState.MenuOpen:
                _uiManager.ClosePanel("D_PackagePanel");
                GameStateManager.Instance.ChangeState(GameState.Gameplay);
                break;
            case GameState.Dialogue:
                // 处理对话取消逻辑
                break;
        }
    }

    // 根据游戏状态切换输入模版
    private void HandleGameStateChange(GameState newState)
    {
        // 根据状态变化切换输入映射
        if (newState == GameState.Gameplay)
        {
            _inputReader.EnablePlayerControls();
            _cinemachineInputProvider.enabled = true;
        }
        else if (newState == GameState.MenuOpen)
        {
            _inputReader.EnableUIControls();
            _cinemachineInputProvider.enabled = false;
        }
    }
}