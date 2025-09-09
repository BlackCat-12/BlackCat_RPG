using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    private readonly int Attacking_EnemyHash = Animator.StringToHash("Attacking_Enemy");
    private const float CrossFadeDuration = 0.2f;
    private const float AnimatorDampTime = 0.1f;
    
    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(Attacking_EnemyHash, CrossFadeDuration);
        stateMachine.WeaponDamageComponent.SetDamage(stateMachine.WeaponDamage, stateMachine.KnockForce);
    }

    public override void Update(float dt)
    {
        if (GetNormalizedTime() >= 1)
        {
            stateMachine.SwitchState(new EnemyChaseState(stateMachine));
        }
    }

    public override void Exit()
    {
        
    }
    
    private float GetNormalizedTime()
    {
        AnimatorStateInfo currInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = stateMachine.Animator.GetNextAnimatorStateInfo(0);

        if (stateMachine.Animator.IsInTransition(0) && nextInfo.IsTag("Attack")) // 正在过渡需要返回新动作的播放进度，并以新进度为窗口判断
        {
            return nextInfo.normalizedTime;
        }else if (!stateMachine.Animator.IsInTransition(0) && currInfo.IsTag("Attack"))  // 没在进行过度，即还没按下攻击，则以当前进度为标准
        {
            return currInfo.normalizedTime;
        }
        return 0f;
    }
}

