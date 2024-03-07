using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPVisualControl : MonoBehaviour
{
    List<playHealthAnimation> childs = new List<playHealthAnimation>(3);
    [SerializeField] Transform container;

    private int health
    {
        get
        {
            return childs.Count;
        }
    }
    private void OnEnable()
    {
        for (var i = 0; i < container.transform.childCount; i++)
        {
            childs.Add(container.GetChild(i).GetComponent<playHealthAnimation>());
        }
        PlayerCar.OnPlayerDamage += HealthDecrease;
    }
    private void OnDisable()
    {
        PlayerCar.OnPlayerDamage -= HealthDecrease;

    }
    private void HealthDecrease()
    {
        if(health > 0){
            childs[childs.Count - 1].PlayAnimationAndDisable();
            childs[childs.Count - 1].alphaFlicker.StartFlickering();
            
            childs.RemoveAt(childs.Count - 1);
        }
        
    }
}
