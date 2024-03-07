using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts;
using MoreMountains.NiceVibrations;


public class GameManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private PlayerCar player;
    [SerializeField] private TextMeshProUGUI coinLabel;
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private TextMeshProUGUI scoreLabel2;
    [SerializeField] private TextMeshProUGUI highestScoreLabel;
    [SerializeField] private GameObject newRecord;
    [SerializeField] private GameObject highestRecord;
    [SerializeField] private GameObject menuUi;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private SfxManager sfxManager;
    [SerializeField] private GridsManager gridsManager;
    [SerializeField] AchievementUI achievementUI;

    private int currentScore;
    private string playerName;
    private int totalCoin;
    public string PlayerName 
    {
        get
        {
            if (PlayerPrefs.HasKey("PlayerName"))
            {
                playerName = PlayerPrefs.GetString("PlayerName");
            }
            else
            {
                int rnd = UnityEngine.Random.Range(0, 99999);
                string player = "Player" + rnd.ToString();
                playerName = player;
                PlayerPrefs.SetString("PlayerName", playerName);
            }
            return playerName;
        }
        set
        {
            playerName = value;
            PlayerPrefs.SetString("PlayerName", value);
        }
    }
    private int highestScore;
    public int HighestScore
    {
        get 
        {
            if (PlayerPrefs.HasKey(HIGHEST_RECORD))
            {
                highestScore = PlayerPrefs.GetInt(HIGHEST_RECORD);
            }
            else
            {
                highestScore = 0;
            }
            return highestScore;
        }
        
        set
        {
            highestScore = value;
            PlayerPrefs.SetInt(HIGHEST_RECORD, highestScore);
            ServerConnection.SetNewRecord(PlayerName ,HighestScore.ToString());
        }
    }
    private Spawner spawner;
    public bool gameRunning;
    private float time;
    private float scoreFloat;
    Vector3 playerDirection;
    Vector2 gridUvOffset;

    public const string HIGHEST_RECORD = "highestRecord";
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if( _instance == null)
            {
                _instance = (GameManager)FindObjectOfType(typeof(GameManager));
                return _instance;
            }
            return _instance;
        }
    }
    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
    }
    public int TotalCoin
    {
        get
        {
            if (PlayerPrefs.HasKey("totalCoin"))
            {
                totalCoin = PlayerPrefs.GetInt("totalCoin");
            }
            return totalCoin;
        }
        set
        {
            totalCoin = value;
            PlayerPrefs.SetInt("totalCoin", value);
        }
    }

    private void OnEnable()
    {
        spawner = GetComponent<Spawner>();
        IncreaseCoin(0);
        PlayerCar.OnPlayerDamage += OnPlayerDamage;
        GameEvents.Instance.OnSpawnableItemCollect += OnSpawnableItemCollect;
        GameEvents.Instance.OnEnemyDie += OnEnemyDie;
    }

    private void OnSpawnableItemCollect(GameObject go, GlobalGameItemsData.Item itemName)
    {
        switch (itemName)
        {
            case GlobalGameItemsData.Item.Coin:
                CollectCoin();
                break;
            case GlobalGameItemsData.Item.LightTrailPowerup:
                spawner.ResetTrailPowerUpTimer();
            break;
        }
    }
    public void IncreaseCoin(int addAmount)
    {
        GameEvents.Instance.OnPropertyChange?.Invoke("Coin", addAmount);
        TotalCoin += addAmount;
        UpdateCoinText();
        
    }
    public void IncreaseCoin(int addAmount, bool showPopup)
    {
        if (showPopup)
        {
            GameEvents.Instance.OnPropertyChange?.Invoke("Coin", addAmount);
        }
        TotalCoin += addAmount;
        UpdateCoinText();

    }
    public bool DecreaseCoin(int amount)
    {
        if(TotalCoin-amount >= 0)
        {
            GameEvents.Instance.OnPropertyChange?.Invoke("Coin", -amount);
            TotalCoin -= amount;
            UpdateCoinText();
            return true;
        }
        else
        {
            return false;
        }

    }
    private void UpdateCoinText()
    {
        coinLabel.text = TotalCoin.ToString();
        PlayerPrefs.SetInt("totalCoin", TotalCoin);
    }
    private void OnEnemyDie(GameObject go, EnemyData enemy, Vector3 p)
    {
        spawner.SpawnFx(GlobalGameItemsData.Item.ExplosionRedFx.ToString(),
        go.transform.position, Quaternion.identity, go);
        GameEvents.Instance.OnPropertyChange("Score", enemy.scoreAddAmount);
        scoreFloat += enemy.scoreAddAmount;
    }


    private void Update()
    {
        if (gameRunning)
        {
            UpdateScore();

        }
       // coinLabel.text = totalCoin.ToString();

        UpdateGridScroll();
    }

    void UpdateScore()
    {
        scoreFloat += Time.deltaTime;
        currentScore = (int)scoreFloat;
        scoreLabel.text = currentScore.ToString();
    }

    void UpdateGridScroll()
    {
        playerDirection = player.movingDirection;
        playerDirection = Vector3.ClampMagnitude(playerDirection, 1);
        gridUvOffset = new Vector2(playerDirection.x, playerDirection.z);
        gridsManager.ScrollGrids(-gridUvOffset);
        
    }
    public void ButtonSelect()
    {
        MMVibrationManager.Haptic(HapticTypes.Selection, true, this);
    }

    public void StartGame()
    {
        gameRunning = true;
        menuUi.SetActive(false);
        gamePanel.SetActive(true);
        spawner.SpawningStart();
        sfxManager.GameStart();
        gridsManager.StartIncreaseSpeed();
        currentScore = 0;
        // do camera animation
        CameraController.instance.StartGame();
        MMVibrationManager.Haptic(HapticTypes.RigidImpact, true, this);
        GameEvents.EventData data = new GameEvents.EventData("GameStart", 0);
        GameEvents.TriggerEvent(data);
        time = 0;
    }
    public void GameLost(PlayerCar player)
    {
        gameRunning = false;
        gridsManager.StopScrolling();
        gamePanel.SetActive(false);
        player.gameObject.SetActive(false);
        gameOverScreen.SetActive(true);
        if (!PlayerPrefs.HasKey(HIGHEST_RECORD))
        {
            HighestScore = currentScore;
            newRecord.SetActive(true);
        }
        else
        {
            if (PlayerPrefs.GetInt(HIGHEST_RECORD) < currentScore)
            {
                // new Record
                HighestScore = currentScore;

                newRecord.SetActive(true);
            }
            else
            {
                highestRecord.SetActive(true);
            }
        }
        scoreLabel2.text = currentScore.ToString();
        PlayerPrefs.SetInt("totalCoin", totalCoin);
        highestScoreLabel.text = HighestScore.ToString();
        MMVibrationManager.Haptic(HapticTypes.RigidImpact, true, this);
        sfxManager.GameEnd();
        GameEvents.EventData data = new GameEvents.EventData("GameEnd", 0);
        GameEvents.TriggerEvent(data);
        CameraController.instance.GameLost();
    }

    private void OnPlayerDamage()
    {
        CameraController.instance.OnPlayerDamage(player.invincibleTime);
        MMVibrationManager.Haptic(HapticTypes.LightImpact, true, this);
    }

    public void Restart()
    {
        ScreenFader fader = FindAnyObjectByType<ScreenFader>();
        if(fader != null)
        {
            fader.FadeToBlack(.5f, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void CollectCoin()
    {
        IncreaseCoin(1);
    }




}
