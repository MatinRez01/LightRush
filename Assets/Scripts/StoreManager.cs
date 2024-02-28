using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private StoreButton[] storeItems;
    [SerializeField] private PlayInvincibleAnimation bikesCounter;
    [SerializeField] private int currentBike;
    [SerializeField] private int currentTrail;
    [SerializeField] private StoreUI storeUI;
    private List<GameObject> bikes = new List<GameObject>();
    private List<GameObject> trails = new List<GameObject>();
    bool isUnlockingAnItem;
    void Awake()
    {
        foreach (var item in storeItems)
        {
            if (item.isTrail)
            {
                trails.Add(item.GameObject);
            }
            else
            {
                bikes.Add(item.GameObject);

            }
        }

        if (!PlayerPrefs.HasKey("currentBike") || !PlayerPrefs.HasKey("currentTrail"))
        {
            currentBike = 0;
            currentTrail = 0;
            PlayerPrefs.SetInt("currentBike", currentBike);
            PlayerPrefs.SetInt("currentTrail", currentTrail);
            SaveItem(bikes[currentBike]);
            SaveItem(trails[currentTrail]);
        }
        else
        {
            currentBike = PlayerPrefs.GetInt("currentBike");
            currentTrail = PlayerPrefs.GetInt("currentTrail");
        }
        SetupButtons();
        SelectCurrentBikeAndTrail();
    }
    private void Start()
    {
        foreach (var item in storeItems)
        {
            item.selectCallback += SelectItem;
        }
        foreach (var item in storeItems)
        {
            if (item.GameObject == bikes[currentBike])
            {
                item.Select();
            }
            else if (item.GameObject == trails[currentTrail])
            {
                item.Select();
            }
        }
    }
    private void SaveItem(GameObject item)
    {
        PlayerPrefs.SetInt(item.name, 1);
    }
    private void SetupButtons()
    {
        foreach (var item in storeItems)
        {
            if (PlayerPrefs.HasKey(item.GameObject.name))
            {
                item.unlocked = true;
            }
            else
            {
                item.unlocked = false;
            }
        }
        foreach (var item in storeItems)
        {
            if (item.isTrail)
            {
                if(item.GameObject == trails[currentTrail])
                {
                    item.Select();
                }
            }
            else
            {
                if (item.GameObject == bikes[currentBike])
                {
                    item.Select();
                }
            }
        }

    }
    private void SelectCurrentBikeAndTrail()
    {
        foreach (var bike in bikes)
        {
            bike.SetActive(false);
        }
        foreach (var trail in trails)
        {
            trail.SetActive(false);
        }
        bikes[currentBike].SetActive(true);
        trails[currentTrail].SetActive(true);
        bikesCounter.Setup();
    }
    private void SelectBike(int bikeIndex)
    {
        currentBike = bikeIndex;
        PlayerPrefs.SetInt("currentBike", currentBike);
        SelectCurrentBikeAndTrail();
    }
    private void SelectTrail(int trailIndex)
    {
        currentTrail = trailIndex;
        PlayerPrefs.SetInt("currentTrail", currentTrail);
        SelectCurrentBikeAndTrail();
    }
    public void SelectItem(int index)
    {
        if (!storeItems[index].unlocked) return;
        bool isItBike = !storeItems[index].isTrail;
        if (isItBike)
        {
            int i = bikes.IndexOf(storeItems[index].GameObject);
            SelectBike(i);
            foreach (var item in storeItems)
            {
                if (!item.isTrail)
                {
                    item.Deselect();
                }
            }
        }
        else
        {
            foreach (var item in storeItems)
            {
                if (item.isTrail)
                {
                    item.Deselect();
                }
            }
            int i = trails.IndexOf(storeItems[index].GameObject);
            SelectTrail(i);
        }
        storeItems[index].Select();


    }
    public void UnlockAnItem()
    {
        if (isUnlockingAnItem) return;
        if (gameManager.TotalCoin < 100) return;
        isUnlockingAnItem = true;
        GameEvents.TriggerEvent("UnlockItem");
        GameManager.Instance.DecreaseCoin(100);
        storeUI.LockScreen();
        float rnd = Random.Range(0, 10);
        bool isItBike = rnd > 6;
        int rndIndex;
        List<StoreButton> locked = new List<StoreButton>();
        locked = lockedList(isItBike);
        if (locked.Count == 0)
        {
            isItBike = !isItBike;
            locked = lockedList(isItBike);
        }
        rndIndex = Random.Range(0, locked.Count);
        storeUI.UnlockAnItem(lockedList(), locked[rndIndex], () =>
        {
            locked[rndIndex].UnlockItem();
            if (isItBike)
            {
                SelectBike(bikes.IndexOf(locked[rndIndex].GameObject));
            }
            else
            {
                SelectTrail(trails.IndexOf(locked[rndIndex].GameObject));
            }
            SaveItem(locked[rndIndex].GameObject);
            isUnlockingAnItem = false;
        });
    }
    private List<StoreButton> lockedList(bool isItBike)
    {
        List<StoreButton> locked = new List<StoreButton>();
        if (isItBike)
        {
            foreach (var item in storeItems)
            {
                if (!item.isTrail)
                {
                    if (!item.unlocked)
                    {
                        locked.Add(item);
                    }
                }
            }
        }
        else
        {
            foreach (var item in storeItems)
            {
                if (item.isTrail)
                {
                    if (!item.unlocked)
                    {
                        locked.Add(item);
                    }
                }
            }
        }
        return locked;
    }
    private List<StoreButton> lockedList()
    {
        List<StoreButton> locked = new List<StoreButton>();
        foreach (var item in storeItems)
        {
            if (!item.unlocked)
            {
                locked.Add(item);
            }
        }
        return locked;
    }
}
