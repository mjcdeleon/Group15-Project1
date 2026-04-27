using UnityEngine;
using UnityEngine.InputSystem;

public class Teleporter : MonoBehaviour
{
    public Transform xrOrigin; 
    public LayerMask groundLayer;
    
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        // This checks if the "Teleport" action (left Trigger) was released
        if (controls.Player.Teleport.WasReleasedThisFrame())
        {
            TryTeleport();
        }
    }

    void TryTeleport()
    {
        RaycastHit hit;
        // Raycast forward from this object (the controller)
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, groundLayer))
        {
            xrOrigin.position = hit.point;
        }
    }
}
