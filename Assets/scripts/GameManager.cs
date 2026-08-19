using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Loading, Playing, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Loading;

    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    private int score = 0;
    private int highScore = 0;
    private HashSet<Pulpit> steppedPulpits = new HashSet<Pulpit>();
    private DoofusController doofus;
    private PulpitManager pulpitManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        doofus = FindObjectOfType<DoofusController>();
        pulpitManager = FindObjectOfType<PulpitManager>();
        gameOverPanel.SetActive(false);
        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();
        GameConfig.Instance.OnConfigLoaded += OnConfigLoaded;
    }

    private void OnConfigLoaded(GameConfigData config)
    {
        StartGame();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        score = 0;
        steppedPulpits.Clear();
        UpdateScoreUI();
        gameOverPanel.SetActive(false);
        pulpitManager.StartGame();
        doofus.ResetDoofus(Vector3.up * 2f);
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        gameOverPanel.SetActive(true);
        finalScoreText.text = $"Score: {score}";
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        UpdateScoreUI();
    }

    public void IncrementScore(Pulpit pulpit)
    {
        score++;
        steppedPulpits.Add(pulpit);
        UpdateScoreUI();
    }

    public bool HasSteppedOnPulpit(Pulpit pulpit)
    {
        return steppedPulpits.Contains(pulpit);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
        if (highScoreText != null)
            highScoreText.text = $"High Score: {highScore}";
    }

    public void OnRestartButton()
    {
        StartGame();
    }
}
