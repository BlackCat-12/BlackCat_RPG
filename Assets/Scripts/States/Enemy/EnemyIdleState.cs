using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private readonly int LocomotionBleedTreeHash = Animator.StringToHash("LocomotionBleedTree");
    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private const float CrossFadeDuration = 0.2f;
    private const float AnimatorDampTime = 0.1f;
    
    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionBleedTreeHash, CrossFadeDuration);
    }

    public override void Update(float dt)
    {
        Move(dt);
        if (IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyChaseState(stateMachine));
        }
        
        stateMachine.Animator.SetFloat(SpeedHash, 0, AnimatorDampTime, dt);
    }

    public override void Exit()
    {
        
    }
}
