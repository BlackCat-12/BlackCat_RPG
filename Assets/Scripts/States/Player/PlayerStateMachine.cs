using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    // ****************************** Component *********************************
    [field: SerializeField] public InputReader InputReader{get; private set;}
    [field: SerializeField] public CharacterController CharacterController{get; private set;}
    [field: SerializeField] public Animator Animator{get; private set;}
    [field: SerializeField] public Targeter Targeter{get; private set;}
    [field: SerializeField] public ForceReceiver ForceReceiver{get; private set;}
    [field: SerializeField] public WeaponDamage WeaponDamage{get; private set;}
    [field: SerializeField] public Attack[] Attacks{get; private set;}
    [field: SerializeField] public Health Health{get; private set;}
    [field: SerializeField] public RagDoll RagDoll{get; private set;}
    [field: SerializeField] public LedgeDetector LedgeDetector{get; private set;}
    
    // ****************************** Variability ***********************************
    [field: SerializeField] public float FreeLookSpeed{get; private set;}
    [field: SerializeField] public float TargetingSpeed{get; private set;}
    [field: SerializeField] public float RotationDamping{get; private set;}
    [field: SerializeField] public float DodgeDuration{get; private set;}
    [field: SerializeField] public float DodgeLength{get; private set;}
    [field: SerializeField] public float DodgeCoolDown{get; private set;}
    [field: SerializeField] public float JumpForce{get; private set;}
	public Transform MainCameraTransform {get; private set; }
    public float PrevoiusDodgeTime { get; private set; } = -1f;
    
    public void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        
		MainCameraTransform = Camera.main.transform;
        SwitchState(new PlayerFreeLookState(this));
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
    private void HandleTakeDamage()
    {
        SwitchState(new PlayerImpactState(this));
    }
    
    private void HandleDead()
    {
        SwitchState(new PlayerDeadState(this));
    }

    public void SetDodgeTime(float time)
    {
        PrevoiusDodgeTime = time;
    }
}
