using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockingState : PlayerBaseState
{
    private readonly int Block_PlayerHash = Animator.StringToHash("Block_Player");
    private const float CrossFadeDuration = 0.3f;
    private const float AnimatorDampTime = 0.1f;
    
    public PlayerBlockingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        stateMachine.Animator.CrossFadeInFixedTime(Block_PlayerHash, CrossFadeDuration);
    }

    public override void Enter()
    {
        stateMachine.Health.SetInvulnerable(true);
    }

    public override void Update(float dt)
    {
        Move(dt);
        if (!stateMachine.InputReader.IsBlock)
        {
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            return;
        }

        if (!stateMachine.InputReader.IsBlock && !stateMachine.Targeter.currTarget)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }
    }

    public override void Exit()
    {
        stateMachine.Health.SetInvulnerable(false);
    }
}
