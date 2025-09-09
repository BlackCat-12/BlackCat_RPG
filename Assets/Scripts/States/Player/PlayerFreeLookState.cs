using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    private readonly int freeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private readonly int FreeLookBlendTree = Animator.StringToHash("FreeLookBlendTree");
    private const float CrossFadeDuration = 0.2f;
    private const float AnimatorDampTime = 0.1f;
    private bool shouldFade;
    
    public PlayerFreeLookState(PlayerStateMachine stateMachine, bool shouldFade = true) : base(stateMachine)
    {
        this.shouldFade = shouldFade;
    }
    
    public override void Enter()
    {
        stateMachine.InputReader.TargetEvent +=  OnTarget;
        stateMachine.InputReader.JumpEvent += OnJump;
        if (shouldFade)
        {
           stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTree, CrossFadeDuration);
        }
        else
        {
            stateMachine.Animator.Play(FreeLookBlendTree);
        }
    }


    public override void Update(float dt)
    {
        if (stateMachine.InputReader.IsAttack)
        {
            stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
            return;
        }
        Vector3 movement = CalculateMovement();
        
        Move(movement * stateMachine.FreeLookSpeed, dt);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(freeLookSpeedHash, 0, AnimatorDampTime, dt);
            return;
        }
        stateMachine.Animator.SetFloat(freeLookSpeedHash, 1, AnimatorDampTime, dt);
        FaceMovementDirection(movement,dt);
    }

    public override void Exit()
    {
        stateMachine.InputReader.TargetEvent -=  OnTarget;
        stateMachine.InputReader.JumpEvent -= OnJump;
    }

    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
    }
    private Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = stateMachine.MainCameraTransform.right;
        right.y = 0;
        right.Normalize();
        
        return forward * stateMachine.InputReader.MovementValue.y + right * stateMachine.InputReader.MovementValue.x;
    }

    private void FaceMovementDirection(Vector3 movement,float dt)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(stateMachine.transform.rotation, Quaternion.LookRotation(movement), 
            dt * stateMachine.RotationDamping);
    }

    private void OnTarget()
    {
        if (!stateMachine.Targeter.SelectTarget()) return;  // 若是Targeter列表中没有对象，则直接返回
        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
    }
    
}