using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public GlobalGameItemsData.Item Name;
    public int scoreAddAmount;
    public EnemyData(GlobalGameItemsData.Item name, int score)
    {
        Name = name;
        scoreAddAmount = score;
    }
}