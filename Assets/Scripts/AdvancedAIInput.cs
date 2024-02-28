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
    private PlayerCar player;
    private Rigidbody rb;
    RaycastHit hit;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerCar>();
        rb = GetComponent<Rigidbody>();
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
        directionToGo = (player.transform.position + (player.transform.forward * prediction) - transform.position).normalized;
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
        /*  Vector3[] rayVector = new Vector3[3];
          rayVector[0] = rb.velocity;
          rayVector[0].Normalize(); 
          rayVector[0] *= lookahead;
          float rayOrientation = Mathf.Atan2(rb.velocity.x, rb.velocity.z);
          float rightRayOrientation = rayOrientation + (sideViewAngle * Mathf.Deg2Rad);
          float leftRayOrientation = rayOrientation - (sideViewAngle * Mathf.Deg2Rad);
          rayVector[1] = new Vector3(Mathf.Cos(rightRayOrientation), 0, Mathf.Sin(rightRayOrientation));
          rayVector[1].Normalize();
          rayVector[1] *= lookahead;
          rayVector[2] = new Vector3(Mathf.Cos(leftRayOrientation), 0, Mathf.Sin(leftRayOrientation));
          rayVector[2].Normalize();
          rayVector[2] *= lookahead;
          int hitCount = 0;
          for (int i = 0; i < rayVector.Length; i++)
          {
              if (RaycastsManager.Raycast(transform.position, rayVector[i], out hit, lookahead, true))
              {
                  hitCount++;
              }
          }
          bool didHit = hitCount > 0;
          return didHit;*/
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

