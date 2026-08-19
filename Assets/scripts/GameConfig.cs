using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class PlayerData
{
    public float speed = 3.0f;
}

[Serializable]
public class PulpitData
{
    public float min_pulpit_destroy_time = 4.0f;
    public float max_pulpit_destroy_time = 5.0f;
    public float pulpit_spawn_time = 2.5f;
}

[Serializable]
public class GameConfigData
{
    public PlayerData player_data = new PlayerData();
    public PulpitData pulpit_data = new PulpitData();
}

public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance { get; private set; }
    public GameConfigData Config { get; private set; } = new GameConfigData();
    public bool IsLoaded { get; private set; } = false;

    public event Action<GameConfigData> OnConfigLoaded;

    private const string ConfigUrl = "https://s3.ap-south-1.amazonaws.com/superstars.assetbundles.testbuild/doofus_game/doofus_diary.json";

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
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(LoadConfig());
    }

    private IEnumerator LoadConfig()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(ConfigUrl))
        {
            www.timeout = 5;
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    Config = JsonUtility.FromJson<GameConfigData>(www.downloadHandler.text);
                    Debug.Log("<color=green>GameConfig loaded successfully from server!</color>");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to parse config JSON: {e.Message}. Using default fallback values.");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to fetch config ({www.error}). Using default fallback values.");
            }
        }

        IsLoaded = true;
        OnConfigLoaded?.Invoke(Config);
    }
}