using UnityEngine;

public class PlatformController : MonoBehaviour
{
    protected Actions action = Actions.None;
    protected enum Actions {
        Spin,
        Move,
        None
    };
    protected LeverController lever;
    protected Vector3 moveTarget;
    public float rotationSpeed = 50f;


    void Update()
    {
        switch (action) {
            case Actions.Spin: UpdateSpinState(); break;
            case Actions.Move: UpdateMoveState(); break;
            case Actions.None: break;
            default: Debug.Log("Unknown Action"); break;
        }
    }

    public void Interact(GameObject trigger) {
        lever = trigger.GetComponent<LeverController>();
        if (lever.gameObject.name == "Lever1") {
            action = lever.isOn ? Actions.Spin : Actions.None;
        }
        if (lever.gameObject.name == "Lever2" && lever.isOn && lever.timesInteracted == 1) {
            moveTarget = new Vector3(transform.position.x, transform.position.y - 2.0f, transform.position.z);
            action = Actions.Move;
        }
    }
    void UpdateMoveState() {
        if(Vector3.Distance(transform.position, moveTarget) == 0.0f) {
            action = Actions.None;
            if (lever.gameObject.name == "Lever2") {
                GameObject.Find("Golem").GetComponent<SwitchActivatorController>().canProceed = true;
            }
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, Time.deltaTime);
        }
    }

    void UpdateSpinState() {
        transform.Rotate(transform.rotation.x, rotationSpeed * Time.deltaTime, transform.rotation.z);
    }
}
