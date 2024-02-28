using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Custom/GameAchievementsDatabase")]
public class AchievementsDatabase : ScriptableObject
{
    public Achievement[] Achievements;

}
