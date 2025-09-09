using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState : State
{
    protected EnemyStateMachine stateMachine;
    
    protected EnemyBaseState(EnemyStateMachine  stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected bool IsInChaseRange()
    {
        Vector2 distance = stateMachine.Player.transform.position - stateMachine.transform.position;
        return distance.sqrMagnitude <= stateMachine.PlayerChasingRange * stateMachine.PlayerChasingRange;
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
        if (!stateMachine.Player) return;
        Vector3 Direction = Vector3.Normalize(stateMachine.Player.transform.position - stateMachine.transform.position);
        Direction.y = 0;
        stateMachine.transform.rotation = Quaternion.LookRotation(Direction);
    }
    
    protected bool IsInAttackRange()
    {
        Vector2 distance = stateMachine.Player.transform.position - stateMachine.transform.position;
        return distance.sqrMagnitude <= stateMachine.AttackingRange * stateMachine.AttackingRange;
    }
}
