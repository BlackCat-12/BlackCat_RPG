using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingState : PlayerBaseState
{
    private Attack attack;
    private float previousFrameTime = 0f;
    private bool hasAttacked = false;
    public PlayerAttackingState(PlayerStateMachine stateMachine, int attackIndex) : base(stateMachine)
    {
        attack = stateMachine.Attacks[attackIndex];
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(attack.AttackAnimationName, attack.TransitionDuration); // 立即进入过渡状态
        stateMachine.WeaponDamage.SetDamage(attack.Damage, attack.Knockback);
    }

    public override void Update(float dt)
    {
        Move(dt);
        float normalizedTime = GetNormalizedTime();
        if (normalizedTime >= attack.ForceTime)  // 为攻击添加前向力
        {
            TryAddForce();
        }
        if (normalizedTime <　1f)  // 确保动画的正向播放
        {
            if (stateMachine.InputReader.IsAttack)
            {
                TryComboAttack(normalizedTime);
            }
        }
        else  // 如果攻击动画播放完毕，根据是否有攻击目标切换到之前的状态
        {
            if (stateMachine.Targeter.currTarget == null)
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            }
            else
            {
                stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            }
        }
        previousFrameTime = normalizedTime;
    }



    public override void Exit()
    {
        
    }
    private void TryAddForce()
    {
        if (!hasAttacked)
        {
            stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * attack.Force);
            hasAttacked = true;
        }
    }
    
    private void TryComboAttack(float normalizedTime)
    {
        if (attack.ComboStateIndex == -1) { return; }
        if (attack.ComboAttackTime > normalizedTime) { return; }
        
        stateMachine.SwitchState(new PlayerAttackingState(stateMachine, attack.ComboStateIndex));  // 切换到下一个Combo
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
