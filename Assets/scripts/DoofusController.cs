using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DoofusController : MonoBehaviour
{
    private Rigidbody rb;
    private float moveSpeed = 3.0f;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameConfig.Instance.OnConfigLoaded += OnConfigLoaded;
        if (GameConfig.Instance.Config != null)
            moveSpeed = GameConfig.Instance.Config.player_data.speed;
    }

    private void OnConfigLoaded(GameConfigData config)
    {
        moveSpeed = config.player_data.speed;
    }

    private void Update()
    {
        if (isDead || GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(h, 0, v).normalized * moveSpeed;

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
        isDead = false;
        transform.position = position;
        rb.linearVelocity = Vector3.zero;
    }
}
