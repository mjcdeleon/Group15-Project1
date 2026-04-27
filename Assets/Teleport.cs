using UnityEngine;
using UnityEngine.InputSystem;

public class Teleporter : MonoBehaviour
{
    public Transform xrOrigin;
    public InputActionReference teleportAction;

    public LayerMask groundLayer;
    public float arcSpeed = 8f;
    public float arcTimeStep = 0.05f;
    public int arcResolution = 30;

    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    private LineRenderer lineRenderer;
    private Vector3 validHitPoint;
    private bool hasValidTarget;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = arcResolution;
    }

    void Update()
    {
        bool isHolding = teleportAction.action.ReadValue<float>() > 0.5f;

        if (isHolding)
        {
            lineRenderer.enabled = true;
            DrawArc();
        }
        else
        {
            lineRenderer.enabled = false;
            hasValidTarget = false;
        }

        if (teleportAction.action.WasReleasedThisFrame() && hasValidTarget)
        {
            TryTeleport();
        }
    }

    void DrawArc()
    {
        Vector3 pos = transform.position;
        Vector3 velocity = transform.forward * arcSpeed;
        hasValidTarget = false;
        int validPointCount = arcResolution;

        for (int i = 0; i < arcResolution; i++)
        {
            lineRenderer.SetPosition(i, pos);

            Vector3 nextVelocity = velocity + Physics.gravity * arcTimeStep;
            Vector3 nextPos = pos + velocity * arcTimeStep;

            // Check for ground collision along this arc segment
            if (Physics.Raycast(pos, velocity.normalized, out RaycastHit hit,
                (nextPos - pos).magnitude, groundLayer))
            {
                validHitPoint = hit.point;
                hasValidTarget = true;
                validPointCount = i + 1;

                // Fill remaining line positions at the hit point
                for (int j = i + 1; j < arcResolution; j++)
                    lineRenderer.SetPosition(j, validHitPoint);

                break;
            }

            pos = nextPos;
            velocity = nextVelocity;
        }

        // Color the line green if valid, red if not
        lineRenderer.startColor = hasValidTarget ? validColor : invalidColor;
        lineRenderer.endColor = hasValidTarget ? validColor : invalidColor;
    }

    void TryTeleport()
    {
        // Offset the XR origin so the camera stays at head height above the target
        Vector3 cameraWorldPos = Camera.main.transform.position;
        Vector3 rigToCamera = new Vector3(
            cameraWorldPos.x - xrOrigin.position.x,
            0f,
            cameraWorldPos.z - xrOrigin.position.z
        );

        // Place the rig so feet land on the hit point, not the head
        xrOrigin.position = validHitPoint - rigToCamera;
    }
}