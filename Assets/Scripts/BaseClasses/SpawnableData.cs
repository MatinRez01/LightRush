using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEditor;
using UnityEngine;
[Serializable]
public class SpawnableData
{
    public GlobalGameItemsData.Item item;
    public GameObject prefab;
    [MinMaxSlider(0, 15)] public Vector2Int itemCounts;
    public bool automaticSpawn;
    [ConditionalField(nameof(automaticSpawn))] public bool ReadyToSpawnOnInit;
    public bool isEnemy;
    [ConditionalField(nameof(automaticSpawn)), MinMaxRange(0, 10)] public RangedFloat spawnCooldown;
    private bool readyToSpawn;

    public bool ReadyToSpawn
    {
        get
        {
            return readyToSpawn;
        }
    }
    [HideInInspector] public CooldownTimer timer;
    [HideInInspector] public int itemMinCount;
    [HideInInspector] public int itemMaxCount;
    public SpawnableData(GlobalGameItemsData.Item item, GameObject prefab, Vector2Int itemCounts, bool automaticSpawn, bool readyToSpawnOnInit, RangedFloat spawnCooldown)
    {
        this.item = item;
        this.prefab = prefab;
        this.itemCounts = itemCounts;
        itemMinCount = itemCounts.x;
        itemMaxCount = itemCounts.y;
        this.automaticSpawn = automaticSpawn;
        ReadyToSpawnOnInit = readyToSpawnOnInit;
        this.spawnCooldown = spawnCooldown;
        this.readyToSpawn = ReadyToSpawnOnInit;
        timer = new CooldownTimer(spawnCooldown.Max);
    }
    public SpawnableData(SpawnableData data)
    {
        this.item = data.item;
        this.prefab = data.prefab;
        this.itemCounts = data.itemCounts;
        itemMinCount = data.itemCounts.x;
        itemMaxCount = data.itemCounts.y;
        this.automaticSpawn = data.automaticSpawn;
        ReadyToSpawnOnInit = data.ReadyToSpawnOnInit;
        this.spawnCooldown = data.spawnCooldown;
        this.readyToSpawn = ReadyToSpawnOnInit;
        this.isEnemy = data.isEnemy;
        timer = new CooldownTimer(data.spawnCooldown.Max);
    }
    public void SwitchReadySpawnState(bool shouldSpawn)
    {
        readyToSpawn = shouldSpawn;
        if (readyToSpawn)
        {
            timer.Start();
        }
    }
    public void IncreaseMinCount()
    {
        if (itemMinCount == itemMaxCount) return;
        itemMinCount++;
    }
    public void DecreaseMinCount()
    {
        if (itemMinCount == 0) return;
        itemMinCount--;
    }
    public void UpdateTimer() 
    {
        timer.Update(Time.deltaTime);

    }

    public void StartTimer(float duration)
    {
        timer.Start(duration);
    }
}
