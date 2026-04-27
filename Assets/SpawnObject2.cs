using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnObjectDup : MonoBehaviour
{
    [Header("Spawnables")]
    public GameObject[] spawnables;
    public float spawnDistance = 1.5f;

    [Header("Input")]
    public InputActionReference rightTrigger;
    public InputActionReference rightGrip;
    public InputActionReference rightA;

    // State
    private int chosenIX = 0;
    private GameObject chosenOJ;
    public bool move = false;
    private Quaternion controlStart;
    private Quaternion objectStart;

    void OnEnable()
    {
        rightTrigger.action.Enable();
        rightGrip.action.Enable();
        rightA.action.Enable();
    }

    void OnDisable()
    {
        rightTrigger.action.Disable();
        rightGrip.action.Disable();
        rightA.action.Disable();
    }

    void Update()
    {
        // Fixed null check order
        if (rightA == null || rightA.action == null) return;
        if (rightTrigger == null || rightTrigger.action == null) return;
        if (rightGrip == null || rightGrip.action == null) return;

        if (rightA.action.WasPressedThisFrame() && !move)
        {
            ChoosingOJ();
        }

        PlayerActions();
    }

    void ChoosingOJ()
    {
        chosenIX = (chosenIX + 1) % spawnables.Length;
        Debug.Log("Selected: " + spawnables[chosenIX].name);
    }

    void PlayerActions()
    {
        bool gripOn = rightGrip.action.ReadValue<float>() > 0.5f;
        bool triggerOn = rightTrigger.action.ReadValue<float>() > 0.5f;

        if (gripOn || triggerOn)
        {
            if (!move)
            {
                Spawning(gripOn, triggerOn);
            }
            else
            {
                ManipulationOJ(gripOn, triggerOn);
            }
        }
        else if (move)
        {
            LetGoOJ();
        }
    }

    void Spawning(bool grip, bool trig)
    {
        // Flatten forward vector to fix eye height issue
        Vector3 flatForward = new Vector3(
            transform.forward.x,
            0f,
            transform.forward.z
        ).normalized;

        Vector3 spawnPos = transform.position + flatForward * spawnDistance;

        // Keep spawn at a consistent height instead of eye level
        spawnPos.y = transform.position.y - 0.3f;

        chosenOJ = Instantiate(spawnables[chosenIX], spawnPos, transform.rotation);

        Rigidbody rb = chosenOJ.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Disable gravity and physics while holding
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        controlStart = transform.rotation;
        objectStart = chosenOJ.transform.rotation;
        move = true;
    }

    void ManipulationOJ(bool grip, bool trig)
    {
        if (chosenOJ == null) return;

        if (grip && trig)
        {
            // Both held — scale up
            chosenOJ.transform.localScale += Vector3.one * 0.2f * Time.deltaTime;
        }
        else if (grip)
        {
            // Grip only — rotate
            Quaternion rotating = transform.rotation * Quaternion.Inverse(controlStart);
            chosenOJ.transform.rotation = rotating * objectStart; // Fixed order
        }
        else if (trig)
        {
            // Trigger only — move/follow controller
            Vector3 flatForward = new Vector3(
                transform.forward.x,
                0f,
                transform.forward.z
            ).normalized;

            Vector3 targetPos = transform.position + flatForward * spawnDistance;
            targetPos.y = transform.position.y - 0.3f;

            // Smooth follow instead of snapping
            chosenOJ.transform.position = Vector3.Lerp(
                chosenOJ.transform.position,
                targetPos,
                Time.deltaTime * 10f
            );
        }
    }

    void LetGoOJ()
    {
        if (chosenOJ == null) return;

        Rigidbody rb = chosenOJ.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Re-enable gravity and physics on release
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        chosenOJ = null;
        move = false;
    }
}