using UnityEngine;
using UnityEngine.InputSystem;

public class SelectMani : MonoBehaviour
{
    //starting vars/ ray set up

    public float rayDistance = 10f;
    public LayerMask canChoose;

    //highlight for selectable
    public Color highlight = Color.yellow;

    public InputActionReference rightTrigger;
    public InputActionReference rightGrip;


    private GameObject selectOJ;
    private Color regularShade;
    private Renderer curr;
    private bool triggerUsed = false;
    private bool gripUsed = false;
    private Quaternion controlStart;
    private Quaternion objectStart;

    void OnEnable()
    {
        rightTrigger.action.Enable();
        rightGrip.action.Enable();
    }

    void OnDisable()
    {
        rightTrigger.action.Disable();
        rightGrip.action.Disable();
    }

    void Update()
    { 
        if (rightTrigger.action == null || rightTrigger == null)
        {
            return;
        }
        if (rightGrip.action == null || rightGrip == null)
        {
            return;
        }
        var checkSpawn = GetComponent<SpawnObject>();
        if (checkSpawn != null && checkSpawn.move)
        {
            return;
        }

        bool triggerOn = rightTrigger.action.ReadValue<float>() > 0.1f;
        bool gripOn = rightGrip.action.ReadValue<float>() > 0.1f;

        if (gripOn || triggerOn)
        {
            if (selectOJ == null)
            {
                Selecting();
            }
            else
            {
                Manipulating(gripOn, triggerOn);
            }
        }
        else
        {
            if (selectOJ != null)
            {
                noSelecting();
            }
        }
    }

    void Selecting()
    {
        RaycastHit target;
        if (Physics.Raycast(transform.position, transform.forward, out target, rayDistance, canChoose))
        {
            GameObject chosenOne = target.collider.gameObject;
            selectOJ = chosenOne;
            curr = selectOJ.GetComponentInChildren<Renderer>();
            if (curr != null)
            {
                regularShade = curr.material.color;
                curr.material.color = highlight;
            }
            controlStart = transform.rotation;
            objectStart = selectOJ.transform.rotation;
        }
    }

    void noSelecting()
    {
        Rigidbody bodyOJ = selectOJ.GetComponent<Rigidbody>();
        if (bodyOJ != null)
        {
            bodyOJ.isKinematic = false;
            bodyOJ.useGravity = true;
            bodyOJ.WakeUp();
        }
        if (curr != null)
        {
            curr.material.color = regularShade;
        }
        selectOJ = null;
        curr = null;
    }

    void Manipulating(bool grip, bool trig)
    {
        if(selectOJ == null)
        {
            return;
        }
        if (grip && trig)
        {
            selectOJ.transform.localScale += Vector3.one * 0.2f * Time.deltaTime;
        }
        else if (grip)
        {
            Quaternion rotating = transform.rotation * Quaternion.Inverse(controlStart);
            selectOJ.transform.rotation = rotating * objectStart;
        }
        else if (trig)
        {
            selectOJ.transform.position = transform.position + (transform.forward * 3.0f);
        }
    }
}

