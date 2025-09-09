using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPullUpState : PlayerBaseState
{
    private readonly int PullUpHash = Animator.StringToHash("PullUp");
    private const float CrossFadeDuration = 0.2f;
    private readonly Vector3 Offset = new Vector3(0f, 2.325f, 0.65f);
    public PlayerPullUpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(PullUpHash, CrossFadeDuration);
    }
    public override void Update(float dt)
    {
        if (GetNormalizedTime() <= 1f) return;  // 要获取正确片段的进度
        stateMachine.CharacterController.enabled = false;
        stateMachine.transform.Translate(Offset, Space.Self);
        stateMachine.CharacterController.enabled = true;
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine, false));
    }
    public override void Exit()
    {
        stateMachine.Animator.applyRootMotion = false;
    }
    
    private float GetNormalizedTime()
    {
        AnimatorStateInfo currInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = stateMachine.Animator.GetNextAnimatorStateInfo(0);

        if (stateMachine.Animator.IsInTransition(0) ) // 正在过渡需要返回新动作的播放进度，并以新进度为窗口判断
        {
            return nextInfo.normalizedTime;
        }else if (!stateMachine.Animator.IsInTransition(0) )  // 没在进行过度，即还没按下攻击，则以当前进度为标准
        {
            return currInfo.normalizedTime;
        }
        return 0f;
    }
}
