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

    public event Action<GameConfigData> OnConfigLoaded;

    private const string ConfigUrl = "https://s3.ap-south-1.amazonaws.com/superstars.assetbundles.testbuild/doofus_game/doofus_diary.json";
    private bool configLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadConfig());
        }
        else
        {
            Destroy(gameObject);
        }
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
                    configLoaded = true;
                }
                catch
                {
                    Debug.LogWarning("Failed to parse config JSON. Using defaults.");
                }
            }
            else
            {
                Debug.LogWarning("Failed to fetch config. Using defaults.");
            }
        }
        OnConfigLoaded?.Invoke(Config);
    }
}
