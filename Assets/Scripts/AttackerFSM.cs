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
            // Go to chase state if player is in range
            if (playerDist <= chaseRange && playerDist > attackRange)
            {
                curState = FSMState.Chase;
            }
            // Return to patrol state after the player leaves the chase range
            else if (playerDist > chaseRange)
            {
                curState = FSMState.Patrol;
            }
        }
        // Go to dead state if no health left
        else if (health <= 0)
        {
            curState = FSMState.Dead;
        }
    }

    // Check the collision with the player
    void OnControllerColliderHit(ControllerColliderHit collider)
    {
        // Reduce health
        if (collider.gameObject.tag == "Player")
        {
            curState = FSMState.Attack;
        }
    }
    // If hit by Spin Attack, lose 1 health
    public override void SpinInteract()
    {
        Debug.Log("hit");
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
           If Sense animation has been played, set moveSpeed to chaseSpeed and move character*/
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
            Move();
        }
    }

    protected void UpdateAttackState()
    {
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));
        animator.Play("Attack", -1);
    }

    protected void UpdateDeadState()
    {
        animator.Play("Die", -1);
        moveSpeed = 0f;
        Destroy(gameObject, 1.5f);
    }

    protected void Move()
    {
        animator.Play("Walk", -1);
        moveDirection = new Vector3(transform.forward.x * moveSpeed, moveDirection.y, 0f);
        moveDirection.y += (Physics.gravity.y * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
        AdjustMoveOnSlope();
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