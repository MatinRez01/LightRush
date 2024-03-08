using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateApp : MonoBehaviour
{
    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(RateAppMethod);
    }

    void RateAppMethod()
    {
        UMM.MarketIntents.OpenComments();
    }
}
