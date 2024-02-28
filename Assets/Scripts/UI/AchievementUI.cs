using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI challengeCounter;
    [SerializeField] private TextMeshProUGUI challengeDescription;
    [SerializeField] private Image progressFillImage;
    [SerializeField] private playOneShotAnimation achivementDoneAnimation;
    [SerializeField] private playOneShotAnimation achievementDonePopup;
    [SerializeField] private Transform spawnCoinsOrigin;
    [SerializeField] private int coinsAmount;
    [SerializeField] private CoinsManager coinsManager;
    private AchievementsManager achievementsManager;
    private bool shouldShowAchievementDone;
    private int doneAchievementCount;

    private void Awake()
    {
        achievementsManager = GetComponent<AchievementsManager>();
    }
    private void OnEnable()
    {
        achievementsManager.OnAchievementDoneEvent += AnAchievementDone;
        if (ShouldShowAchievementDone)
        {
            ShowAchievementDoneInMenu();
        }
        else
        {
            UpdateMenuUI();
        }
    }
    void AnAchievementDone(float addAmount)
    {
        Debug.Log("An achievement Done");
        ShouldShowAchievementDone = true;
        DoneAchievementCount++;
        if(challengeCounter.gameObject.activeInHierarchy)
        {
            Debug.Log("Menu Animation Shown");
            ShowAchievementDoneInMenu();
        }
        ShowAchievementDonePopup();

    }
    void ShowAchievementDoneInMenu()
    {
        StartCoroutine(_MenuAnimationShow());
    }
    IEnumerator _MenuAnimationShow()
    {
        int currentIndex = achievementsManager.CurrentAchievementIndex;
        int index = currentIndex - DoneAchievementCount;
        SetMenuUI(achievementsManager.GetAchievementData(index));
        for (int i = 0; i < DoneAchievementCount; i++)
        {
            achivementDoneAnimation.PlayAnimation();
            yield return new WaitForSeconds(1.5f);
            AddCoinAnimation(achievementsManager.GetAchievementData(index).coinAddAmt);
            index++;
            SetMenuUI(achievementsManager.GetAchievementData(index));
        }
        UpdateMenuUI();
        yield return null;
    }
    public void AddCoinAnimation(int coinAmount)
    {
        coinsManager.AddCoins(spawnCoinsOrigin.position, coinAmount);
    }
    void ShowAchievementDonePopup()
    {
        achievementDonePopup.PlayAnimationsAndDisable();
    }
    private void SetMenuUI(AchievementData data)
    {
        challengeDescription.text = data.description;
        int i = int.Parse(data.id) + 1;
        challengeCounter.text = "Challenge" + i;
    }
    private void UpdateMenuUI()
    {
        if (achievementsManager.AchievementsAllDone)
        {
            challengeDescription.text = "Challenges Complete!";
            challengeCounter.text = "";

        }
        else
        {
            challengeDescription.text = achievementsManager.CurrentAchievementDescription;
            challengeCounter.text = achievementsManager.CurrentAchievementCounter;
            progressFillImage.fillAmount = achievementsManager.GetCurrentAchievementProgress();
            ShouldShowAchievementDone = false;
            DoneAchievementCount = 0;
        }
    }

    private bool ShouldShowAchievementDone
    {
        get
        {
            if (PlayerPrefs.HasKey("ShouldShowAchievementDone"))
            {
                int v = PlayerPrefs.GetInt("ShouldShowAchievementDone");
                shouldShowAchievementDone = v > 0;
            }
            else
            {
                shouldShowAchievementDone = false;
            }
            return shouldShowAchievementDone;
        }
        set
        {
            shouldShowAchievementDone = value;
            PlayerPrefs.SetInt("ShouldShowAchievementDone", value ? 1 : 0);
        }
    }
    private int DoneAchievementCount
    {
        get
        {
            if (PlayerPrefs.HasKey("DoneAchievementCount"))
            {
                int v = PlayerPrefs.GetInt("DoneAchievementCount");
                doneAchievementCount = v;
            }
            else
            {
                doneAchievementCount = 0;
            }
            return doneAchievementCount;
        }
        set
        {
            doneAchievementCount = value;
            PlayerPrefs.SetInt("DoneAchievementCount", value);
        }
    }

}
