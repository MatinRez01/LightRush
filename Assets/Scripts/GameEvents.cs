using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    private static GameEvents _instance;
    public Action<EventData> OnEvent;
    public Action<GameObject, GlobalGameItemsData.Item> OnSpawnableItemCollect;
    public Action<string, int> OnPropertyChange;
    public Action<GameObject, EnemyData> OnEnemyDie;

    public static GameEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (GameEvents)FindObjectOfType(typeof(GameEvents));
            }
            return _instance;
        }
    }
    public static void TriggerEvent(EventData data)
    {
        Instance.OnEvent.Invoke(data);
    }

    public static void TriggerEvent(string stringData)
    {
        EventData eventData = new EventData(stringData, 0);
        Instance.OnEvent.Invoke(eventData);
    }
    public static void TriggerEvent(string stringData, int value)
    {
        EventData eventData = new EventData(stringData, value);
        Instance.OnEvent.Invoke(eventData);
    }
    private void Awake()
    {
        _instance = (GameEvents)FindObjectOfType(typeof(GameEvents));
        OnEnemyDie += EnemyDie;
        OnSpawnableItemCollect += ItemCollect;
    }
    void EnemyDie(GameObject go, EnemyData data)
    {
        EventData eventData = new EventData(data.Name.ToString(), 0);
        TriggerEvent(eventData);
    }
    void ItemCollect(GameObject go, GlobalGameItemsData.Item item)
    {
        EventData eventData = new EventData(item.ToString(), 0);
        TriggerEvent(eventData);
    }
    public class EventData
    {
        public string eventName;
        public float value;
        public EventData(string eventName, float value)
        {
            this.eventName = eventName;
            this.value = value;
        }
    }
}
