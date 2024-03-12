using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(200)]
public class trailItem : MonoBehaviour
{
    [SerializeField,ColorUsage(true, true)] Color color;
    [SerializeField] PlayerTrailManager trailManager;
    private void OnEnable()
    {
        trailManager.SetTrailColor(color);
    }
}
