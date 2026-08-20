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
        {
            // "0.00" formats the float with 2 decimal places (e.g. 1.84)
            timerText.text = Mathf.Max(0f, timer).ToString("0.00");
        }
    }

    private bool hasGivenScore = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasGivenScore) return;

        if (other.CompareTag("Player") && pulpitManager != null)
        {
            // 2D Distance check between platform center and player
            Vector2 pulpitPos2D = new Vector2(transform.position.x, transform.position.z);
            Vector2 playerPos2D = new Vector2(other.transform.position.x, other.transform.position.z);

            // Within 4 units of platform center
            if (Vector2.Distance(pulpitPos2D, playerPos2D) < 4.0f)
            {
                hasGivenScore = true;
                pulpitManager.OnDoofusStepped(this);
            }
        }
    }
}