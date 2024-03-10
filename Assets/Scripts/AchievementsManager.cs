using System;
using UnityEngine;
using UnityEngine.Events;

public class AchievementsManager : MonoBehaviour
{
    [SerializeField] private AchievementsDatabase database;
    [SerializeField] private GameManager gameManager;
    [Space]
    [SerializeField] private int minAchievementCoinAddAmt;
    [SerializeField] private int eachAchievementCoinAddAmt;
    [SerializeField] private int maxAchievementCoinAddAmt;
    private Achievement[] achievments;
    private int currentAchievementIndex = 0;
    Achievement currentAchievement;
    public Action<float> OnAchievementDoneEvent;
    public bool AchievementsAllDone;
    public enum AchievementType
    {
        SurviveTime,
        ScoreMoreThan,
        Counter,
        DontTime, 
        Period
    }

    void Awake()
    {
        achievments = new Achievement[database.Achievements.Length];

        for (int i = 0; i < achievments.Length; i++)
        {
            achievments[i] = new Achievement();
            achievments[i].data = database.Achievements[i].data;
            achievments[i].requirement = database.Achievements[i].requirement;
            achievments[i].type = database.Achievements[i].type;
        }

        gameManager = FindObjectOfType<GameManager>();

    }
    private void OnEnable()
    {

        if (PlayerPrefs.HasKey("CurrentAchievement"))
        {
            CurrentAchievementIndex = PlayerPrefs.GetInt("CurrentAchievement");
        }
        else
        {
            CurrentAchievementIndex = 0;
        }
        UpdateCurrentAchievement();
    }
    private void Update()
    {
        currentAchievement.Check();
    }
    void UpdateCurrentAchievement()
    {
        if(currentAchievementIndex >= achievments.Length)
        {
            AchievementsAllDone = true;
            return;
        }
        Achievement ach = achievments[currentAchievementIndex];
        ach.data.coinAddAmt = Mathf.Clamp(minAchievementCoinAddAmt + (eachAchievementCoinAddAmt * currentAchievementIndex),
            minAchievementCoinAddAmt, maxAchievementCoinAddAmt);
        if (PlayerPrefs.HasKey("Achievement" + ach.data.id))
        {
            ach.Progress = PlayerPrefs.GetFloat("Achievement" + ach.data.id);
        }
        else
        {
            PlayerPrefs.SetFloat("Achievement" + ach.data.id, 0);
        }
        switch (achievments[currentAchievementIndex].type)
        {
            case AchievementType.SurviveTime:
                currentAchievement = new TimeAchievment(ach.data, ach.requirement, ach.Progress);
                break;
            case AchievementType.DontTime:
                currentAchievement = new DontTime(ach.data, ach.requirement, ach.Progress);
                break;
            case AchievementType.Period:
                currentAchievement = new Period(ach.data, ach.requirement, ach.Progress);
                break;
            case AchievementType.Counter:
                currentAchievement = new Counter(ach.data, ach.requirement, ach.Progress);
                break;
            case AchievementType.ScoreMoreThan:
                currentAchievement = new ScoreMoreThan(ach.data, ach.requirement, ach.Progress);
                break;
        }



        GameEvents.Instance.OnEvent += OnEventTriggered;
      
        currentAchievement.OnDone += OnAchievementDone;

    }
    void OnEventTriggered(GameEvents.EventData eventData)
    {
        foreach (var item in currentAchievement.data.relatedEvents)
        {
            if(item == eventData.eventName)
            {
                currentAchievement.OnEventTriggered(eventData);
            }
        }
    }
    void OnAchievementDone()
    {
        OnAchievementDoneEvent.Invoke(currentAchievement.data.coinAddAmt);
        CurrentAchievementIndex++;
        PlayerPrefs.SetInt("CurrentAchievement", CurrentAchievementIndex);
        UpdateCurrentAchievement();
    }
    
    public AchievementData GetCurrentAchievementData()
    {
        return currentAchievement.data;
    }
    public AchievementData GetAchievementData(int index)
    {
        return achievments[index].data;
    }
    public float GetCurrentAchievementProgress()
    {
        return currentAchievement.Progress;
    }
    public int CurrentAchievementIndex
    {
        get
        {
            return currentAchievementIndex;
        }
        set
        {
            currentAchievementIndex = value;
        }
    }
    public string CurrentAchievementDescription
    {
        get
        {
            return currentAchievement.data.description;
        }
    }
    public string CurrentAchievementCounter
    {
        get
        {
            return "Challenge " + (currentAchievementIndex + 1);
        }
    }

}

[Serializable]
public class Achievement
{
    [SerializeField] public AchievementData data;
    [SerializeField] public float requirement;
    [SerializeField] public AchievementsManager.AchievementType type;
    private float progress;
    private bool done;
    public UnityAction OnDone;

    public bool Done
    {
        get
        {
            return done;
        }
        set
        {
            done = value;
            if (done)
            {
                SetDone();
            }
        }
    }
    public float Progress
    {
        get
        {
            return progress;
        }
        set
        {
            progress = value;
            if (data.saveProgress)
            {
                PlayerPrefs.SetFloat("Achievement" + data.id, progress);
            }
            Done = progress > .95;
        }
    }
    public virtual void OnEventTriggered(GameEvents.EventData eventData)
    {

    }
    public virtual void Check()
    {

    }

    public void SetDone()
    {
        OnDone.Invoke();
    }
}
[Serializable]
public class AchievementData
{
    [SerializeField] public string description;
    [SerializeField] public string id;
    [SerializeField] public string[] relatedEvents;
    public bool saveProgress;
    [HideInInspector]public int coinAddAmt;
    
    public AchievementData(string id, string description)
    {
        this.id = id;
        this.description = description;
    }

}
public class TimeAchievment : Achievement
{
    float timeElapsed;
    bool startTicking;
    public TimeAchievment(AchievementData data, float timeRequireMent, float progress) 
    {
        this.data = data;
        this.requirement = timeRequireMent;
        this.Progress = progress ;
        timeElapsed = (Progress * timeRequireMent);
    }


    public override void OnEventTriggered(GameEvents.EventData eventData)
    {

        switch (eventData.eventName)
        {
            case "GameStart":
                startTicking = true;
                break;
            case "GameEnd":
                startTicking = false;
                break;
        }
    }

    public override void Check()
    {
        if (GameManager.Instance.gameRunning)
        {
            startTicking = true;
        }
        if (startTicking)
        {
            timeElapsed += Time.deltaTime;
            Progress = timeElapsed / requirement ;
        }
    }

}
public class Period : Achievement
{
    float timeElapsed;
    bool startTicking;
    public Period(AchievementData data, float timeRequireMent, float progress)
    {
        this.data = data;
        this.requirement = timeRequireMent;
        this.Progress = progress;
        timeElapsed = (Progress * timeRequireMent);
    }


    public override void OnEventTriggered(GameEvents.EventData eventData)
    {
        switch (eventData.value)
        {
            case 0:
                startTicking = false;
                break;
            case 1:
                startTicking = true;
                break;
        }

    }

    public override void Check()
    {
        if (startTicking)
        {
            timeElapsed += Time.deltaTime;
            Progress = timeElapsed / requirement;
        }
    }

}
public class DontTime : Achievement
{
    float timeElapsed;
    bool startTicking;
    bool failed;
    public DontTime(AchievementData data, float timeRequireMent, float progress)
    {
        this.data = data;
        this.requirement = timeRequireMent;
        this.Progress = progress;
        timeElapsed = (Progress * timeRequireMent);
    }


    public override void OnEventTriggered(GameEvents.EventData eventData)
    {
        switch (eventData.eventName)
        {
            case "GameStart":
                startTicking = true;
                failed = false;
                break;
            case "GameEnd":
                startTicking = false;
                failed = true;
                break;
            default:
                failed = true;
                break;
        }
    }

    public override void Check()
    {
        if (GameManager.Instance.gameRunning)
        {
            startTicking = true;
        }
        if (startTicking && !failed)
        {
            timeElapsed += Time.deltaTime;
            Progress = timeElapsed / requirement;
        }
    }

}
public class ScoreMoreThan : Achievement
{
    bool startChecking;
    int score;
    public ScoreMoreThan(AchievementData data, float scoreRequireMent, float progress)
    {
        this.data = data;
        this.requirement = scoreRequireMent;
        this.Progress = progress;
        score = (int)(Progress * requirement);
    }
    public override void Check()
    {
        if (!data.saveProgress)
        {
            if (GameManager.Instance.gameRunning)
            {
                startChecking = true;
            }
            if (startChecking)
            {
                score = GameManager.Instance.CurrentScore;
                Progress = score / requirement;
            }
        }
        
    }
    public override void OnEventTriggered(GameEvents.EventData eventData)
    {

        switch (eventData.eventName)
        {
            case "GameEnd":
                if (data.saveProgress)
                {
                    score += GameManager.Instance.CurrentScore;
                    Progress = score / requirement;
                }
                break;
        }
    }
}
/*public class ComparisonAchievement : Achievement
{
    float value;
    public ComparisonAchievement(float progress)
    {
        this.Progress = progress;
    }

    public override void Check()
    {
        
    }

    public override void OnEventTriggered(GameEvents.EventData eventData)
    {
        Progress = (eventData.value / value) * 100;
    }
}*/
public class Counter : Achievement
{
    float value;
    public Counter( AchievementData data, float require, float progress) 
    {
        this.data = data;
        this.requirement = require;
        this.Progress = progress;
        this.value = (Progress * requirement);

    }
    public override void Check()
    {
        
    }

    public override void OnEventTriggered(GameEvents.EventData eventData)
    {
        value++;
        Progress = (value / requirement);
    }
}