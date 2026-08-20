using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DoofusController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;
    private float moveSpeed = 3.0f;
    private bool isDead = false;

    // Hash parameter for better performance
    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsMovingParam = Animator.StringToHash("isMoving");

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Searches for Animator on this object or any child (e.g., Pig model)
        anim = GetComponentInChildren<Animator>();
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
        if (isDead || GameManager.Instance == null || GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            UpdateAnimation(0f);
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveInput = new Vector3(h, 0f, v);
        float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);

        Vector3 moveDirection = moveInput.normalized * (moveSpeed * 1.5f);
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);

        // Smoothly rotate the pig towards movement direction
        if (inputMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.fixedDeltaTime);
        }

        // Send speed to PigAnimatorController
        UpdateAnimation(inputMagnitude);

        // Fall check
        if (transform.position.y < -3f)
        {
            Die();
        }
    }

    private void UpdateAnimation(float magnitude)
    {
        if (anim != null)
        {
            anim.SetFloat(SpeedParam, magnitude);
            anim.SetBool(IsMovingParam, magnitude > 0.1f);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        UpdateAnimation(0f);

        // Halt physics to prevent infinite falling
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void ResetDoofus(Vector3 position)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        isDead = false;
        transform.position = position;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        UpdateAnimation(0f);
    }
}