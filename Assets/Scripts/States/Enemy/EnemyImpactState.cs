using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImpactState : EnemyBaseState
{
    private readonly int Impact_EnemyHash = Animator.StringToHash("Impact_Enemy");
    private const float CrossFadeDuration = 0.2f;
    private const float AnimatorDampTime = 0.1f;
    private float duration = 1f;
    
    public EnemyImpactState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(Impact_EnemyHash, CrossFadeDuration); // 立即进入过渡状态
    }

    public override void Update(float dt)
    {
        Move(dt);
        duration -= dt;
        if (duration <= 0f)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }
    }

    public override void Exit()
    {
        
    }
}
