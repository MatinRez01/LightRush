using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName = "Custom/StoreDatabase"), Serializable]

public class StoreDatabase : ScriptableObject
{
    public StoreItem[] items;

}
[Serializable]
public class StoreItem
{
    public GameObject prefab;
    public bool isTrail;
    public bool unlocked;
    public Sprite itemImage;
}
