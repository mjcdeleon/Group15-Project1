using UnityEngine;
using UnityEngine.InputSystem;

public class Teleporter : MonoBehaviour
{
    public Transform xrOrigin;
    public LayerMask groundLayer;
    public InputActionReference teleportAction;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // Keep it hidden until the user aims
    }

    void Update()
    {
        bool isHolding = teleportAction.action.ReadValue<float>() > 0.5f;

        if (isHolding)
        {
            lineRenderer.enabled = true;
            DrawTeleportRay();
        }
        else
        {
            lineRenderer.enabled = false;
        }

        if (teleportAction.action.WasReleasedThisFrame())
        {
            TryTeleport();
        }
    }

    void DrawTeleportRay()
    {
        RaycastHit hit;
        // Start the line at the controller
        lineRenderer.SetPosition(0, transform.position);

        // If it hits the ground, draw to the hit point
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, groundLayer))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // Otherwise, draw a line into the air to show where we are aiming
            lineRenderer.SetPosition(1, transform.position + (transform.forward * 10f));
        }
    }

    void TryTeleport()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, groundLayer))
        {
            xrOrigin.position = hit.point;
        }
    }
}
