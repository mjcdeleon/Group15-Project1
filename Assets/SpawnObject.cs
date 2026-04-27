using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnObject : MonoBehaviour
{
    //spawnables + actions
    public GameObject[] spawnables;
    private float spawnDistance = 1.5f;
    public InputActionReference rightTrigger;
    public InputActionReference rightGrip;
    public InputActionReference rightA;

    //selection and manipulation vars
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
        if (rightA.action == null ||rightA == null)
        {
            return;
        }
        if (rightTrigger.action == null || rightTrigger == null)
        {
            return;
        }
        if (rightA.action.WasPressedThisFrame() && !move)
        {
            choosingOJ();
        }
        playerActions();
    }

    void choosingOJ()
    {
        chosenIX = (chosenIX + 1) % spawnables.Length;
        Debug.Log("Selected Item: " + spawnables[chosenIX].name);
    }

    void playerActions()
    {
        bool gripOn = rightGrip.action.ReadValue<float>() > 0.5f;
        bool triggerOn = rightTrigger.action.ReadValue<float>() > 0.5f;

        if (gripOn || triggerOn)
        {
            if (!move)
            {
                Spawning();
            }
            else
            {
                {
                    ManipulationOJ(gripOn, triggerOn);
                }
            }
        }
        else if (move)
        {
            letGoOJ();
        }
    }
    //all methods / vars handling spawnable obects
    void Spawning()
    {
        Vector3 PositionSP = transform.position + transform.forward * spawnDistance;
        chosenOJ = Instantiate(spawnables[chosenIX], PositionSP, transform.rotation);

        Rigidbody bodyOJ = chosenOJ.GetComponent<Rigidbody>();
        if (bodyOJ != null)
        {
            bodyOJ.isKinematic = true;
        }

        controlStart = transform.rotation;
        objectStart = chosenOJ.transform.rotation;
        move = true;
    }

    void ManipulationOJ(bool grip, bool trig)
    {
        if (chosenOJ == null)
        {
            return;
        }

        if (grip && trig)
        {
            chosenOJ.transform.localScale += Vector3.one * 0.2f * Time.deltaTime;
        }
        else if (grip)
        {
            Quaternion rotating = transform.rotation * Quaternion.Inverse(controlStart);
            chosenOJ.transform.rotation = objectStart * rotating;
        }
        else if (trig)
        {
            chosenOJ.transform.position = transform.position + (transform.forward * spawnDistance);
        }
    }

    void letGoOJ()
    {
        Rigidbody bodyOJ = chosenOJ.GetComponent<Rigidbody>();

        if (bodyOJ != null)
        {
            bodyOJ.isKinematic = false;
            bodyOJ.useGravity = true;
        }

        chosenOJ = null;
        move = false;
    }



}
