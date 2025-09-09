using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions, Controls.IUIActions
{
    private Controls controls;
    
    public event Action JumpEvent;
    public event Action DodgeEvent;
    public event Action TargetEvent;
    public event Action CancelEvent;
    public event UnityAction OpenInventoryEvent;
    public event UnityAction CloseInventoryEvent;
    
    
    public bool IsAttack { get; private set; }
    public bool IsBlock { get; private set; }
    public Vector2 MovementValue { get; private set; }

    void Awake()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.UI.SetCallbacks(this);
    }

    void OnEnable()
    {
        // 默认只启用玩家控制
        EnablePlayerControls();
    }


    public void EnablePlayerControls()
    {
        controls.UI.Disable();
        controls.Player.Enable();
    }

    public void EnableUIControls()
    {
        Debug.Log("disable player Controls");
        controls.Player.Disable(); // 禁用玩家控制
        controls.UI.Enable();     // 启用UI控制
    }

    void OnDisable()
    {
        controls.Player.Disable();
        controls.UI.Disable();
    }

    void OnDestroy()
    {
        if (controls != null)
        {
            controls.Player.SetCallbacks(null);
            controls.Player.Disable();
            controls.Dispose();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        JumpEvent?.Invoke();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DodgeEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
        
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TargetEvent?.Invoke();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        CancelEvent?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsAttack = true;
        }
        else if(context.canceled)
        {
            IsAttack  = false;
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsBlock = true;
        }
        else if(context.canceled)
        {
            IsBlock  = false;
        }
    }
    
    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        OpenInventoryEvent?.Invoke();
    }

    public void OnECS(InputAction.CallbackContext context)
    {

    }
    
    // ******************************** UI **********************************

    public void OnNewaction(InputAction.CallbackContext context)
    {
        
    }

    public void OnCloseInventory(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        CloseInventoryEvent?.Invoke();
    }
}