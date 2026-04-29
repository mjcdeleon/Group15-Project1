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
    private Quaternion controlStart;
    private Quaternion objectStart;
    private LineRenderer lineRenderer;

    private float gripHeldTime = 0f;
    private bool rotationApplied = false;

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
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }
    void UpdateRay()
    {
        if (lineRenderer == null) return;

        lineRenderer.SetPosition(0, transform.position);

        RaycastHit target;
        if (Physics.Raycast(transform.position, transform.forward, out target, rayDistance, canChoose))
        {
        
            lineRenderer.SetPosition(1, target.point);
        }
        else
        {
     
            lineRenderer.SetPosition(1, transform.position + transform.forward * rayDistance);
        }
    }

    void Update()
    {
        UpdateRay();
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

        bool triggerOn = rightTrigger.action.ReadValue<float>() > 0.5f;
        bool gripOn = rightGrip.action.ReadValue<float>() > 0.5f;

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

            Rigidbody rb = selectOJ.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            curr = selectOJ.GetComponentInChildren<Renderer>();
            if (curr != null)
            {
                regularShade = curr.material.color;
                curr.material.color = highlight;
            }
            controlStart = transform.rotation;
            objectStart = selectOJ.transform.rotation;
            selectOJ.transform.eulerAngles = new Vector3(
                0f,
                Mathf.Round(selectOJ.transform.eulerAngles.y / 90f) * 90f,
                0f
                );
        }
    }

    void noSelecting()
    {
        if (curr != null)
        {
            curr.material.color = regularShade;
        }

        // Unfreeze physics on release
        if (selectOJ != null)
        {
            Rigidbody rb = selectOJ.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        selectOJ = null;
        curr = null;
    }

    void Manipulating(bool grip, bool trig)
    {
        if (selectOJ == null) return;

        if (grip && trig)
        {
            selectOJ.transform.localScale += Vector3.one * 0.2f * Time.deltaTime;
        }
        else if (grip)
        {
            gripHeldTime += Time.deltaTime;

            if (gripHeldTime >= 0.5f && !rotationApplied)
            {
                Vector3 currentEuler = selectOJ.transform.eulerAngles;
                float currentY = currentEuler.y;

                // Snap to nearest 90 first, then step to next 90
                float snappedY = Mathf.Round(currentY / 90f) * 90f;
                float nextY = snappedY + 90f;

                selectOJ.transform.eulerAngles = new Vector3(0f, nextY, 0f);

                rotationApplied = true;
            }

            if (gripHeldTime >= 0.5f)
            {
                gripHeldTime = 0f;
                rotationApplied = false;
            }
        }
        else if (trig)
        {
            gripHeldTime = 0f;
            rotationApplied = false;
            selectOJ.transform.position = transform.position + (transform.forward * 3.0f);
        }
    }
}

