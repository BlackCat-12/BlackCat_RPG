using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _gameStateManager;
    public static GameStateManager Instance => _gameStateManager;
    
    public GameState CurrentState { get; private set; }
    public static event UnityAction<GameState> OnGameStateChanged;
    
    private void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _gameStateManager = this;
        }
    }
    
    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;
        
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        
        Debug.Log($"Game state changed to: {newState}");
    }
}

public enum GameState
{
    Gameplay, // 正常游戏状态，玩家控制角色
    MenuOpen, // 菜单、背包等UI打开的状态
    Dialogue, // 对话中
}
