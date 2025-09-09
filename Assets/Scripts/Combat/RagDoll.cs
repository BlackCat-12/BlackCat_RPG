using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RagDoll : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    
    private Collider[] allColliders;
    private Rigidbody[]  allRigidbodies;

    private void Start()
    {
        allColliders = GetComponentsInChildren<Collider>(true);
        allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        ToggleRagDoll(false);
    }

    public void ToggleRagDoll(bool isRagDoll)
    {
        foreach (Collider collider in allColliders)
        {
            if (collider.gameObject.CompareTag("RagDoll"))
            {
                collider.enabled = isRagDoll;
            }
        }

        foreach (Rigidbody rigidbody in allRigidbodies)
        {
            if (rigidbody.gameObject.CompareTag("RagDoll"))
            {
                rigidbody.isKinematic = !isRagDoll;
                rigidbody.useGravity = isRagDoll;
            }
        }
        
        animator.enabled = !isRagDoll;
        characterController.enabled = !isRagDoll;
    }
}
