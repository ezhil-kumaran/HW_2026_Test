using System.Collections.Generic;
using UnityEngine;

public class PulpitManager : MonoBehaviour
{
    [Header("Prefab & Hierarchy References")]
    public GameObject pulpitPrefab;
    public Transform pulpitsParent;

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
        SpawnPulpit(Vector3.zero);
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

    public void SpawnPulpit(Vector3 gridPos)
    {
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

        // Keep maximum of 2 platforms active simultaneously
        if (activePulpits.Count > 2)
        {
            DestroyPulpit(activePulpits[0]);
        }
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
        // Direction offsets (Platform width = 9 units)
        List<Vector3> directions = new List<Vector3>
        {
            Vector3.forward * 9f,
            Vector3.back * 9f,
            Vector3.right * 9f,
            Vector3.left * 9f
        };

        // Shuffle directions to spawn randomly
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