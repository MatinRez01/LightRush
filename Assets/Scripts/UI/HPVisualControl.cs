using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPVisualControl : MonoBehaviour
{
    List<playOneShotAnimation> childs = new List<playOneShotAnimation>(3);
    [SerializeField] Image glowImage;
    [SerializeField] Transform container;

    float startGlowAlpha;
    float eachDecreaseAlpha;
    private int health
    {
        get
        {
            return childs.Count;
        }
    }
    private void OnEnable()
    {
        startGlowAlpha = glowImage.color.a;
        for (var i = 0; i < container.transform.childCount; i++)
        {
            childs.Add(container.GetChild(i).GetComponent<playOneShotAnimation>());
        }
        eachDecreaseAlpha = (startGlowAlpha) / (2);
        
        PlayerCar.OnPlayerDamage += HealthDecrease;
    }

    private void HealthDecrease()
    {
        if(health > 0){
            childs[childs.Count - 1].PlayAnimation();
            childs.RemoveAt(childs.Count - 1);
            Debug.Log(glowImage.color.a - eachDecreaseAlpha);
            glowImage.DOFade(glowImage.color.a - eachDecreaseAlpha, 1);
        }
        
    }
}
