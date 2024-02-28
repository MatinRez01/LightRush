using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Car : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] protected Transform carBody;
    [SerializeField] protected float currentSpeed;
    [SerializeField] protected float currentTurnSpeed;
    [SerializeField] protected float tiltMaxAngle;
    [SerializeField] private float maxImpactForce;
    [SerializeField] protected float tiltSmooth;
    [SerializeField] private Color fxColor;

    protected InputManager inputManager;

    private Rigidbody rb;
    private Quaternion tilt;
    private Vector3 force;
    private GameObject lastImpactGo;
    protected GameManager gameManager;



    protected virtual void OnEnable()
    {
        inputManager = GetComponent<InputManager>();
        if(rb==null)
        {
           rb = GetComponent<Rigidbody>();
        }
        FloatingOrigin.OriginShiftEventChannel += OriginShift;


    }


    private void OnDisable()
    {
        FloatingOrigin.OriginShiftEventChannel -= OriginShift;
    }

    private void OriginShift(Vector3 offset)
    {
        rb.position += offset;
    }
    protected virtual void FixedUpdate()
    {
        Move();
        RotateAndTilt();
    }
    private void Move()
    {
        force = transform.forward * (currentSpeed * Time.deltaTime * 100);
        rb.velocity = force;
    }


    private void RotateAndTilt()
    {
        transform.Rotate(0.0f, inputManager.GetSteerDirection() * currentTurnSpeed * Time.deltaTime, 0);
        tilt = Quaternion.AngleAxis(inputManager.GetSteerDirection() * -tiltMaxAngle, Vector3.forward);
        carBody.localRotation = Quaternion.Slerp(carBody.localRotation, tilt, tiltSmooth * Time.deltaTime);
    }

    

    protected virtual void CalculateCollision(Collision other, bool oneHitKill)
    {
        if (ValidateDamage(other.relativeVelocity))
        {
            if (oneHitKill)
            {
                Die();
            }
            else
            {
                TakeDamage();
            }

        }
        else
        {
            if (lastImpactGo != other.gameObject)
            {
                SpawnImpactFx(other);
            }
        }
    }
    protected abstract void TakeDamage();
    protected abstract void Die();

    protected bool ValidateDamage(Vector3 relativeVelocity)
    {
        if (relativeVelocity.magnitude > maxImpactForce)
        {
            return true;
        }
        else if (relativeVelocity.magnitude > 10)
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    protected virtual void SpawnImpactFx(Collision other)
    {
        var estimatedNormal = Vector3.zero;
        for (int i = 0; i < other.contactCount; i++)
        {
            estimatedNormal += other.GetContact(i).normal;
        }
        estimatedNormal /= other.contactCount;
        var normal = Quaternion.LookRotation(estimatedNormal, Vector3.up);
        Spawner.Instance.SpawnImpactFx(other.contacts[0].point, normal, fxColor);
        lastImpactGo = other.gameObject;
    }



}
