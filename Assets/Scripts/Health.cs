using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour
{
    [field: SerializeField]  public float MaxHealth { get; private set; }
    private float _currentHealth;
    public event Action OnTakeDamage;  // 受伤事件
    public event Action OnDead; 
    private bool invulnerable;
    void Start()
    {
        _currentHealth =  MaxHealth;
    }

    public void SetInvulnerable(bool invulnerable)
    {
        this.invulnerable = invulnerable;
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0)
        {
            return;
        }

        if (invulnerable)
        {
            return;
        }
        OnTakeDamage?.Invoke();
        _currentHealth = math.max(0, _currentHealth - damage);
        if (_currentHealth <= 0)
        {
            OnDead?.Invoke();
        }
    }
}
