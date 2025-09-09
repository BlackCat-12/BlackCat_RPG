using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : StateMachine
{
    // ************************************ Component *********************************
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController CharacterController{get; private set;}
    [field: SerializeField] public ForceReceiver ForceReceiver{get; private set;}
    [field: SerializeField] public NavMeshAgent NavMeshAgent{get; private set;}
    [field: SerializeField] public float MovementSpeed{get; private set;}
    [field: SerializeField] public WeaponDamage WeaponDamageComponent{get; private set;}
    [field: SerializeField] public Health Health{get; private set;}
    [field: SerializeField] public Target Target{get; private set;}
    [field: SerializeField] public RagDoll RagDoll{get; private set;}
    
    // ************************************ Variable **********************************
    [field: SerializeField] public float PlayerChasingRange { get; private set; }
    [field: SerializeField] public float AttackingRange { get; private set; }
    [field: SerializeField] public float WeaponDamage{get; private set;}
    [field: SerializeField] public float KnockForce{get; private set;}
    public GameObject Player { get; private set; }
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        
        NavMeshAgent.updatePosition = false;  // 自定义物体的移动和旋转
        NavMeshAgent.updateRotation = false;
        
        SwitchState(new EnemyIdleState(this));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, PlayerChasingRange);
    }

    private void OnEnable()
    {
        Health.OnTakeDamage += HandleTakeDamage;
        Health.OnDead += HandleDead;
    }


    private void OnDisable()
    {
        Health.OnTakeDamage -= HandleTakeDamage;
        Health.OnDead -= HandleDead;
    }

    private void HandleDead()
    {
        SwitchState(new EnemyDeadState(this));
    }
    
    private void HandleTakeDamage()
    {
        SwitchState(new EnemyImpactState(this));
    }
}
