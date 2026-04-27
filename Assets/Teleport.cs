using UnityEngine;
using UnityEngine.InputSystem;

public class Teleporter : MonoBehaviour
{
    public Transform xrOrigin;
    public LayerMask groundLayer;
    public InputActionReference teleportAction; // Use this slot in Inspector!

    void OnEnable() => teleportAction.action.Enable();
    void OnDisable() => teleportAction.action.Disable();

    void Update()
    {
        // Now it checks the specific action you drag into the Inspector
        if (teleportAction.action.WasReleasedThisFrame())
        {
            TryTeleport();
        }
    }

    void TryTeleport()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, groundLayer))
        {
            xrOrigin.position = hit.point;
        }
        else
        {
            Debug.Log("Teleport Raycast missed! Check your Ground Layer.");
        }
    }
}
