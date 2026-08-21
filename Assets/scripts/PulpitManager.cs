using System.Collections.Generic;
using UnityEngine;

public class PulpitManager : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject pulpitPrefab;
    public GameObject collectiblePrefab; // Assign Star Collectible Prefab here
    public Transform pulpitsParent;

    [Header("Collectible Settings")]
    [Range(0f, 1f)] public float collectibleSpawnChance = 0.7f; // 70% chance to spawn a star

    private readonly List<Pulpit> activePulpits = new List<Pulpit>();
    private readonly HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private float minDestroy = 4f;
    private float maxDestroy = 5f;
    private float spawnTime = 2.5f;

    private void Start()
    {
        if (GameConfig.Instance != null)
        {
            GameConfig.Instance.OnConfigLoaded += OnConfigLoaded;
            if (GameConfig.Instance.Config != null)
            {
                ApplyConfig(GameConfig.Instance.Config);
            }
        }
    }

    private void OnDestroy()
    {
        if (GameConfig.Instance != null)
        {
            GameConfig.Instance.OnConfigLoaded -= OnConfigLoaded;
        }
    }

    private void OnConfigLoaded(GameConfigData config)
    {
        ApplyConfig(config);
    }

    private void ApplyConfig(GameConfigData config)
    {
        minDestroy = config.pulpit_data.min_pulpit_destroy_time;
        maxDestroy = config.pulpit_data.max_pulpit_destroy_time;
        spawnTime = config.pulpit_data.pulpit_spawn_time;
    }

    public void StartGame()
    {
        ClearAllPulpits();
        SpawnPulpit(Vector3.zero, isStartPlatform: true);
    }

    public void StopSpawning()
    {
        for (int i = 0; i < activePulpits.Count; i++)
        {
            if (activePulpits[i] != null)
            {
                activePulpits[i].StopAllCoroutines();
            }
        }
    }

    public void ClearAllPulpits()
    {
        for (int i = activePulpits.Count - 1; i >= 0; i--)
        {
            if (activePulpits[i] != null)
            {
                Destroy(activePulpits[i].gameObject);
            }
        }
        activePulpits.Clear();
        occupiedPositions.Clear();
    }

    public void SpawnPulpit(Vector3 gridPos, bool isStartPlatform = false)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        if (pulpitPrefab == null)
        {
            Debug.LogError("PulpitPrefab is not assigned in PulpitManager!");
            return;
        }

        Transform parent = (pulpitsParent != null) ? pulpitsParent : transform;
        GameObject go = Instantiate(pulpitPrefab, gridPos, Quaternion.identity, parent);

        Pulpit pulpit = go.GetComponent<Pulpit>();
        pulpit.Init(this, gridPos, minDestroy, maxDestroy, spawnTime);

        activePulpits.Add(pulpit);
        occupiedPositions.Add(gridPos);

        // Spawn collectible on newly generated platforms (skip starting platform)
        if (!isStartPlatform && collectiblePrefab != null && Random.value <= collectibleSpawnChance)
        {
            SpawnCollectibleOnPulpit(go.transform, gridPos);
        }

        if (activePulpits.Count > 2)
        {
            DestroyPulpit(activePulpits[0]);
        }
    }

    private void SpawnCollectibleOnPulpit(Transform pulpitTransform, Vector3 pulpitPos)
    {
        // Safe inner boundary: random offset within -3 to +3 on X and Z
        float randomX = Random.Range(-3.0f, 3.0f);
        float randomZ = Random.Range(-3.0f, 3.0f);
        float heightY = 0.8f; // Hover slightly above platform surface

        Vector3 spawnLocation = pulpitPos + new Vector3(randomX, heightY, randomZ);

        // Parent to the pulpit so it gets deleted automatically when the platform expires
        Instantiate(collectiblePrefab, spawnLocation, Quaternion.identity, pulpitTransform);
    }

    public void DestroyPulpit(Pulpit pulpit)
    {
        if (pulpit == null) return;

        occupiedPositions.Remove(pulpit.gridPosition);
        activePulpits.Remove(pulpit);
        Destroy(pulpit.gameObject);
    }

    public void OnPulpitSpawnTimeReached(Pulpit current)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        List<Vector3> directions = new List<Vector3>
        {
            Vector3.forward * 9f,
            Vector3.back * 9f,
            Vector3.right * 9f,
            Vector3.left * 9f
        };

        for (int i = 0; i < directions.Count; i++)
        {
            Vector3 temp = directions[i];
            int randomIndex = Random.Range(i, directions.Count);
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        foreach (var dir in directions)
        {
            Vector3 newPos = current.gridPosition + dir;
            if (!occupiedPositions.Contains(newPos))
            {
                SpawnPulpit(newPos);
                return;
            }
        }
    }

    public void OnDoofusStepped(Pulpit pulpit)
    {
        if (GameManager.Instance != null && !GameManager.Instance.HasSteppedOnPulpit(pulpit))
        {
            GameManager.Instance.IncrementScore(pulpit);
        }
    }
}