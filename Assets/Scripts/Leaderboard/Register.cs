using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField playerPhoneNumber;
    [SerializeField] private Toggle termsOfServiceToggle;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject signinPanel;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button privacyButton;
    [SerializeField] private Button closePrivacyButton;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private float buttonSoundVolume = 1;


    public const string PLAYER_PHONE_NUMBER_PREFS = "PlayerPhoneNumber";
    public const string PLAYER_NAME_PREFS = "PlayerName";

    void Start()
    {

        submitButton.onClick.AddListener(() => ComfirmButton());
       // privacyButton.onClick.AddListener(()=> OnclickPrivacy());
      //  closePrivacyButton.onClick.AddListener(()=> OnclickClosePrivacy());

    }
    public void OnLeaderboardButton()
    {
        if (!ServerConnection.IsConnected())
        {
            ErrorHandler.Instance.ShowError("NO INTERNET!");
            return;
        }
        if(GameManager.Instance.HighestScore == 0)
        {
            ErrorHandler.Instance.ShowError("PLAY A RUN TO UNLOCK LEADERBOARD!");
            return;
        }
        if (ServerConnection.LoggedIn())
        {
            leaderboardPanel.SetActive(true);
            Leaderboard.Instance.GetLeaderboard();
        }
        else
        {
            signinPanel.SetActive(true);
        }
    }
    public void ComfirmButton()
    {
        if (!termsOfServiceToggle.isOn)
        {
            ErrorHandler.Instance.ShowError("PLEASE ACCEPT OUR TERMS OF SERVICE TO LOG IN"); return;
        }
        if (!ServerConnection.IsConnected())
        {
            ErrorHandler.Instance.ShowError("NO INTERNET");
            return;
        }

        if (playerName.text == "" && playerPhoneNumber.text == "")
        {
            ErrorHandler.Instance.ShowError("USERNAME OR PHONENUMBER INVALID!");
            return;
        }

        if (playerName.text == "")
        {
            ErrorHandler.Instance.ShowError("USERNAME INVALID!");
            return;
        }
        if (playerName.text.Length < 3 || playerName.text.Length > 12)
        {
            ErrorHandler.Instance.ShowError("USERNAME TOO SHORT OR TOO LONG!");
            return;
        }

        if (playerPhoneNumber.text == "")
        {
            ErrorHandler.Instance.ShowError("PLEASE ENTER PHONENUMBER!");
            return;
        }

        if (playerPhoneNumber.text[0] != '0' || playerPhoneNumber.text[1] != '9')
        {
            ErrorHandler.Instance.ShowError("PHONENUMBER INVALID!");
            return;
        }
        if (playerPhoneNumber.text.Length != 11)
        {
            ErrorHandler.Instance.ShowError("PHONENUMBER INVALID!");
            return;
        }

        PlayerPrefs.SetString(PLAYER_NAME_PREFS, playerName.text);
        PlayerPrefs.SetString(PLAYER_PHONE_NUMBER_PREFS, playerPhoneNumber.text.Trim());
        ServerConnection.SetNewRecord(GameManager.Instance.PlayerName, PlayerPrefs.GetString(PLAYER_PHONE_NUMBER_PREFS), GameManager.Instance.HighestScore.ToString(), () =>
        {
            signinPanel.SetActive(false);
        });

    }

    private void OnclickPrivacy()
    {
      //  StartCoroutine(menuLoader.Loader(menuLoader.RegisterMenu, menuLoader.PrivacyMenu));
      //  MusicManager.Instance.SfxPlayer(buttonSound, buttonSoundVolume);
    }
    private void OnclickClosePrivacy()
    {
       // StartCoroutine(menuLoader.Loader(menuLoader.PrivacyMenu, menuLoader.RegisterMenu));
       // MusicManager.Instance.SfxPlayer(buttonSound, buttonSoundVolume);
    }
}