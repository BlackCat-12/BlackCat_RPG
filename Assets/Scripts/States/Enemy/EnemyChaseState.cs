using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    private readonly int LocomotionBleedTreeHash = Animator.StringToHash("LocomotionBleedTree");
    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private const float CrossFadeDuration = 0.2f;
    private const float AnimatorDampTime = 0.1f;
    
    public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionBleedTreeHash, CrossFadeDuration);
    }

    public override void Update(float dt)
    {
        if (!IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }

        if (IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
        }
        stateMachine.Animator.SetFloat(SpeedHash, 1, AnimatorDampTime, dt);
        FaceTarget();
        MoveToPlayer(dt);
    }

    public override void Exit()
    {
        if (stateMachine.NavMeshAgent.isOnNavMesh)
        {
            stateMachine.NavMeshAgent.ResetPath();
        }

        stateMachine.NavMeshAgent.velocity = Vector3.zero;
    }

    private void MoveToPlayer(float dt)
    {
        if (stateMachine.NavMeshAgent.isOnNavMesh)
        {
            stateMachine.NavMeshAgent.destination = stateMachine.Player.transform.position;  // 获取ai计算的运动方向
            Move(stateMachine.NavMeshAgent.desiredVelocity.normalized * stateMachine.MovementSpeed, dt);  // desiredvelocity代表意向运动方向
        }
        stateMachine.NavMeshAgent.velocity = stateMachine.CharacterController.velocity;
    }
}
