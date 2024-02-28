using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public abstract class SpawnableItem : MonoBehaviour
{
    [SerializeField] private GlobalGameItemsData.Item itemName;
    [SerializeField] private float maxTimeOnGround = 3f;
    [SerializeField] private float maxDistanceFromPlayer = 100f;
    [SerializeField] private GlobalGameItemsData.Item collectParticle;
    
    private float timeOnGround = 0;
    private bool enoughTimeOnGround = false;
    private bool visible = false;
    private void Update()
    {
        if(!enoughTimeOnGround)
        {
            timeOnGround += Time.deltaTime;
            enoughTimeOnGround = timeOnGround >= maxTimeOnGround;
        }
        if(Time.frameCount % 10 == 0)
        {
            if (ReadyToDespawn)
            {
                Spawner.Instance.Despawn(gameObject);
            }
        }
    }

    private void OnEnable()
    {
        Reset();
        FloatingOrigin.OriginShiftEventChannel += OriginShift;

    }
    private void OnDisable()
    {
        FloatingOrigin.OriginShiftEventChannel -= OriginShift;

    }
    private void OriginShift(Vector3 offset)
    {
        transform.position += offset;
    }
    private void Reset()
    {
        timeOnGround = 0;
        enoughTimeOnGround = false;
        visible = false;
    }
    protected void OnBecameVisible()
    {
        visible = true;
    }
    protected void OnBecameInvisible()
    {
        visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnCollected(other.gameObject);
        }
    }

    protected virtual void OnCollected(GameObject player)
    {
        Spawner.Instance.SpawnFx(collectParticle.ToString(), transform.position, Quaternion.identity, gameObject);
        GameEvents.Instance.OnSpawnableItemCollect.Invoke(gameObject, itemName);
    }
    private bool ReadyToDespawn
    {
        get
        {
            return enoughTimeOnGround & Vector3.Distance(PlayerCar.PreviousPosition, transform.position) >= maxDistanceFromPlayer & !visible;
        }
    } 
}
