using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ServerModels;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image rankHolder;
    [SerializeField] private TopRanks topRanks;

    [System.Serializable]
    public struct TopRanks
    {
        public Sprite First;
        public Sprite Second;
        public Sprite Third;
    }

    public void Setup(User data)
    {

        highScoreText.text = data.topScore.ToString();
        nameText.text = data.name;
        rankText.text = data.rank.ToString();

        switch (data.rank)
        {
            case 1:
                rankHolder.sprite = topRanks.First;
                break;
            case 2:
                rankHolder.sprite = topRanks.Second;
                break;
            case 3:
                rankHolder.sprite = topRanks.Third;
                break;
            default:
                rankHolder.enabled = false;
                rankText.gameObject.SetActive(true);
                break;
        }

    }
}