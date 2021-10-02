using UnityEngine;

public class PatrollerController : MonoBehaviour
{
    protected CharacterController controller;
    public enum FSMState
    {
        Patrol,
        Deactivate
    }

    public FSMState currentState;
    public GameObject[] waypointList;
    public int currentWaypoint = 0;
    public float speed = 2.0f;

    public float slopeForce = 5.0f;
    public float slopeForceRayLength = 2.0f;

    protected float timeElapsed = 0.0f;
    protected LeverController lever;
    public float interactionDuration = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentState = FSMState.Patrol;
    }

    // Update is called once per frame
    void Update()
    {
      switch (currentState) {
        case FSMState.Patrol: UpdatePatrolState(); break;
        case FSMState.Deactivate: UpdateDeactivateState(); break;
      }
    }

    protected void UpdatePatrolState() {
        if(Mathf.Abs(transform.position.x - waypointList[currentWaypoint].transform.position.x) <= 0.5f) {
            currentWaypoint++;
            if (currentWaypoint >= waypointList.Length) {
                currentWaypoint = 0;
            }
        }
        transform.LookAt(new Vector3(waypointList[currentWaypoint].transform.position.x, transform.position.y, transform.position.z));
        controller.SimpleMove(transform.forward * speed);
        // * Raycast from controller to ground to determine if on slope. If is on slope, increase downwards force to stop bumping down slope
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength);
        if (hit.normal != Vector3.up) {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }
    }

    protected void UpdateDeactivateState() {
        if (timeElapsed >= interactionDuration) {
            lever.gameObject.SendMessage("SpinInteract");
            currentState = FSMState.Patrol;
            timeElapsed = 0.0f;
        };
        timeElapsed += Time.deltaTime;

    }

    void OnTriggerEnter(Collider collider){
        lever = collider.gameObject.GetComponent<LeverController>();
        if (collider.gameObject.tag == "Lever" && lever.isOn == true) {
            currentState = FSMState.Deactivate;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.tag == "Player") {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
            currentWaypoint++;
            if (currentWaypoint >= waypointList.Length) {
                currentWaypoint = 0;
            }
        }
    }

    // Gizmos
    private void OnDrawGizmos() {
      Gizmos.color = Color.red;
    }
}
