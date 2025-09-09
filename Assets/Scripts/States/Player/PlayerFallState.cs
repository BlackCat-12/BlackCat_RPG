using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    private readonly int FallHash = Animator.StringToHash("Fall");
    private const float CrossFadeDuration = 0.2f;
    private Vector3 momentum;
    
    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(FallHash, CrossFadeDuration);
        momentum = stateMachine.CharacterController.velocity;
        momentum.y = 0;
    }

    public override void Update(float dt)
    {
        Move(momentum ,dt);
        if (stateMachine.CharacterController.isGrounded)
        {
            ReturnToLocomotion();
            return;
        }
    }

    public override void Exit()
    {
        
    }
}
