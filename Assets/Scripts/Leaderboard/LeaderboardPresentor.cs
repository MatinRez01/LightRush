using UnityEngine;

public class LeaderboardPresentor : MonoBehaviour
{
    [SerializeField] private Transform itemsHolder;
    [SerializeField] private LeaderboardItem itemPrefab;
    [SerializeField] private LeaderboardItem itemPlayerPrefab;

    private void DeleteExistItems()
    {
        foreach (Transform obj in itemsHolder.transform)
            Destroy(obj.gameObject);
    }
    public void CreateItems(ServerModels.Top10 items)
    {
        DeleteExistItems();

        foreach (var item in items.topUsers)
        {
            if (item.name != GameManager.Instance.PlayerName)
            {
                var ui = Instantiate(itemPrefab, itemsHolder.transform);
                ui.Setup(item);
            }
            else
            {
                var ui = Instantiate(itemPlayerPrefab, itemsHolder.transform);
                ui.Setup(item);
            }
        }
    }
    public void AddItems(ServerModels.Median items)
    {
        if (items.medianUserScore.userScore.rank != 11)
        {
            var uiHigher = Instantiate(itemPrefab, itemsHolder.transform);
            uiHigher.Setup(items.medianUserScore.userWithHigherScore);
        }
            var uiPlayer = Instantiate(itemPlayerPrefab, itemsHolder.transform);
            uiPlayer.Setup(items.medianUserScore.userScore);
            var uilower = Instantiate(itemPrefab, itemsHolder.transform);
            uilower.Setup(items.medianUserScore.userWithLowerScore);
    }
}