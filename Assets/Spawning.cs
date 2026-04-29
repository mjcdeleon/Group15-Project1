using UnityEngine;
using UnityEngine.InputSystem;

public class Spawning : MonoBehaviour
{
    [Header("Spawnables")]
    public GameObject[] spawnables;
    public float spawnDistance = 1.5f;

    [Header("Input")]
    public InputActionReference leftTrigger;
    public InputActionReference leftGrip;
    public InputActionReference leftThumbstick; // NEW

    [Header("Joystick Settings")]
    public float rotateSpeed = 90f;       // degrees per second
    public float minDistance = 0.5f;
    public float maxDistance = 5f;

    // State
    private int chosenIX = 0;
    private GameObject chosenOJ;
    private bool isHolding = false;
    private Quaternion controlStart;
    private Quaternion objectStart;
    private float currentDistance;        // NEW — tracks how far object is from hand

    // Press detection
    private bool triggerWasPressed = false;
    private bool gripWasPressed = false;

    void OnEnable()
    {
        leftTrigger.action.Enable();
        leftGrip.action.Enable();
        leftThumbstick.action.Enable();   // NEW
    }

    void OnDisable()
    {
        leftTrigger.action.Disable();
        leftGrip.action.Disable();
        leftThumbstick.action.Disable();  // NEW
    }

    void Update()
    {
        if (leftTrigger == null || leftTrigger.action == null) return;
        if (leftGrip == null || leftGrip.action == null) return;

        bool triggerDown = leftTrigger.action.ReadValue<float>() > 0.5f;
        bool gripDown = leftGrip.action.ReadValue<float>() > 0.5f;

        // Trigger: cycle selection (only when not holding)
        if (triggerDown && !triggerWasPressed && !isHolding)
        {
            CycleSelection();
        }

        // Grip: spawn on fresh press
        if (gripDown && !gripWasPressed && !isHolding)
        {
            SpawnObject();
        }

        // Grip held: manipulate
        if (gripDown && isHolding)
        {
            ManipulateObject(triggerDown);
        }

        // Grip released: let go
        if (!gripDown && isHolding)
        {
            LetGoObject();
        }

        triggerWasPressed = triggerDown;
        gripWasPressed = gripDown;
    }

    void CycleSelection()
    {
        if (spawnables == null || spawnables.Length == 0) return;
        chosenIX = (chosenIX + 1) % spawnables.Length;
        Debug.Log($"Selected: {spawnables[chosenIX].name} ({chosenIX + 1}/{spawnables.Length})");
    }

    void SpawnObject()
    {
        if (spawnables == null || spawnables.Length == 0) return;

        currentDistance = spawnDistance; // start at default distance
        Vector3 spawnPos = GetHoldPosition();
        spawnPos.y += 0.1f;

        chosenOJ = Instantiate(spawnables[chosenIX], spawnPos, transform.rotation);
        Collider[] colliders = chosenOJ.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            
            BoxCollider bc = chosenOJ.AddComponent<BoxCollider>();

            
            Renderer[] renderers = chosenOJ.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                foreach (Renderer r in renderers)
                {
                    bounds.Encapsulate(r.bounds);
                }
                bc.center = chosenOJ.transform.InverseTransformPoint(bounds.center);
                bc.size = chosenOJ.transform.InverseTransformVector(bounds.size);
            }
        }
        foreach (Collider col in colliders)
        {
            // Fix concave mesh colliders by making them convex
            MeshCollider mc = col as MeshCollider;
            if (mc != null)
            {
                mc.convex = true;
            }
        }
        Rigidbody rb = chosenOJ.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = chosenOJ.AddComponent<Rigidbody>();
        }


        rb.isKinematic = true;
        rb.useGravity = false;
        controlStart = transform.rotation;
        objectStart = chosenOJ.transform.rotation;
        isHolding = true;
    }

    void ManipulateObject(bool triggerDown)
    {
        if (chosenOJ == null) return;

        Vector2 stick = leftThumbstick != null ? leftThumbstick.action.ReadValue<Vector2>() : Vector2.zero;

        if (triggerDown)
        {
            // Grip + Trigger: scale up
            chosenOJ.transform.localScale += Vector3.one * 0.2f * Time.deltaTime;
        }
        else
        {
            // Object position = exactly where the controller is in space + a fixed offset forward
            chosenOJ.transform.position = Vector3.Lerp(
                chosenOJ.transform.position,
                transform.position + transform.forward * currentDistance,
                Time.deltaTime * 20f
            );

            // Joystick rotates the object independently
            if (Mathf.Abs(stick.x) > 0.1f)
            {
                chosenOJ.transform.Rotate(Vector3.up, stick.x * rotateSpeed * Time.deltaTime, Space.World);
            }
            if (Mathf.Abs(stick.y) > 0.1f)
            {
                chosenOJ.transform.Rotate(Vector3.right, -stick.y * rotateSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    Vector3 GetHoldPosition()
    {
        return transform.position + transform.forward * currentDistance;
    }

    void LetGoObject()
    {
        if (chosenOJ == null) return;

        Rigidbody rb = chosenOJ.GetComponent<Rigidbody>();
        if(rb == null)
        {
            rb = chosenOJ.AddComponent<Rigidbody>();
        }
     
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.isKinematic = false;
        rb.useGravity = true;
        

        chosenOJ = null;
        isHolding = false;
    }

}