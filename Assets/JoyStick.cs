using UnityEngine;
using UnityEngine.InputSystem;

public class JoyStick : MonoBehaviour
{
    [Header("References")]
    public Transform xrOrigin;
    public Transform cameraTransform;        // Main Camera
    public GameObject movementIndicator;     // Arrow/circle showing move direction
    public GameObject orientationIndicator;  // Arrow showing where you're facing

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float deadzone = 0.2f;

    [Header("Input Actions")]
    public InputActionReference moveJoystick;   // Left joystick - movement
    public InputActionReference turnJoystick;   // Right joystick - orientation

    private Vector2 moveInput;
    private Vector2 turnInput;

    void OnEnable()
    {
        moveJoystick.action.Enable();
        turnJoystick.action.Enable();
    }

    void OnDisable()
    {
        moveJoystick.action.Disable();
        turnJoystick.action.Disable();
    }

    void Update()
    {
        moveInput = moveJoystick.action.ReadValue<Vector2>();
        turnInput = turnJoystick.action.ReadValue<Vector2>();

        HandleMovement();
        HandleOrientation();
        UpdateIndicators();
    }

    void HandleMovement()
    {
        // Ignore input within deadzone
        if (moveInput.magnitude < deadzone) return;

        // Move relative to where the camera is facing (not the rig)
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Flatten to ground plane so you don't fly up/down
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);
        xrOrigin.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void HandleOrientation()
    {
        // Ignore input within deadzone
        if (turnInput.magnitude < deadzone) return;

        // Rotate the rig based on right joystick horizontal axis
        float turnAmount = turnInput.x * 60f * Time.deltaTime;
        xrOrigin.Rotate(0f, turnAmount, 0f);
    }

    void UpdateIndicators()
    {
        // Movement indicator — shows direction of travel
        if (movementIndicator != null)
        {
            if (moveInput.magnitude > deadzone)
            {
                movementIndicator.SetActive(true);

                // Point the indicator in the move direction
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);

                if (moveDirection != Vector3.zero)
                    movementIndicator.transform.rotation = Quaternion.LookRotation(moveDirection);

                // Place indicator in front of player on the ground
                movementIndicator.transform.position = new Vector3(
                    xrOrigin.position.x + moveDirection.x,
                    xrOrigin.position.y,
                    xrOrigin.position.z + moveDirection.z
                );
            }
            else
            {
                movementIndicator.SetActive(false);
            }
        }

        // Orientation indicator — shows which way the player is facing
        if (orientationIndicator != null)
        {
            // Always show, rotate with the rig
            orientationIndicator.transform.position = new Vector3(
                xrOrigin.position.x,
                xrOrigin.position.y,
                xrOrigin.position.z
            );

            orientationIndicator.transform.rotation = Quaternion.Euler(
                0f,
                xrOrigin.eulerAngles.y,
                0f
            );
        }
    }
}