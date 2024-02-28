using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Unity.VisualScripting;
using Tools;

public class Cop : Car
{
    [SerializeField] private TrailRenderer trail;
    [SerializeField, MinMaxSlider(0, 150)] private Vector2 minMaxSpeed;
    [SerializeField, MinMaxSlider(0,360)] private Vector2 minmaxTurnSpeed;
    [SerializeField] private float accelerationDuration;
    [SerializeField] private float decelerationDuration;
    [SerializeField] private float minCopLineOfSite;
    [SerializeField] private EnemyData enemyData;

    private Coroutine accelerateCoroutine;
    private Coroutine decelerateCoroutine;


    [HideInInspector] public  Transform  player;


    private float dot;
    private Transform mTransform;
    private float trailTime;
    GameEvents gameEvents;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        mTransform = transform;
        trailTime = trail.time;
        gameEvents = GameEvents.Instance;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        currentTurnSpeed = UnityEngine.Random.Range(minmaxTurnSpeed.x, minmaxTurnSpeed.y);
        currentSpeed = UnityEngine.Random.Range(minMaxSpeed.x, minMaxSpeed.y);
        Timer.Register(.5f, () =>
        {
            if (trail != null)
            {
                trail.time = trailTime;
            }
        });
    }
    private void OnDisable()
    {
        if (trail != null)
        {
            trail.time = 0;
        }
    
    }

    protected void Update()
    {
        dot = Vector3.Dot(transform.forward, (player.position - mTransform.position).normalized);

        if (dot >= minCopLineOfSite)
        {
            if (accelerateCoroutine != null)
            {
                return;
            }
            if (decelerateCoroutine != null)
            {
                StopCoroutine(decelerateCoroutine);
                decelerateCoroutine = null;
            }
            accelerateCoroutine = StartCoroutine(_AccelerateCoroutine(minMaxSpeed.y, accelerationDuration));
        }
        else if (dot < 0)
        {
            if (decelerateCoroutine != null)
            {
                return;
            }
            if (accelerateCoroutine != null)
            {
                StopCoroutine(accelerateCoroutine);
                accelerateCoroutine = null;

            }
            decelerateCoroutine = StartCoroutine(_DecelerateCoroutine(minMaxSpeed.x, decelerationDuration));
        }
    }
    protected override void FixedUpdate()
    {
        if (player != null)
        {
            base.FixedUpdate();
        }
    }
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Enemy") )
        {
            CalculateCollision(other, true);
        }else if (other.gameObject.CompareTag("LightTrail"))
        {
            Die();
            GameEvents.TriggerEvent("LightTrailKill");
        }

    }
    IEnumerator _AccelerateCoroutine(float targetSpeed, float duration)
    {
        float time = 0;
        float previousSpeed = currentSpeed;
        while (duration >= time)
        {
            currentSpeed = Mathf.Lerp(previousSpeed, targetSpeed, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        accelerateCoroutine = null;
    }
    IEnumerator _DecelerateCoroutine(float targetSpeed, float duration)
    {
        float time = 0;
        float previousSpeed = currentSpeed;
        while (duration >= time)
        {
            currentSpeed = Mathf.Lerp(previousSpeed, targetSpeed, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        decelerateCoroutine = null;
    }

    protected override void TakeDamage()
    {
        Die();
    }
    
    protected override void Die()
    {
        gameEvents.OnEnemyDie(gameObject, enemyData);
    }
  
 
}