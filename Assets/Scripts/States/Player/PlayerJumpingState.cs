using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private const float CrossFadeDuration = 0.2f;
    private Vector3 momentum;


    public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {
        stateMachine.LedgeDetector.OnLedgeDetect += HandleHanging;
        stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        momentum = stateMachine.CharacterController.velocity;  // 保存起跳时的方向，空中保持向该方向移动
        momentum.y = 0;
    }

    public override void Update(float dt)
    {
        Move(momentum, dt);  // move只控制xz轴方向
        if (stateMachine.CharacterController.velocity.y < 0.0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }
    }

    public override void Exit()
    {
        stateMachine.LedgeDetector.OnLedgeDetect -= HandleHanging;
    }

    private void HandleHanging(Vector3 LedgeForward, Vector3 ClosestPointOnBounds)
    {
        stateMachine.SwitchState(new PlayerHangingState(stateMachine, LedgeForward, ClosestPointOnBounds));
    }
}
