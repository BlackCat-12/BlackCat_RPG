using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;

    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(Vector3 movement, float deltaTime)
    {
        stateMachine.CharacterController.Move((movement + stateMachine.ForceReceiver.Movement()) * deltaTime);
    }

    protected void Move(float deltaTime)  // 原地不动，但是可以下落
    {
        Move(Vector3.zero, deltaTime);
    }

    protected void FaceTarget()
    {
        if (!stateMachine.Targeter.currTarget) return;
        Vector3 Direction = Vector3.Normalize(stateMachine.Targeter.currTarget.transform.position - stateMachine.transform.position);
        Direction.y = 0;
        stateMachine.transform.rotation = Quaternion.LookRotation(Direction);
    }
    
    protected void ReturnToLocomotion()
    {
        if (stateMachine.Targeter.currTarget)
        {
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        }
        else
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }
    }
}
