using UnityEngine;

public class WallBreakerController : MonoBehaviour
{
    protected CharacterController controller;
    protected Animator animator;

    protected Transform player;
    public enum FSMState
    {
        Idle,
        Stunned,
        Charge,
        Defeat
    }

    public FSMState currentState = FSMState.Idle;

    private float damage = 2;
    public float chargeRange = 3.0f;
    public float chargeSpeed = 1.0f;
    public bool chargeStarted = false;
    private Vector3 moveDirection;
    public Vector3 chargeToPosition;
    public float stunTime = 5.0f;
    public float stunTimeElapsed = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.detectCollisions = false;
        animator = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
    }

    void Update()
    {
        switch (currentState) {
        case FSMState.Idle: UpdateIdleState(); break;
        case FSMState.Charge: UpdateChargeState(); break;
        case FSMState.Stunned: UpdateStunnedState(); break;
        case FSMState.Defeat: UpdateDefeatState(); break;
      }

      // State Switching Logic
      float distance = Vector3.Distance(transform.position, player.position);
      if (currentState != FSMState.Stunned && currentState != FSMState.Defeat) {
        if (distance <= chargeRange) {
            currentState = FSMState.Charge;
        }
        if (distance > chargeRange && !chargeStarted) {
            currentState = FSMState.Idle;
        }
      }

      // Animation
      animator.SetBool("chargeStarted", chargeStarted);

    }

    void UpdateIdleState() {
        chargeStarted = false;
    }

    void UpdateChargeState() {
        transform.LookAt(new Vector3(chargeToPosition.x, transform.position.y, 0));
        if(!chargeStarted) {
            chargeStarted = true;
            float over = Mathf.Sign(Vector3.Dot(transform.TransformDirection(Vector3.forward), new Vector3(player.position.x, 0, 0) - new Vector3(transform.position.x, 0, 0)));
            chargeToPosition = new Vector3(player.position.x + over * 2.0f, transform.position.y, 0);
        }
        if(Vector3.Distance(transform.position, chargeToPosition) <= 0.5f) {
            chargeStarted = false;
        }
        moveDirection = new Vector3(transform.forward.x * chargeSpeed, Vector3.zero.y, 0f);
        moveDirection.y += (Physics.gravity.y * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
        AdjustMoveOnSlope();

    }

    void UpdateStunnedState() {
        chargeStarted = false;
        animator.SetBool("isStunned", true);
        if (stunTimeElapsed >= stunTime) {
            currentState = FSMState.Idle;
            animator.SetBool("isStunned", false);
            stunTimeElapsed = 0.0f;
        }
        stunTimeElapsed += Time.deltaTime;
    }

    void UpdateDefeatState() {
        animator.Play("Die", -1);
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>(), transform.GetComponent<Collider>());
    }

    // Check the collision with the player
    void OnControllerColliderHit(ControllerColliderHit collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player.gameObject.GetComponent<HealthController>().ReceiveDamage(damage);
            currentState = FSMState.Stunned;
        }
        if (collider.gameObject.tag == "Breakable_Wall") {
            collider.gameObject.GetComponent<DestroyBreakableWallScript>().DestroyAndSpawnEmitter();
            currentState = FSMState.Defeat;
        }
        if (collider.gameObject.tag == "Side")
        {
            currentState = FSMState.Idle;
        }
        
    }

    // Raycast from controller to ground to determine if on slope. If is on slope, increase downwards force to smooth movement down slope
    void AdjustMoveOnSlope() {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * 2.0f);
        if (hit.normal != Vector3.up)
        {
            controller.Move(Vector3.down * controller.height / 2 * 5.0f * Time.deltaTime);
        }
    }

    void Destroy() {
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmos() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, chargeRange);
      Gizmos.color = Color.red;
      Gizmos.DrawLine(chargeToPosition, new Vector3(chargeToPosition.x, 0.5f, 0));
    }
}
