using UnityEngine;
using UnityEngine.AI;

public class WallBreakerController : MonoBehaviour
{
    private NavMeshAgent nav;
    protected Transform player;

    public enum FSMState
    {
        Idle,
        Stunned,
        Charge,
        Defeat
    }

    public FSMState currentState;

    protected bool chargeStarted = false;
    protected Vector3 chargeToPosition;

    public float chargeRange = 3.0f;
    private float damage = 3;

    private float waitTime = 1.5f;
    private float timeElapsed = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        // Select Wallbreaker for gizmos
        UnityEditor.Selection.activeGameObject = GameObject.FindGameObjectWithTag("WallBreaker");
        // Set wb starting state
        currentState = FSMState.Idle;
        // Get player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
        if (!player) {
            print("Player doesn't exist.. Please add one with Tag named 'Player'");
        }
    }

    // Update is called once per frame
    void Update()
    {
      switch (currentState) {
        case FSMState.Idle: UpdateIdleState(); break;
        case FSMState.Charge: UpdateChargeState(); break;
        case FSMState.Stunned: UpdateStunnedState(); break;
        case FSMState.Defeat: UpdateDefeatState(); break;
      }

      float distance = Vector3.Distance(transform.position, player.position);
      if (distance <= chargeRange && currentState != FSMState.Stunned && currentState != FSMState.Defeat) {
        currentState = FSMState.Charge;
      }
      if (distance > chargeRange) {
        currentState = FSMState.Idle;
      }
    }

    protected void UpdateIdleState() {

    }

    protected void UpdateChargeState() {
      if (!chargeStarted) {
        chargeStarted = true;
        chargeToPosition = player.position;
      }
      float distance = Vector3.Distance(transform.position, player.position);
      timeElapsed += Time.deltaTime;
      if (timeElapsed > waitTime) {
        nav.SetDestination(chargeToPosition);
        timeElapsed = 0.0f;
      }
      if (Vector3.Distance(transform.position, chargeToPosition) <= 0.5f) {
        chargeStarted = false;
      }
    }
    protected void UpdateStunnedState() {
      // TODO: Implement timer, check if player in range and change to charge or idle state
    }
    protected void UpdateDefeatState() {
      // TODO: Implement death animation
    }

    void OnTriggerEnter(Collider collider){
      nav.SetDestination(transform.position);
      nav.isStopped = true;
      currentState = FSMState.Stunned;
      if (collider.gameObject.tag == "Player") {
        collider.gameObject.SendMessage("ReceiveDamage", damage);
      }
      if (collider.gameObject.tag == "Breakable_Wall") {
        print("Hit Breakable Wall!");
        currentState = FSMState.Defeat;
        // TODO: Send message to wall to destroy???
      }
    }

    // This is a method for the player spin attack to send message to
    void ApplyStun() {
      currentState = FSMState.Stunned;
    }

    // Gizmos
    private void OnDrawGizmos() {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, chargeRange);
      Gizmos.color = Color.cyan;
      Gizmos.DrawWireSphere(chargeToPosition, 1.0f);
    }
}
