using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImpactState : PlayerBaseState
{
    private readonly int Impact_PlayerHash = Animator.StringToHash("Impact_Player");
    private const float CrossFadeDuration = 0.2f;
    private const float AnimatorDampTime = 0.1f;
    private float duration = 1f;
    
    public PlayerImpactState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(Impact_PlayerHash, CrossFadeDuration); // 立即进入过渡状态
    }

    public override void Update(float dt)
    {
        Move(dt);
        duration -= dt;
        if (duration <= 0f)
        {
            ReturnToLocomotion();
        }
    }

    public override void Exit()
    {
        
    }
}
