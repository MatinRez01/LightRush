using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/GameItems"), Serializable]
public class GlobalGameItems : ScriptableObject
{
    public SpawnableData[] spawnables;


    public Dictionary<GlobalGameItemsData.Item, SpawnableData> GetAutoSpawnItemsList()
    {
         Dictionary<GlobalGameItemsData.Item, SpawnableData> autoSpawnList = new Dictionary<GlobalGameItemsData.Item, SpawnableData>(GetAutoSpawnItemsCount());
        foreach (var item in spawnables)
        {
            if (!item.automaticSpawn) continue;
            var spawnable = new SpawnableData(item);
            autoSpawnList[item.item] = spawnable;
        }
        return autoSpawnList;
    }
    private int GetAutoSpawnItemsCount()
    {
        int i = 0;
        foreach (var item in spawnables)
        {
            if (item.automaticSpawn)
            {
                i++;
            }
        }
        return i;
    }

}
[Serializable, CreateAssetMenu(menuName = "Custom/GameItemsData")]
public class GlobalGameItemsData : ScriptableObject
{
    public enum Item
    {
        Enemy,
        AdvancedEnemy,
        Coin,
        LightTrailPowerup,
        ExplosionRedFx,
        ExplosionBlueFx,
        ImpactFx,
        CoinFx,
        LightTrailPowerupFx,
        LightTrailEnemy,
        ErrorText,
        AddText

    }

}