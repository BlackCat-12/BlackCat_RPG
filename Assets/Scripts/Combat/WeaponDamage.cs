using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    [field: SerializeField]  public Collider PlayerCollider { get; private set; }

    public float Damage { get; private set; }
    public float Knockback { get; private set; }
    private List<Collider> _colliders = new List<Collider>();

    private void OnEnable()
    {
        _colliders.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == PlayerCollider) return;
        if (_colliders.Contains(other)) return;
        _colliders.Add(other);
        
        if(other.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(Damage);
        }

        if (other.TryGetComponent<ForceReceiver>(out ForceReceiver forceReceiver))  // 施加击退力
        {
            Vector3 direction = other.transform.position - PlayerCollider.transform.position;
            forceReceiver.AddForce(direction.normalized * Knockback);
        }
    }

    public void SetDamage(float damage, float knockback)
    {
        Damage = damage;
        Knockback = knockback;
    }
}
