using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Loading, Playing, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Loading;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    private int score = 0;
    private int highScore = 0;
    private readonly HashSet<Pulpit> steppedPulpits = new HashSet<Pulpit>();
    private DoofusController doofus;
    private PulpitManager pulpitManager;
    [Header("Scene Navigation")]
    [Tooltip("The build index of your StartScene / Main Menu.")]
    [SerializeField] private int mainMenuSceneIndex = 1; // Set to your StartScene build index

    // Hook this to your 'Return to Main Menu' button's OnClick()
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Ensure physics/time are unpaused
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        FetchSceneReferences();
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();

        if (GameConfig.Instance != null)
        {
            // If already loaded, start game immediately
            if (GameConfig.Instance.IsLoaded)
            {
                StartGame();
            }
            else
            {
                // Otherwise wait for network response
                GameConfig.Instance.OnConfigLoaded += OnConfigLoaded;
            }
        }
        else
        {
            // Fallback if GameConfig component is absent in scene
            StartGame();
        }
    }

    private void OnDestroy()
    {
        if (GameConfig.Instance != null)
        {
            GameConfig.Instance.OnConfigLoaded -= OnConfigLoaded;
        }
    }

    private void FetchSceneReferences()
    {
        if (doofus == null)
            doofus = FindFirstObjectByType<DoofusController>();

        if (pulpitManager == null)
            pulpitManager = FindFirstObjectByType<PulpitManager>();
    }

    private void OnConfigLoaded(GameConfigData config)
    {
        StartGame();
    }

    public void StartGame()
    {
        FetchSceneReferences();

        CurrentState = GameState.Playing;
        score = 0;
        steppedPulpits.Clear();
        UpdateScoreUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pulpitManager != null)
            pulpitManager.StartGame();

        if (doofus != null)
            doofus.ResetDoofus(Vector3.up * 2f);
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = $"{score}";

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        UpdateScoreUI();
    }

    public void IncrementScore(Pulpit pulpit)
    {
        if (pulpit == null) return;

        score++;
        steppedPulpits.Add(pulpit);
        UpdateScoreUI();
    }
    public void AddBonusScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }
    public bool HasSteppedOnPulpit(Pulpit pulpit)
    {
        return pulpit != null && steppedPulpits.Contains(pulpit);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"{score}";

        if (highScoreText != null)
            highScoreText.text = $"High Score: {highScore}";
    }

    public void OnRestartButton()
    {
        StartGame();
    }
}