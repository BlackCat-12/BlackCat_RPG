using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    private readonly int Targeting = Animator.StringToHash("TargetingBlendTree");
    private readonly int TargetingForwardHash = Animator.StringToHash("TargetingForward");
    private readonly int TargetingRightHash = Animator.StringToHash("TargetingRight");
    private const float CrossFadeDuration = 0.2f;

    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {
        stateMachine.InputReader.CancelEvent += OnCancel;
        stateMachine.InputReader.DodgeEvent += OnDodging;
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.Animator.CrossFadeInFixedTime(Targeting, CrossFadeDuration);
    }

    public override void Update(float dt)
    {
        if (stateMachine.InputReader.IsAttack)
        {
            stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
            return;
        }
        if (stateMachine.InputReader.IsBlock)
        {
            stateMachine.SwitchState(new PlayerBlockingState(stateMachine));
            return;
        }
        if (stateMachine.Targeter.currTarget == null)
        {
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            return;
        }
        FaceTarget();
        Vector3 movement = CalculateMovement(dt);
        UpdateAnimation(dt);
        Move(movement * stateMachine.TargetingSpeed, dt);
    }

    public override void Exit()
    {
        stateMachine.InputReader.CancelEvent -= OnCancel;
        stateMachine.InputReader.DodgeEvent -= OnDodging;
        stateMachine.InputReader.JumpEvent -= OnJump;
    }

    private void OnCancel()
    {
        stateMachine.Targeter.Cancel();
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }

    private void OnDodging()
    {
        stateMachine.SwitchState(new PlayerDodgeState(stateMachine, stateMachine.InputReader.MovementValue));

    }

    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
    }
    
    private Vector3 CalculateMovement(float dt)
    {
        Vector3 movement = new Vector3();
        
        movement += stateMachine.transform.right * stateMachine.InputReader.MovementValue.x;  // 会根据当前面朝方向移动，target状态下face会绕targeter转动
        movement += stateMachine.transform.forward * stateMachine.InputReader.MovementValue.y;
        
        return movement;
    }

    private void UpdateAnimation(float dt)
    {
        if (stateMachine.InputReader.MovementValue.x == 0)
        {
            stateMachine.Animator.SetFloat(TargetingRightHash, 0, 0.1f, dt);
        }
        else
        {
            float rightVal = stateMachine.InputReader.MovementValue.x > 0 ? 1f : -1f;
            stateMachine.Animator.SetFloat(TargetingRightHash, rightVal, 0.1f, dt);
        }
        
        if (stateMachine.InputReader.MovementValue.y == 0)
        {
            stateMachine.Animator.SetFloat(TargetingForwardHash, 0, 0.1f, dt);
        }
        else
        {
            float forwardVal = stateMachine.InputReader.MovementValue.y > 0 ? 1f : -1f;
            stateMachine.Animator.SetFloat(TargetingForwardHash, forwardVal, 0.1f, dt);
        }
    }
    
}
