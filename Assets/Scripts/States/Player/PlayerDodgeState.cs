using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeState : PlayerBaseState
{
    private readonly int DodgeBlendHash = Animator.StringToHash("DodgeBlendTree");
    private readonly int DodgeForwardHash = Animator.StringToHash("DodgeForward");
    private readonly int DodgeRightHash = Animator.StringToHash("DodgeRight");
    private const float CrossFadeDuration = 0.2f;
    private float remainDodgingTime;
    
    private Vector3 dodgeDirInput;
    public PlayerDodgeState(PlayerStateMachine stateMachine, Vector3 dodgeDir) : base(stateMachine)
    {
        dodgeDirInput =  dodgeDir;
    }

    public override void Enter()
    {
        stateMachine.SetDodgeTime(Time.time);
        stateMachine.Health.SetInvulnerable(true);
        
        remainDodgingTime = stateMachine.DodgeDuration;
        
        stateMachine.Animator.CrossFadeInFixedTime(DodgeBlendHash, CrossFadeDuration);
        stateMachine.Animator.SetFloat(DodgeForwardHash, dodgeDirInput.y);
        stateMachine.Animator.SetFloat(DodgeRightHash, dodgeDirInput.x);
    }

    public override void Update(float dt)
    {
        Vector3 movement = new Vector3();

        // 在duration的时间内移动length的长度
        movement += stateMachine.transform.right * (dodgeDirInput.x * stateMachine.DodgeLength) / stateMachine.DodgeDuration;
        movement += stateMachine.transform.forward * (dodgeDirInput.y * stateMachine.DodgeLength) / stateMachine.DodgeDuration;
        Move(movement ,dt);
        remainDodgingTime -= dt;
        if (remainDodgingTime <= 0)
        {
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.Health.SetInvulnerable(false);
    }
}
