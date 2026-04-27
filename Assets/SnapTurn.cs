using UnityEngine;
using UnityEngine.InputSystem;

public class SnapTurn : MonoBehaviour
{
    public InputActionReference turnJoystick; 
    public float turnAngle = 45f; 
    private bool turned = false;

    void OnEnable() => turnJoystick.action.Enable();
    void OnDisable() => turnJoystick.action.Disable();

    void Update()
    {
        float val = turnJoystick.action.ReadValue<Vector2>().x;

        // Simple threshold to prevent twitchy turning
        if (Mathf.Abs(val) > 0.8f)
        {
            if (!turned)
            {
                // If stick pushed right (val > 0), turn right; else turn left
                float direction = val > 0 ? 1 : -1;
                transform.Rotate(0, direction * turnAngle, 0);
                turned = true;
            }
        }
        else
        {
            turned = false; // Reset 
        }
    }
}
