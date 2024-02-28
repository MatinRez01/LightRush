using System;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]private LeaderboardPresentor presentor;

    private static Leaderboard _instance;
    public static Leaderboard Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindAnyObjectByType<Leaderboard>();
            }
            return _instance;
        }
    }
    bool alreadyGotLeaderboard;
    public void GetLeaderboard()
    {
        if (alreadyGotLeaderboard) return;
        ServerConnection.GetTop10Records(OnGetLeaderboard);
        ServerConnection.GetMedianRecords(PlayerPrefs.GetString("PlayerPhoneNumber"), OnMedian);
    }
    public void OnGetLeaderboard(ServerModels.Top10 list)
    {
        presentor.CreateItems(list);
        alreadyGotLeaderboard = true;
    }
    public void OnMedian(ServerModels.Median list)
    {
        if(list.medianUserScore.userScore.rank > 10)
        {
            presentor.AddItems(list);
        }
    }
}