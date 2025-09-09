using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.RagDoll.ToggleRagDoll(true);
        stateMachine.WeaponDamage.gameObject.SetActive(false);
    }

    public override void Update(float dt)
    {
        
    }

    public override void Exit()
    {
        
    }
}
