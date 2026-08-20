using TMPro;
using UnityEngine;

public class Pulpit : MonoBehaviour
{
    [HideInInspector] public float destroyTime;
    [HideInInspector] public float pulpitSpawnTime;
    [HideInInspector] public Vector3 gridPosition;

    private float timer;
    private bool spawnTriggered = false;
    private bool isInitialized = false;
    private TMP_Text timerText;
    private PulpitManager pulpitManager;

    private void Awake()
    {
        timerText = GetComponentInChildren<TMP_Text>();
    }

    public void Init(PulpitManager manager, Vector3 gridPos, float minDestroy, float maxDestroy, float spawnTime)
    {
        pulpitManager = manager;
        gridPosition = gridPos;
        destroyTime = Random.Range(minDestroy, maxDestroy);
        pulpitSpawnTime = spawnTime;
        timer = destroyTime;
        spawnTriggered = false;
        isInitialized = true;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isInitialized || pulpitManager == null) return;

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
        if (other.CompareTag("Player") && pulpitManager != null)
        {
            // Calculate 2D horizontal distance between platform center and Doofus
            Vector2 pulpitPos2D = new Vector2(transform.position.x, transform.position.z);
            Vector2 playerPos2D = new Vector2(other.transform.position.x, other.transform.position.z);

            // Platform is 9x9, so half-width is 4.5. 
            // Only trigger if player is within 4 units of this platform's center.
            if (Vector2.Distance(pulpitPos2D, playerPos2D) < 4.0f)
            {
                pulpitManager.OnDoofusStepped(this);
            }
        }
    }
}