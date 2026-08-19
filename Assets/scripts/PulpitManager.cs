using System.Collections.Generic;
using UnityEngine;

public class PulpitManager : MonoBehaviour
{
    public GameObject pulpitPrefab;
    public Transform pulpitsParent;

    private List<Pulpit> activePulpits = new List<Pulpit>();
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private float minDestroy, maxDestroy, spawnTime;

    private void Start()
    {
        GameConfig.Instance.OnConfigLoaded += OnConfigLoaded;
        if (GameConfig.Instance.Config != null)
            ApplyConfig(GameConfig.Instance.Config);
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
        StartGame();
    }

    public void StartGame()
    {
        ClearAllPulpits();
        Vector3 startPos = Vector3.zero;
        SpawnPulpit(startPos);
    }

    public void ClearAllPulpits()
    {
        foreach (var p in activePulpits)
            Destroy(p.gameObject);
        activePulpits.Clear();
        occupiedPositions.Clear();
    }

    public void SpawnPulpit(Vector3 gridPos)
    {
        GameObject go = Instantiate(pulpitPrefab, gridPos, Quaternion.identity, pulpitsParent);
        Pulpit pulpit = go.GetComponent<Pulpit>();
        pulpit.Init(this, gridPos, minDestroy, maxDestroy, spawnTime);
        activePulpits.Add(pulpit);
        occupiedPositions.Add(gridPos);

        // Keep only 2 pulpits
        if (activePulpits.Count > 2)
        {
            DestroyPulpit(activePulpits[0]);
        }
    }

    public void DestroyPulpit(Pulpit pulpit)
    {
        occupiedPositions.Remove(pulpit.gridPosition);
        activePulpits.Remove(pulpit);
        Destroy(pulpit.gameObject);
    }

    public void OnPulpitSpawnTimeReached(Pulpit current)
    {
        if (activePulpits.Count >= 2) return;

        Vector3[] directions = {
            Vector3.forward * 9f,
            Vector3.back * 9f,
            Vector3.right * 9f,
            Vector3.left * 9f
        };

        foreach (var dir in directions)
        {
            Vector3 newPos = current.gridPosition + dir;
            if (!occupiedPositions.Contains(newPos))
            {
                SpawnPulpit(newPos);
                break;
            }
        }
    }

    public void OnDoofusStepped(Pulpit pulpit)
    {
        if (!GameManager.Instance.HasSteppedOnPulpit(pulpit))
        {
            GameManager.Instance.IncrementScore(pulpit);
        }
    }
}

