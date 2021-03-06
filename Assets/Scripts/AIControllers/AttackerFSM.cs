using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerFSM : Interactable
{
    protected CharacterController controller;
    protected Animator animator;
    protected Transform playerTransform;// Player Transform
    
    public enum FSMState
    {
        None,
        Patrol,
        Dead,
        Chase,
        Attack
    }

    public FSMState curState;
    public GameObject[] waypointList;
    public int currentWaypoint = 0;

    private Vector3 moveDirection;
    private float moveSpeed;
    public float patrolMoveSpeed = 3.0f;
    public float chaseMoveSpeed = 5.0f;
    public float slopeForce = 5.0f;
    public float slopeForceRayLength = 2.0f;

    // Range triggers for FSM States
    public float chaseRange = 12.0f;
    public float attackRange = 0.35f;
    public float health = 1f; // Health of the Attacker AI
    protected float elapsedTime = 0.0f; // Time elapsed since Sense animation start
    public float damageAmount = 1.0f; // Amount of damage enemy causes player


    // Start is called before the first frame update
    void Start()
    {
        curState = FSMState.Patrol;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Get the target (Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        if (!playerTransform)
            Debug.Log("Player doesn't exist.. Please add one with Tag named 'Player'");
    }

    // Update is called once per frame
    void Update()
    {
        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
        }

        float playerDist = Vector3.Distance(transform.position, playerTransform.position);

        if (health > 0)
        {
            // Go to chase state if player is in range and in line of sight
            if (CanSeePlayer() && playerDist <= chaseRange && playerDist > attackRange)
            {
                curState = FSMState.Chase;
            }
            // Return to patrol state after the player leaves the chase range or line of sight
            else if (!CanSeePlayer() || playerDist > chaseRange)
            {
                curState = FSMState.Patrol;
            }
        }
        // Go to dead state if no health left
        else if (health <= 0)
        {
            curState = FSMState.Dead;
        }

        if(GetComponent<EdgeDetection>().isGrounded){
            moveDirection.y = -0.1f;
        }
    }

    // Check the collision with the player
    void OnControllerColliderHit(ControllerColliderHit collider)
    {
        // Go to attack state if collide with Player
        if (collider.gameObject.tag == "Player")
        {
            curState = FSMState.Attack;
        }
        // Go to next or beginning waypoint if hit obstacle with "Side" tag
        if (collider.gameObject.tag == "Side") {
            if (currentWaypoint + 1 < waypointList.Length) { currentWaypoint++; } else { currentWaypoint = 0; }
        }
    }
    // If hit by Spin Attack, lose 1 health
    public override void SpinInteract()
    {
        health -= 1;
    }

    protected void UpdatePatrolState()
    {
        animator.SetBool("playerSeen", false); // If in patrol state, set playerSeen animation condition to false
        moveSpeed = patrolMoveSpeed; //set move speed to patrol move speed
        transform.LookAt(new Vector3(waypointList[currentWaypoint].transform.position.x, transform.position.y, transform.position.z));
        AdjustMoveOnSlope();
        if (Mathf.Abs(transform.position.x - waypointList[currentWaypoint].transform.position.x) <= 0.05)
        {
            if (currentWaypoint + 1 < waypointList.Length)
            {
                currentWaypoint++;
            }
            else
            {
                currentWaypoint = 0;
            }
        }
        Move();
    }

    protected void UpdateChaseState()
    {
        /* If Sense animation has not been played (i.e. playerSeen is false), turn in direction of the player, play animation. 
           After the animation has played, set playerSeen to true and reset timeElapsed. 
           If Sense animation has been played, set moveSpeed to chaseSpeed. If will be touching ground in 0.5 then move character
           else play Taunt animation
        */
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));
        if (!animator.GetBool("playerSeen"))
        {
            animator.Play("Sense", -1);
            if (elapsedTime > animator.GetCurrentAnimatorStateInfo(0).length)
            {
                animator.SetBool("playerSeen", true);
                elapsedTime = 0;
            }
            elapsedTime += Time.deltaTime;
        }
        else
        {
            moveSpeed = chaseMoveSpeed; //set move speed to chase move speed
            if (GetComponent<EdgeDetection>().isTouching) {
                Move();
            }
            else {
                animator.Play("Taunt", -1);
            }
        }
    }

    // Rotate towards player and play attack animation. The attack animation has an Animation Event which will
    // trigger DamagePlayer() when the animation reaches where the head of character hits the player
    protected void UpdateAttackState()
    {
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));
        animator.Play("Attack", -1);
    }

    // Play die animation turn off collision with Player character, then destroy after a delay
    protected void UpdateDeadState()
    {
        animator.Play("Die", -1);
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>(), transform.GetComponent<Collider>());
        Destroy(gameObject, 1.5f);
    }

    /* Cast a Ray directly towards the player x position. If the ray collides with the Player then canSeePlayer is true. Return canSeePlayer*/
    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 direction = new Vector3(playerTransform.position.x, transform.position.y + 0.5f, 0) - new Vector3(transform.position.x, transform.position.y + 0.5f, 0);
        direction.z = 0;
        bool canSeePlayer = Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), direction, out hit, chaseRange) && hit.collider.gameObject.tag == "Player";
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), direction, Color.cyan);
        return canSeePlayer;
    }

    protected void Move()
    {
        if (transform.GetComponent<EdgeDetection>().isTouching) {
            animator.Play("Walk", -1);
            moveDirection = new Vector3(transform.forward.x * moveSpeed, moveDirection.y, 0f);
            moveDirection.y += (Physics.gravity.y * Time.deltaTime);
            controller.Move(moveDirection * Time.deltaTime);
            AdjustMoveOnSlope();
        }
        else {
            if (currentWaypoint == 0) {
                transform.LookAt(new Vector3(waypointList[1].transform.position.x, transform.position.y, transform.position.z));
                currentWaypoint = 1;
            }
            else {
                transform.LookAt(new Vector3(waypointList[0].transform.position.x, transform.position.y, transform.position.z));
                currentWaypoint = 0;
            }
        }
    }

    // Raycast from controller to ground to determine if on slope. If is on slope, increase downwards force to smooth movement down slope
    void AdjustMoveOnSlope() {
        
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength);
        if (hit.normal != Vector3.up)
        {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

    }

    // Find the HealthController on the Player GameObject and call the ReceiveDamage() function
    // passing the damage caused by this character
    protected void DamagePlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>().ReceiveDamage(damageAmount);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}