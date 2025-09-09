using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForceReceiver : MonoBehaviour
{
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private CharacterController controller;
    [SerializeField] private float drag = 0.3f;
    private float verticalVelocity;
    private Vector3 dampingVelocity;
    private Vector3 impact;
    
    public Vector3 Movement()
    {
        return impact + Vector3.up * verticalVelocity;
    }

    void Update()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -0.4f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
        
        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, drag);

        if (Agent && impact.sqrMagnitude < 0.2f * 0.2f)  // 防止过慢退出受击状态
        {
            impact = Vector3.zero;
            Agent.enabled = true;
        }
    }

    public void AddForce(Vector3 force)
    {
        impact += force;
        if (Agent)
        {
            Agent.enabled = false;
        }
    }

    public void Jump(float force)
    {
        verticalVelocity += force;
    }

    public void Reset()
    {
        verticalVelocity = 0;
        impact = Vector3.zero;
    }
}
