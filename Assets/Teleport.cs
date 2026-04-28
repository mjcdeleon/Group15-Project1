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
    private bool wasHolding = false;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = arcResolution;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        bool isHolding = teleportAction.action.ReadValue<Vector2>().magnitude > 0.5f;

        if (isHolding)
        {
            lineRenderer.enabled = true;
            DrawArc();
        }
        else
        {
            // ADD THIS - if we WERE holding last frame but not now, teleport!
            if (wasHolding && hasValidTarget)
            {
                Debug.Log("Teleporting!");
                TryTeleport();
            }
            lineRenderer.enabled = false;
            hasValidTarget = false;
        }

        wasHolding = isHolding; // track last frame
    }

    void DrawArc()
    {
        Vector3 pos = transform.position;
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 velocity = new Vector3(camForward.x, 0f, camForward.z).normalized * arcSpeed;
        hasValidTarget = false;
        int validPointCount = arcResolution;

        for (int i = 0; i < arcResolution; i++)
        {
            lineRenderer.SetPosition(i, pos);
            Vector3 nextVelocity = velocity + Physics.gravity * arcTimeStep;
            Vector3 nextPos = pos + velocity * arcTimeStep;

            if (Physics.Raycast(pos, velocity.normalized, out RaycastHit hit,
                (nextPos - pos).magnitude, groundLayer))
            {
                // ADD THIS LINE INSIDE HERE
                Debug.Log("Hit: " + hit.collider.gameObject.name + " layer: " + hit.collider.gameObject.layer);

                validHitPoint = hit.point;
                hasValidTarget = true;
                validPointCount = i + 1;
                for (int j = i + 1; j < arcResolution; j++)
                    lineRenderer.SetPosition(j, validHitPoint);
                break;
            }

            pos = nextPos;
            velocity = nextVelocity;
        }

        lineRenderer.startColor = hasValidTarget ? validColor : invalidColor;
        lineRenderer.endColor = hasValidTarget ? validColor : invalidColor;
    }

    void TryTeleport()
    {
        Debug.Log("Teleporting to: " + validHitPoint);
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