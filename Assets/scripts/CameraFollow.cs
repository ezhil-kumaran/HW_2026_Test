using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Manager References")]
    [SerializeField] private Transform target;           // Doofus Transform
    [SerializeField] private PulpitManager pulpitManager; // Reference to find active platforms

    [Header("Camera Settings")]
    [SerializeField] private Vector3 defaultOffset = new Vector3(0f, 18f, -14f);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float minFieldOfView = 50f;
    [SerializeField] private float maxFieldOfView = 75f;
    [SerializeField] private float fovZoomSpeed = 3f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (pulpitManager == null)
        {
            pulpitManager = FindFirstObjectByType<PulpitManager>();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculate midpoint between Player and all active platforms
        Vector3 focusPoint = CalculateFocusCenter();

        // 2. Smoothly follow the focus point
        Vector3 desiredPosition = focusPoint + defaultOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Maintain fixed isometric pitch/angle
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);

        // 3. Dynamic Field-of-View scaling based on distance between platforms
        AdjustZoom(focusPoint);
    }

    private Vector3 CalculateFocusCenter()
    {
        Bounds bounds = new Bounds(target.position, Vector3.zero);

        // Encapsulate all active pulpits in the scene
        Pulpit[] pulpits = FindObjectsByType<Pulpit>(FindObjectsSortMode.None);
        foreach (var p in pulpits)
        {
            if (p != null)
            {
                bounds.Encapsulate(p.transform.position);
            }
        }

        return bounds.center;
    }

    private void AdjustZoom(Vector3 focusCenter)
    {
        if (cam == null || !cam.orthographic)
        {
            float maxDist = Vector3.Distance(target.position, focusCenter);
            float targetFOV = Mathf.Clamp(minFieldOfView + (maxDist * 1.5f), minFieldOfView, maxFieldOfView);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovZoomSpeed * Time.deltaTime);
        }
    }
}