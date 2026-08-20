using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DoofusController : MonoBehaviour
{
    private Rigidbody rb;
    private float moveSpeed = 3.0f;
    private bool isDead = false;

    private void Awake()
    {
        // Cache Rigidbody early to avoid null reference when ResetDoofus is called at start
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (GameConfig.Instance != null)
        {
            GameConfig.Instance.OnConfigLoaded += OnConfigLoaded;
            if (GameConfig.Instance.Config != null)
            {
                moveSpeed = GameConfig.Instance.Config.player_data.speed;
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
        if (config != null && config.player_data != null)
        {
            moveSpeed = config.player_data.speed;
        }
    }

    private void FixedUpdate()
    {
        // Null checks for GameManager and dead state
        if (isDead || GameManager.Instance == null || GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(h, 0, v).normalized * moveSpeed*2;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        // Death check
        if (transform.position.y < -5f)
        {
            isDead = true;
            GameManager.Instance.GameOver();
        }
    }

    public void ResetDoofus(Vector3 position)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        isDead = false;
        transform.position = position;

        rb.linearVelocity = Vector3.zero;

        rb.angularVelocity = Vector3.zero;
    }
}