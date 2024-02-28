using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILightTrailManager : MonoBehaviour
{
    [SerializeField] private LightTrailGeneration lt;
    [SerializeField] private float enableLightDelay = 5f;
   

    public void OnVisible()
    {
        if (lt != null)
        {
            if (!lt.Active)
            {
                EnableLightTrail();
            }
        }
    }
    private void EnableLightTrail()
    {
        Timer.Register(enableLightDelay, () =>
        {
            lt.ChargeLightTrail(2);
            lt.SwitchTrailState();
        });
    }
    private void OnDisable()
    {
        if (lt.Active)
        {
           lt.SwitchTrailState();
        }
    }
}
