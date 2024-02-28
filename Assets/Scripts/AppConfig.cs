using MoreMountains.NiceVibrations;
using Tools;
using UnityEngine;

public class AppConfig : MonoBehaviour
{
    [MinMaxSlider(0.6f, 1), SerializeField] Vector2 resolutionMinAndMax;
    [SerializeField] private PlanarReflection pl;
    [SerializeField] private FastBloom bloom;
    [SerializeField] private SfxManager sfxManager;

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
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;

        sfxManager.SoundMute(!SoundOn);
        MMVibrationManager.SetHapticsActive(Vibrate);
        SetResolution();
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
        //  Resolution nativeRes = Display.main. Screen.currentResolution;
        if (BatterySaving)
        {
            resolutionPercentage = resolutionMinAndMax.x;
            bloom.enabled = false;
        }
        else
        {
            resolutionPercentage = resolutionMinAndMax.y;
            bloom.enabled = true;
        }
        //calculate 80% of the native resolution
        int width = (int)(Display.main.systemWidth * resolutionPercentage);
        int height = (int)(Display.main.systemHeight * resolutionPercentage);

        //set the resolution to 80% of the native resolution
        Screen.SetResolution(width, height, true);
    }
    public void BatterySavingChange()
    {
        BatterySaving = !BatterySaving;
        SetResolution();
        if (BatterySaving) 
        {
            pl.ReflectionTexResolution = 256;
        }
        else
        {
            pl.ReflectionTexResolution = 512;
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
           GUIStyle style = new GUIStyle();
        style.fontSize = Screen.width / 20;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(50, 5, 500, 500), fps_string, style);
        GUI.Label(new Rect(Screen.width - Screen.width/3.5f, 5, 500, 500), resolution_Text, style);
    }


}