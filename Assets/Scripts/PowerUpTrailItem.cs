using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTrailItem : SpawnableItem
{
    protected override void OnCollected(GameObject player)
    {
        base.OnCollected(player);
        player.GetComponent<PlayerCar>().lt.ChargeLightTrail();
        GameEvents.TriggerEvent(base.name);
    }
}
