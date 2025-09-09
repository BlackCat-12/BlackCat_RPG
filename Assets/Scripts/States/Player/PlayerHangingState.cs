using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangingState : PlayerBaseState
{
    private readonly int Hanging_PlayerHash = Animator.StringToHash("Hanging_Player");
    private const float CrossFadeDuration = 0.2f;
    private Vector3 LedgeForward;
    private Vector3 ClosestPointOnBounds;

    public PlayerHangingState(PlayerStateMachine stateMachine ,Vector3 LedgeForward, Vector3 ClosetPos) : base(stateMachine)
    {
        this.LedgeForward = LedgeForward;
        ClosestPointOnBounds  = ClosetPos;
    }

    public override void Enter()
    {
        stateMachine.transform.rotation = Quaternion.LookRotation(LedgeForward, Vector3.up);

        stateMachine.Animator.CrossFadeInFixedTime(Hanging_PlayerHash, CrossFadeDuration);
    }

    public override void Update(float dt)
    {
        if (stateMachine.InputReader.MovementValue.y < 0)  // 玩家向后退，则下落
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }else if (stateMachine.InputReader.MovementValue.y > 0)
        {
            stateMachine.SwitchState(new PlayerPullUpState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.ForceReceiver.Reset();
    }
}
