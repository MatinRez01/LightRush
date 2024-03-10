using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.RaycastsManager;

public class AdvancedAIInput : InputManager
{
    [Header("Persue")]
    [SerializeField] private float prediction = 1;
    [Header("Obstacle Avoidance")]
    [SerializeField] private float avoidDistance = 1f;  
    [SerializeField] private float lookahead = 2f;  
    [SerializeField] private float sideViewAngle = 45f;
    [SerializeField] private int numberOfRays;
    [SerializeField] private LayerMask layerMask;
    private Vector3 directionToGo;
    private Vector3 cross;
    private readonly PlayerCar player;
    RaycastHit hit;
    Transform target;
    private void OnEnable()
    {
        target = GameObject.FindWithTag("Player").transform;
    }
    public override float GetSteerDirection()
    {
        if (FrontObscured())
        {
            return ObstacleAvoid();
        }
        else
        {
            return Persue();
        }
    }
    private float Persue()
    {
        directionToGo = (target.position + (target.forward * prediction) - transform.position).normalized;
        cross = Vector3.Cross(transform.forward, directionToGo);
        return cross.y;
    }
    bool FrontObscured()
    {
        Vector3 origin = transform.position + (transform.up * 2);
        if (RaycastsManager.RaycastsScatteringRotOneHit(origin, transform.forward, sideViewAngle, numberOfRays, lookahead, layerMask, out hit, true))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected float ObstacleAvoid()
    {
        
        Vector3 steering;
        float steeringVal;
        Vector3 cross;
        Vector3 target = hit.point + (hit.normal * avoidDistance); 
        steering = target - transform.position; 
        steering.Normalize();
        cross = Vector3.Cross(transform.forward, steering);
        steeringVal = Mathf.Clamp(cross.y * 2, -1, 1) ;
        return steeringVal;
    }
}

