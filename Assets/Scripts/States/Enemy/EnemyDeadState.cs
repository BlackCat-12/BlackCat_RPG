using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.RagDoll.ToggleRagDoll(true);
        stateMachine.WeaponDamageComponent.gameObject.SetActive(false);
        GameObject.Destroy(stateMachine.Target);
    }

    public override void Update(float dt)
    {
        
    }

    public override void Exit()
    {
       
    }
}
