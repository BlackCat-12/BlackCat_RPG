using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup cineTargetGroup;
    
    private Camera mainCam;
    public List<Target> targets = new List<Target>();
    public Target currTarget { get; private set; }

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Target target))
        {
            targets.Add(target);
            target.OnDestroyed += RemoveTarget;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Target target))
        {
            targets.Remove(target);
            RemoveTarget(target);
        }
    }

    public bool SelectTarget()
    {
        if (targets.Count == 0) return false;
        float minTargetDistance = Mathf.Infinity;
        Target minTarget = null;
        Vector2 center = new Vector2(0.5f, 0.5f);
        foreach (var target in targets)  // 选择最近且在屏幕中的target
        {
            Vector2 viewPos = mainCam.WorldToViewportPoint(target.transform.position);
            // if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1) { continue; }
            if(!target.GetComponentInChildren<Renderer>().isVisible){continue;}
            
            Vector2 toCenter = viewPos - center;
            if (toCenter.sqrMagnitude < minTargetDistance)
            {
                minTarget = target;
                minTargetDistance = toCenter.sqrMagnitude;
            }
        }

        if (!minTarget)
        {
            return false;
        }
        currTarget = minTarget;
        Debug.Log(currTarget.name);
        cineTargetGroup.AddMember(currTarget.transform, 1f, 2f);
        return true;
    }

    public void Cancel()
    {
        if (currTarget != null)
        {
            cineTargetGroup.RemoveMember(currTarget.transform);
        }
        currTarget = null; 
    }

    private void RemoveTarget(Target target)
    {
        if (currTarget == target)
        {
            cineTargetGroup.RemoveMember(currTarget.transform);
            currTarget = null;
        }
        target.OnDestroyed -= RemoveTarget;
        targets.Remove(target);
    }
}
