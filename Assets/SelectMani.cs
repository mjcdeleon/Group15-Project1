using UnityEngine;
using UnityEngine.InputSystem;

public class SelectMani : MonoBehaviour
{
    //starting vars/ ray set up

    public float rayDistance = 10f;
    public LayerMask canChoose;

    //highlight for selectable
    public Color highlight = Color.yellow;

    public InputActionReference leftTrigger;
    public InputActionReference leftGrip;
    public InputActionReference leftJoystick;

    public InputActionReference rightJoystick;

    private GameObject selectOJ;
    private Color regularShade;
    private bool triggerUsed = false;
    private bool gripUsed = false;

    void OnEnable()
    {
        leftTrigger.action.Enable();
        leftGrip.action.Enable();
        leftJoystick.action.Enable();
        rightJoystick.action.Enable();
    }

    void OnDisable()
    {
        leftTrigger.action.Disable();
        leftGrip.action.Disable();
        leftJoystick.action.Disable();
        rightJoystick.action.Disable();
    }

    void Update()
    {
        SelectFirstMethod();
        SelectSecondMethod();
        ManipulateFull();
    }

    void SelectFirstMethod()
    {
        float value = leftTrigger.action.ReadValue<float>();
        bool triggerOn = value > 0.5f;

        if(triggerOn && !triggerUsed)
        {
            SelectingOJ();
        }

        triggerUsed = triggerOn;
    }

    void SelectSecondMethod()
    {
        float gripVal = leftGrip.action.ReadValue<float>();
        bool gripOn = gripVal > 0.5f;

        if (gripOn && !gripUsed)
        {
            SelectingOJ();
        }
        gripUsed = gripOn;
    }

    void SelectingOJ()
    {
        RaycastHit wanted;

        if (Physics.Raycast(transform.position, transform.forward, out wanted, rayDistance, canChoose))
        { 
            GameObject wantedObject = wanted.collider.gameObject;


            //deselecting
            if (selectOJ == wantedObject)
            {
                Deselect();
                return;
            }

            if (selectOJ != null)
            {
                Deselect();
            }

            Select(wantedObject);
        }
        else
        {
            if(selectOJ != null)
            {
                Deselect();
            }
        }
    }

    void Select(GameObject objects)
    {
        selectOJ = objects;

        Renderer addHigh = objects.GetComponentInChildren<Renderer>();
        if (addHigh != null)
        {
            regularShade = addHigh.material.color;
            //adding highlighr
            addHigh.material.color = highlight;
        }

        Debug.Log("Selected: " + objects.name);
    }

    void Deselect()
    {
        if(selectOJ == null)
        {
            return;
        }

        Renderer colorChange = selectOJ.GetComponentInChildren<Renderer>();
        if(colorChange != null)
        {
            colorChange.material.color = regularShade;
        }

        selectOJ = null;
        Debug.Log("Deselected");
    }
 
    void ManipulateFull()
    {
        if(selectOJ == null)
        {
            return;
        }

        Vector2 leftSide = leftJoystick.action.ReadValue<Vector2>();
        Vector2 rightSide = rightJoystick.action.ReadValue<Vector2>();

        float gripVal = leftGrip.action.ReadValue<float>();
        float trigVal = leftTrigger.action.ReadValue<float>();

        bool gripOn = gripVal > 0.5f;
        bool triggerOn = trigVal > 0.5f;

        if(gripOn)
        {
            float rotateSP = 90f;
            selectOJ.transform.Rotate(0f, leftSide.x * rotateSP * Time.deltaTime, 0f, Space.World);
        }
        else if(triggerOn)
        {
            float moveSP = 1.5f;
            selectOJ.transform.position += new Vector3(leftSide.x * moveSP * Time.deltaTime, 0f, leftSide.y * moveSP * Time.deltaTime);
        }
    }
}
