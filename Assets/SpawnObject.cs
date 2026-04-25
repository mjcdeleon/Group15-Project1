using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnObject : MonoBehaviour
{
    //spawnables + actions
    public GameObject[] spawnables;
    private float spawnDistance = 1.5f;
    public InputActionReference rightTrigger;
    public InputActionReference rightGrip;
    public InputActionReference rightJoystick;

    //selection and manipulation vars
    private int chosenIX = 0;
    private GameObject chosenOJ;
    private bool move = false;
    private bool triggerOn = false;
    private bool joystickOn = false;

    public global::System.Single SpawnDistance { get => spawnDistance; set => spawnDistance = value; }
    public global::System.Single SpawnDistance1 { get => spawnDistance; set => spawnDistance = value; }

    void OnEnable()
    {
        rightTrigger.action.Enable();
        rightGrip.action.Enable();
        rightJoystick.action.Enable();
    }

    void OnDisable()
    {
        rightTrigger.action.Disable();
        rightGrip.action.Disable();
        rightJoystick.action.Disable();
    }

    void Update()
    {
        ChooseOJ();
        Spawning();
        ManipulationOJ();
    }

    //all methods / vars handling spawnable obects
    void Spawning()
    {
        float trigNum = rightTrigger.action.ReadValue<float>();
        bool touched = trigNum > 0.5f;

        if (touched && !triggerOn)
        {
            SpawnInObject();
        }

        if (!touched && triggerOn && move)
        {
            letGoOJ();
        }

        void SpawnInObject()
        {
            Vector3 PositionSP = transform.position + transform.forward * SpawnDistance;
            Quaternion RotationSP = transform.rotation;

            //fix bug: make sure when moving / rotation object it stays selected
            //double check fix below

            chosenOJ = Instantiate(spawnables[chosenIX], PositionSP, RotationSP);

            Rigidbody stuck = chosenOJ.GetComponent<Rigidbody>();
            if (stuck != null)
            {
                stuck.isKinematic = true;
            }
            //change move var
            move = true;
        }
    }

    void ManipulationOJ()
    {
        if (!move || spawnables == null)
        {
            return;
        }

        Vector2 moveStick = rightJoystick.action.ReadValue<Vector2>();
        float holdValue = rightGrip.action.ReadValue<float>();
        bool gripOn = holdValue > 0.5f;

        if (gripOn)
        {
            float speed = 90f;
            chosenOJ.transform.Rotate(0f, moveStick.x * speed * Time.deltaTime, 0f, Space.World);
        }
        else
        {
            float actionSpeed = 1.5f;
            chosenOJ.transform.position += new Vector3(moveStick.x * actionSpeed * Time.deltaTime, 0f, moveStick.y * actionSpeed * Time.deltaTime);
        }
    }

    void letGoOJ()
    {
        if (chosenOJ == null)
        {
            return;
        }

        Rigidbody falling = chosenOJ.GetComponent<Rigidbody>();
        if (falling != null)
        {
            falling.isKinematic = false;
        }

        chosenOJ = null;
        move = false;
    }

    void ChooseOJ()
    {
        if (move) //checking if there is already chosenOJ
        {
            return;
        }

        Vector2 chooseStick = rightJoystick.action.ReadValue<Vector2>();

        if (!joystickOn)
        {
            if (chooseStick.x > 0.5f)
            {
                chosenIX = Mathf.Min(chosenIX + 1, spawnables.Length - 1);
                joystickOn = true;
                Debug.Log("Selected: " + spawnables[chosenIX].name);
            }
            else if (chooseStick.x < -0.5f)
            {
                chosenIX = Mathf.Max(chosenIX - 1, 0);
                joystickOn = true;
                Debug.Log("Selected: " + spawnables[chosenIX].name);
            }
        }

        if (Mathf.Abs(chooseStick.x) < 0.2f)
        {
            joystickOn = false;
            //reset joystick after selection/ exit selection
        }

    }

}
