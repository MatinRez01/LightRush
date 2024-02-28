using UMM;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class IntentButton : MonoBehaviour
{
    public enum Intent { OpenApp, OpenComments, OpenDeveloperApps, OpenMarketLogin }

    [SerializeField] private Intent onClick;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        switch (onClick)
        {
            case Intent.OpenApp:
                MarketIntents.OpenApp();
                break;
            case Intent.OpenComments:
                MarketIntents.OpenComments();
                break;
            case Intent.OpenDeveloperApps:
                MarketIntents.OpenDeveloperApps();
                break;
            case Intent.OpenMarketLogin:
                MarketIntents.OpenMarketLogin();
                break;
            default:
                break;
        }
    }
}