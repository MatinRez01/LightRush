using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BecameVisibleEnemy : MonoBehaviour
{
    [SerializeField] private AILightTrailManager aiLt;
    private void OnBecameVisible()
    {
        aiLt.OnVisible();
    }
}
