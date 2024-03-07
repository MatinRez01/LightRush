using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Setup")] 
    [SerializeField] private PlayerCar player;
    [SerializeField] private GameObject  enemySpawnPointsParent;
    [SerializeField] private GameObject  coinSpawnPointsParent;
    [SerializeField] private GameObject powerUpSpawnPointsParent;
    [SerializeField] private GlobalGameItems gameItems;
    public static Spawner Instance
    {
        get
        {
            return _instance;
        }
    }

    private static Spawner _instance;
    private List<Transform> enemySpawnPoints = new List<Transform>();
    private List<Transform> coinSpawnPoints = new List<Transform>();
    private List<Transform> powerUpSpawnPoints = new List<Transform>();
    private GameManager gameManager;
    private Dictionary<GlobalGameItemsData.Item, SpawnableData> autoSpawnItemsList = new Dictionary<GlobalGameItemsData.Item, SpawnableData>();
    bool gameStarted;
    private void Start()
    {
             _instance = (Spawner)FindObjectOfType(typeof(Spawner));
             gameManager = GetComponent<GameManager>();
             autoSpawnItemsList = gameItems.GetAutoSpawnItemsList();
             FillInSpawnPoints();

    }
    private void FillInSpawnPoints()
    {
        for (int i = 0; i < enemySpawnPointsParent.transform.childCount; i++)
        {
            enemySpawnPoints.Add(enemySpawnPointsParent.transform.GetChild(i));
        }
        for (int i = 0; i < coinSpawnPointsParent.transform.childCount; i++)
        {
            coinSpawnPoints.Add(coinSpawnPointsParent.transform.GetChild(i));
        }
        for (int i = 0; i < powerUpSpawnPointsParent.transform.childCount; i++)
        {
            powerUpSpawnPoints.Add(powerUpSpawnPointsParent.transform.GetChild(i));
        }
    }

    public void SpawningStart()
    {

        foreach (var item in autoSpawnItemsList)
        {
            if (item.Value.ReadyToSpawn)
            {
                item.Value.StartTimer(item.Value.spawnCooldown.Max);
            }
        }
        for (int i = 0; i < 2; i++)
        {
            PoolManager.Instance.Spawn(GlobalGameItemsData.Item.Enemy.ToString(), RandomSpawnPoint(enemySpawnPoints), Quaternion.identity);
        }
        gameStarted = true;
    }

   
    
    private void Update()
    {
        if (gameStarted)
        {
            foreach (var item in autoSpawnItemsList)
            {
                if (item.Value.ReadyToSpawn)
                {
                    if (item.Value.isEnemy)
                    {
                        float cooldown = item.Value.spawnCooldown.Max;
                        cooldown -= Time.deltaTime / 16;
                        cooldown = Mathf.Clamp(cooldown, item.Value.spawnCooldown.Min, item.Value.spawnCooldown.Max);
                        item.Value.spawnCooldown.Max = cooldown;
                    }
                    SpawnItem(item.Value.item, item.Value.timer,
                    item.Value.isEnemy ? RandomSpawnPoint(enemySpawnPoints) : RandomSpawnPoint(coinSpawnPoints),
                    0.5f, item.Value.isEnemy? item.Value.itemMinCount : item.Value.itemMaxCount);
              
                }
                
            }

        }
    }

 
    private void SpawnItem(GlobalGameItemsData.Item itemName, CooldownTimer itemTimer, Vector3 spawnPoint, float itemDropChance, int maxDropCount)
    {
        if (itemTimer.IsCompleted)
        {
            if (PoolManager.Instance.Pools[itemName.ToString()].CountActive >= maxDropCount)
            {
                return;
            }
            if (ShouldSpawnByChance(itemDropChance))
            {
                PoolManager.Instance.Spawn(itemName.ToString(), spawnPoint, Quaternion.identity);
                float rndSpawnTime = UnityEngine.Random.Range(autoSpawnItemsList[itemName].spawnCooldown.Min, 
                    autoSpawnItemsList[itemName].spawnCooldown.Max);
                itemTimer.Start(rndSpawnTime);

            }
        }
        else
        {
            itemTimer.Update(Time.deltaTime);
        }
    }
    public GameObject SpawnFx(string objName, Vector3 pos, Quaternion rot)
    {
       var obj = PoolManager.Instance.SpawnTemporary(objName, pos, rot);
       return obj;
    }
    public GameObject SpawnUI(string objName, Transform parent)
    {
        var obj = PoolManager.Instance.Spawn(objName, parent.transform.position, parent);
        return obj.gameObject;
    }
    public void Despawn(GameObject go)
    {
        PoolManager.Instance.Despawn(go);
    }
    public void SpawnFx(string objName, Vector3 pos, Quaternion rot, GameObject go)
    {
        PoolManager.Instance.SpawnTemporary(objName, pos, rot);
        PoolManager.Instance.Despawn(go);
    }
    public void SpawnImpactFx(Vector3 position, Quaternion rot, Color color)
    {
        GameObject fx = SpawnFx(GlobalGameItemsData.Item.ImpactFx.ToString(), position, rot );
      //  fx.GetComponent<ParticleSetup>().color = color;
    }
    public void ResetTrailPowerUpTimer()
    {
        ResetTimer(autoSpawnItemsList[GlobalGameItemsData.Item.LightTrailPowerup].timer);
    }
    public void ActivateSpawningOf(GlobalGameItemsData.Item item)
    {
        autoSpawnItemsList[item].SwitchReadySpawnState(true);
    }

    public void IncreaseMinimumCountsOf(string item)
    {
        autoSpawnItemsList[(GlobalGameItemsData.Item)
            Enum.Parse(typeof(GlobalGameItemsData.Item), item)].IncreaseMinCount();
        SpawnableData data = autoSpawnItemsList[(GlobalGameItemsData.Item)
            Enum.Parse(typeof(GlobalGameItemsData.Item), item)];

    }
    public void DecreaseMinimumCountsOf(string item)
    {
        DeactivateSpawningOf(item);
        autoSpawnItemsList[(GlobalGameItemsData.Item)
    Enum.Parse(typeof(GlobalGameItemsData.Item), item)].DecreaseMinCount();
        ActivateSpawningOf(item);
        SpawnableData data = autoSpawnItemsList[(GlobalGameItemsData.Item)
    Enum.Parse(typeof(GlobalGameItemsData.Item), item)];

    }
    public void ActivateSpawningOf(string item)
    {
        autoSpawnItemsList[(GlobalGameItemsData.Item)Enum.Parse(typeof(GlobalGameItemsData.Item), item)].SwitchReadySpawnState(true);
    }
    public void DeactivateSpawningOf(string item)
    {
        autoSpawnItemsList[(GlobalGameItemsData.Item)Enum.Parse(typeof(GlobalGameItemsData.Item), item)].SwitchReadySpawnState(false);
    }
    public void DeactivateSpawningOf(GlobalGameItemsData.Item item)
    {
        autoSpawnItemsList[item].SwitchReadySpawnState(false);
    }
    public void ResetTimer(CooldownTimer timer)
    {
        timer.Start();
    }
    private Vector3 RandomSpawnPoint(List<Transform> spawnPoint)
    {
        int randomNumber = UnityEngine.Random.Range(0, spawnPoint.Count);
        return spawnPoint[randomNumber].position;
    }

    private bool ShouldSpawnByChance(float chance)
    {
        var rnd = UnityEngine.Random.value;
        return rnd <= chance;
    }
    
}
