using UnityEngine;

public class SwitchActivatorController : MonoBehaviour
{
    protected CharacterController controller;
    public enum FSMState
    {
        Wait,
        Move,
        Activate
    }

    public FSMState currentState;
    public GameObject[] waypointList;
    public int currentWaypoint = 0;
    public float speed = 2.0f;

    public float slopeForce = 5.0f;
    public float slopeForceRayLength = 2.0f;

    public bool canProceed = true;
    public LeverController lever;
    protected float timeElapsed;
    protected float interactionDuration;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentState = FSMState.Move;
    }

    // Update is called once per frame
    void Update()
    {
      switch (currentState) {
        case FSMState.Wait: UpdateWaitState(); break;
        case FSMState.Move: UpdateMoveState(); break;
        case FSMState.Activate: UpdateActivateState(); break;
      }
    }

    void UpdateWaitState() {
        if(canProceed)
        {
            currentState = FSMState.Move;
        }
    }

    void UpdateActivateState() {
        if (timeElapsed >= interactionDuration) {
            lever.gameObject.SendMessage("ActivatorInteract");
            currentState = FSMState.Wait;
            timeElapsed = 0.0f;
        };
        timeElapsed += Time.deltaTime;
    }

    void UpdateMoveState() {
        if(canProceed) {
            transform.LookAt(new Vector3(waypointList[currentWaypoint].transform.position.x, transform.position.y, transform.position.z));
            controller.SimpleMove(transform.forward * speed);
            // * Raycast from controller to ground to determine if on slope. If is on slope, increase downwards force to stop bumping down slope
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength);
            if (hit.normal != Vector3.up) {
                controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
            }
            if (Mathf.Abs(transform.position.x - waypointList[currentWaypoint].transform.position.x) <= 0.05) {
                canProceed = false;
                if (currentWaypoint + 1 <= waypointList.Length) {
                currentWaypoint++;
                }
                currentState = FSMState.Wait;
            }
        }
    }

    void OnTriggerEnter(Collider collider) {
        lever = collider.gameObject.GetComponent<LeverController>();
        if (!lever.isOn) {
            if (collider.gameObject.tag == "Lever" && lever.requireActivator) {
                currentState = FSMState.Activate;
            }
        }
    }

    // Gizmos
    private void OnDrawGizmos() {
      Gizmos.color = Color.red;
    }
}
