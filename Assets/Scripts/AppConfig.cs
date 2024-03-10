using MoreMountains.NiceVibrations;
using Tools;
using UnityEngine;

public class AppConfig : MonoBehaviour
{
    [MinMaxSlider(0.6f, 1), SerializeField] Vector2 resolutionMinAndMax;
    [SerializeField] public int defaultResolution;
    [SerializeField] private PlanarReflection pl;
    [SerializeField] private MobilePostProcessing mobilePostProcessing;
    [SerializeField] private FastBloom fastBloom;
    [SerializeField] private SfxManager sfxManager;
    [SerializeField] private FPSAnalyzer fpsAnalyzer;
    [SerializeField] private bool debugMode;

    float resolutionPercentage;
    private string fps_string;
    private string resolution_Text;
    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    const string display = "{0} FPS";
    private bool soundOn = true;
    private bool vibrate = true;
    private bool batterySaving = false;
    public bool BatterySaving
    {
        get
        {
            return batterySaving;
        }
        set
        {
            batterySaving = value;
            PlayerPrefs.SetInt("BatterySaving", batterySaving ? 1 : 0);
        }
    }
    public bool Vibrate
    {
        get
        {
            return vibrate;
        }
        set
        {
            vibrate = value;
            PlayerPrefs.SetInt("Vibrate", vibrate ? 1 : 0);
        }
    }
    public bool SoundOn
    {
        get
        {
            return soundOn;
        }
        set
        {
            soundOn = value;
            PlayerPrefs.SetInt("SoundOn", soundOn ? 1 : 0);
        }
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        
        SetResolution();
        if (debugMode)
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        }
        sfxManager.SoundMute(!SoundOn);
        MMVibrationManager.SetHapticsActive(Vibrate);
        fpsAnalyzer.Setup(BatterySaving);
        fpsAnalyzer.OnPerfomancePreferred += () => 
        {
            BatterySavingChange();
            NotifyManager.Instance.ShowTip("ACTIVATED PERFOMANCE MODE");
        } ;
    }
    private void OnEnable()
    {
        Setup();
        
    }
    void Setup()
    {
        if (PlayerPrefs.HasKey("BatterySaving"))
        {
            BatterySaving = PlayerPrefs.GetInt("BatterySaving") > 0;
            
        }
        else
        {
            BatterySaving = false;
        }

        if (PlayerPrefs.HasKey("Vibrate"))
        {
            Vibrate = PlayerPrefs.GetInt("Vibrate") > 0;
        }
        else
        {
            Vibrate = true;
            PlayerPrefs.SetInt("Vibrate", Vibrate ? 1 : 0);
        }

        if (PlayerPrefs.HasKey("SoundOn"))
        {
            SoundOn = PlayerPrefs.GetInt("SoundOn") > 0;
        }
        else
        {
            SoundOn = true;
            PlayerPrefs.SetInt("SoundOn", SoundOn ? 1 : 0);
        }
        
    }
    public void SoundSettingChange()
    {
        SoundOn = !SoundOn;
        sfxManager.SoundMute(!SoundOn);
    }
    private void SetResolution()
    {
        if (BatterySaving)
        {
            resolutionPercentage = resolutionMinAndMax.x;
        }
        else
        {
            resolutionPercentage = resolutionMinAndMax.y;
        }

         float aspect = Screen.height / (float)Screen.width;
          Vector2 finalResolution = new Vector2(defaultResolution / aspect, defaultResolution) * resolutionPercentage;
        //Vector2 finalResolution = new Vector2(Screen.width, Screen.height) * resolutionPercentage;

        Screen.SetResolution((int)finalResolution.x, (int)finalResolution.y, true);
    }
    public void BatterySavingChange()
    {
        BatterySaving = !BatterySaving;
        SetResolution();
        if (BatterySaving) 
        {
            pl.ReflectionTexResolution = 128;
            mobilePostProcessing.enabled = false;
            //Application.targetFrameRate = 30;
            fastBloom.enabled = false;
        }
        else
        {
            pl.ReflectionTexResolution = 512;
            mobilePostProcessing.enabled = true;
       //     Application.targetFrameRate = 60;
            fastBloom.enabled = true;
        }
    }
    public void VibrateSettingChange()
    {
        vibrate = !vibrate;
        if(vibrate)
        {
            MMVibrationManager.Vibrate();
        }
        MMVibrationManager.SetHapticsActive(vibrate);
    }
    private void Update()
    {
        if (!debugMode) return;
        resolution_Text = Screen.width.ToString() + "x" + Screen.height.ToString();

        // measure average frames per second
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
            fps_string = string.Format(display, m_CurrentFps);
        }
    }
    private void OnGUI()
    {
        if (!debugMode) return;
        GUIStyle style = new GUIStyle();
        style.fontSize = Screen.width / 20;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(50, 5, 500, 500), fps_string, style);
        GUI.Label(new Rect(Screen.width - Screen.width / 3.5f, 5, 500, 500), resolution_Text, style);
    }


}