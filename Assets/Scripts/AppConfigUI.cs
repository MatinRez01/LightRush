using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppConfigUI : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Toggle soundToggle;
    [SerializeField] Toggle vibrateToggle;
    [SerializeField] Toggle batterySavingToggle;

    AppConfig appConfig;
    private void OnEnable()
    {
        appConfig = GetComponent<AppConfig>();
        soundToggle.isOn = !appConfig.SoundOn;
        vibrateToggle.isOn = !appConfig.Vibrate;
        batterySavingToggle.isOn = appConfig.BatterySaving;
    }
}
