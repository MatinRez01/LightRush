using MyBox;
using System;
using UnityEngine;
[DefaultExecutionOrder(100)]
public class FPSAnalyzer : MonoBehaviour
{
    [SerializeField] private int goodFps = 45;
    float avg = 0F;
    float ActualAvgFps;
    bool shouldAnalyze = false;
    bool alreadyInPerfomanceMode;
    #region Constructers
    private bool AnalyzedPerfomance
    {
        get
        {
            if (PlayerPrefs.HasKey("AnalyzedPerfomance"))
            {
                int i = PlayerPrefs.GetInt("AnalyzedPerfomance");
                return i > 0;
            }
            else
            {
                return false;
            }
        }
        set
        {

            PlayerPrefs.SetInt("AnalyzedPerfomance", value ? 1 : 0);
        }
    }
    private bool perfomanceModePreferred
    {
        get
        {
            if (PlayerPrefs.HasKey("perfomanceModePreferred"))
            {
                int i = PlayerPrefs.GetInt("perfomanceModePreferred");
                return i > 0;
            }
            else
            {
                return false;
            }
        }
        set
        {
            PlayerPrefs.SetInt("perfomanceModePreferred", value ? 1 : 0);
        }
    }
    public bool ChangedSettingsOnce
    {
        get
        {
            if (PlayerPrefs.HasKey("ChangedSettingsOnce"))
            {
                int i = PlayerPrefs.GetInt("ChangedSettingsOnce");
                return i > 0;
            }
            else
            {
                return false;
            }
        }
        set
        {

            PlayerPrefs.SetInt("ChangedSettingsOnce", value ? 1 : 0);
        }
    }
    #endregion
    #region Unity Callbacks
    void Start()
    {
        if (alreadyInPerfomanceMode) return;
        if (AnalyzedPerfomance)
        {
            if (perfomanceModePreferred && !ChangedSettingsOnce)
            {
                ChangeSettings();
            }
            shouldAnalyze = false;
            return;
        }
        GameEvents.Instance.OnEvent += OnEventTriggered;
    }

    void Update()
    {
        if (!shouldAnalyze) return;
        CheckPerfomance();
    }
    #endregion
    #region internal
    void OnEventTriggered(GameEvents.EventData eventData)
    {

        if ("GameEnd" == eventData.eventName)
        {
            CheckPerfomanceResultStatus();
        }else if("GameStart" == eventData.eventName)
        {
            shouldAnalyze = true;
        }

    }
    void CheckPerfomanceResultStatus()
    {
        StopCheckingForPerfomance();
        bool changeToPerfomanceMode = !(ActualAvgFps >= goodFps);
        SaveSettings(changeToPerfomanceMode);
    }
    void SaveSettings(bool perfomancePreferred)
    {
        perfomanceModePreferred = perfomancePreferred;
        AnalyzedPerfomance = true;
    }
    void StopCheckingForPerfomance()
    {
        shouldAnalyze = false;
    }
    void CheckPerfomance()
    {
        avg += ((Time.deltaTime / Time.timeScale) - avg) * 0.03f;
        ActualAvgFps = (1F / avg);
    }
    void ChangeSettings()
    {
        ChangedSettingsOnce = true;
        OnPerfomancePreferred?.Invoke();
    }
    #endregion
    #region public
    public Action OnPerfomancePreferred;
    /// <param name="inPerfomanceMode">If game is already in perfomance mode there is no need for analyzing perfomance</param>
    public void Setup(bool inPerfomanceMode)
    {
        alreadyInPerfomanceMode = inPerfomanceMode;
    }
    #endregion
    
}
