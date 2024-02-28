using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private GlobalGameItems gameItems;
    private static PoolManager _instance;
    public static PoolManager Instance{
        get{
            return _instance;
        }
    }
    public Dictionary<string, ObjectPool<GameObject>> Pools = new Dictionary<string, ObjectPool<GameObject>>(StringComparer.OrdinalIgnoreCase);
    private List<PoolItem> poolItems = new List<PoolItem>();
    [Serializable]
    private struct PoolItem
    {
        public GlobalGameItemsData.Item name;
        public GameObject prefab;
        public int instancesToPreload;
        public int maxInstancesLimit;
    }

    void Awake()
    {
        _instance = (PoolManager)FindObjectOfType(typeof(PoolManager));
        InitializePooling();
    }

    private void InitializePooling()
    {
        foreach (var item in gameItems.spawnables)
        {
            PoolItem poolItem = new PoolItem();
            poolItem.instancesToPreload = item.itemCounts.y;
            poolItem.maxInstancesLimit = item.itemCounts.y + 2;
            poolItem.prefab = item.prefab;
            poolItem.name = item.item;
            poolItems.Add(poolItem);
        }
        Pools = new Dictionary<string, ObjectPool<GameObject>>(poolItems.Count);
        foreach (var item in poolItems)
        {
            Pools[item.name.ToString()] = new ObjectPool<GameObject>(() =>
                {
                    GameObject obj = Instantiate(item.prefab, transform.position, Quaternion.identity);
                    return obj;
                }, ActionOnGet, ActionOnRelease, ActionOnDestroy, false,
                item.instancesToPreload, item.maxInstancesLimit);
          //  Pools[item.name.ToString()].maxInstances = item.maxInstancesLimit;
        }

    }
    private void ActionOnRelease(GameObject obj)
    {
        obj.SetActive(false);
    }
    private void ActionOnDestroy(GameObject obj)
    {
        Destroy(obj);
    }
    private void ActionOnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    public Transform Spawn(string objName, Vector3 pos, Quaternion rot){
        GameObject obj = Pools[objName].Get();
        obj.name = objName;
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        return obj.transform;
    }
    public Transform Spawn(string objName, Vector3 pos,Transform parent)
    {
        GameObject obj = Pools[objName].Get();
        obj.transform.SetParent(parent);
        obj.transform.position = pos;
        obj.name = objName;
        return obj.transform;
    }
    public GameObject SpawnTemporary(string objName, Vector3 pos, Quaternion rot)
    {
        Transform obj = Spawn(objName, pos, rot);
        DespawnInSeconds(obj.gameObject);
        return obj.gameObject;
    }
    public GameObject SpawnTemporary(string objName, Vector3 pos,Transform parent)
    {
        Transform obj = Spawn(objName, pos,parent);
        DespawnInSeconds(obj.gameObject);
        return obj.gameObject;
    }
    public void Despawn(GameObject go){
        if (go != null)
        {
            Pools[go.name].Release(go);
        }
    }

    private void DespawnInSeconds(GameObject go)
    {
        Timer.Register(2, () =>
        {
            if (go != null)
            {
                Despawn(go);
            }
        });
    }
    
}