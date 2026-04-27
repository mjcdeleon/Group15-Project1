using UnityEngine;
using UnityEngine.InputSystem;

public class SelectMani : MonoBehaviour
{
    //starting vars/ ray set up

    public float rayDistance = 20f;
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
    private LineRenderer liner;

    void Awake()
    {
        liner = GetComponent<LineRenderer>();
        Debug.Log("LineRenderer found: " + (liner != null));
        if (liner != null)
        {
            liner.positionCount = 2;
            liner.startWidth = 0.01f;
            liner.endWidth = 0.01f;
            liner.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

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
        // Use negative forward to fix Y:180 rotation
        Vector3 rayDirection = -transform.forward;

        if (liner != null)
        {
            liner.enabled = true;
            liner.SetPosition(0, transform.position);
            liner.SetPosition(1, transform.position + rayDirection * rayDistance);
            liner.startColor = Color.red;
            liner.endColor = Color.red;
        }

        RaycastHit target;
        if (Physics.Raycast(transform.position, rayDirection, out target, rayDistance, canChoose))
        {
            if (liner != null)
            {
                liner.SetPosition(1, target.point);
                liner.startColor = Color.green;
                liner.endColor = Color.green;
            }

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
        if (liner != null)
        {
            liner.enabled = false;
        }
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

