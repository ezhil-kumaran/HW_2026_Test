using TMPro;
using UnityEngine;

public class Pulpit : MonoBehaviour
{
    [HideInInspector] public float destroyTime;
    [HideInInspector] public float pulpitSpawnTime;
    [HideInInspector] public Vector3 gridPosition;

    private float timer;
    private bool spawnTriggered = false;
    private TextMeshPro timerText;
    private PulpitManager pulpitManager;

    private void Awake()
    {
        timerText = GetComponentInChildren<TextMeshPro>();
    }

    public void Init(PulpitManager manager, Vector3 gridPos, float minDestroy, float maxDestroy, float spawnTime)
    {
        pulpitManager = manager;
        gridPosition = gridPos;
        destroyTime = Random.Range(minDestroy, maxDestroy);
        pulpitSpawnTime = spawnTime;
        timer = destroyTime;
        spawnTriggered = false;
        UpdateTimerUI();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        UpdateTimerUI();

        if (!spawnTriggered && timer <= (destroyTime - pulpitSpawnTime))
        {
            spawnTriggered = true;
            pulpitManager.OnPulpitSpawnTimeReached(this);
        }

        if (timer <= 0f)
        {
            pulpitManager.DestroyPulpit(this);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"{Mathf.Max(0, timer):0.0}s";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pulpitManager.OnDoofusStepped(this);
        }
    }
}
