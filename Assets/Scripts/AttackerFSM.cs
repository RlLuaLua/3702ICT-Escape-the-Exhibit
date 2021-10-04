using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerFSM : Interactable
{
    protected CharacterController controller;
    public enum FSMState
    {
        None,
        Patrol,
        Dead,
        Chase
    }
    public FSMState curState;
    public GameObject[] waypointList;
    public int currentWaypoint = 0;
    public float slopeForce = 5.0f;
    public float slopeForceRayLength = 2.0f;

    //ranges for State Changes
    private float moveSpeed;
    public float patrolMoveSpeed = 3.0f;
    public float chaseMoveSpeed = 5.0f;
    public float attackMoveSpeed = 8.0f;
    public float attackJumpForce = 5.0f;
    private Vector3 moveDirection;
    
    public float chaseRange = 12.0f;
    public float attackRange = 3.0f;
    public float health = 1f;
    protected bool bDead;

    public float attackCoolDown;
    protected float elapsedTime;

    protected Transform playerTransform;// Player Transform
    protected Vector3 destPos; // Next destination position of the NPC Tank
    protected GameObject[] pointList;

    // Start is called before the first frame update
    void Start()
    {
        curState = FSMState.Patrol;
        bDead = false;
        controller = GetComponent<CharacterController>();

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
        }
        
        float playerDist = Vector3.Distance(transform.position, playerTransform.position);
        
        if (health > 0)
        {
            // Go to chase state if player is in range
            if (playerDist <= chaseRange && attackRange < playerDist)
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

        attackCoolDown -= Time.deltaTime;
        if (attackCoolDown < 0f){
            attackCoolDown=0f;
        }
        moveDirection = new Vector3(transform.forward.x * moveSpeed, moveDirection.y, 0f);
        moveDirection.y += (Physics.gravity.y * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
    }

    // Check the collision with the player
    void OnCollisionEnter(Collision collision)
    {
        // Reduce health
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("1");
        }
    }
    //if hit by spinattack lose 1 health
    public override void SpinInteract()
    {
        Debug.Log("hit");
        health -= 1;
    }

    protected void UpdatePatrolState(){
        //set move speed to patrol move speed
        moveSpeed = patrolMoveSpeed;
        transform.LookAt(new Vector3(waypointList[currentWaypoint].transform.position.x, transform.position.y, transform.position.z));
            // * Raycast from controller to ground to determine if on slope. If is on slope, increase downwards force to stop bumping down slope
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength);
            if (hit.normal != Vector3.up) {
                controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
            }
            if (Mathf.Abs(transform.position.x - waypointList[currentWaypoint].transform.position.x) <= 0.05) {
                if (currentWaypoint + 1 < waypointList.Length) {
                    Debug.Log("called");
                    currentWaypoint++;
                }else{
                    currentWaypoint = 0;
                }

            }
    }

    protected void UpdateDeadState(){
        moveSpeed=0f;
        
        Destroy(gameObject, 1.5f);
    }

    protected void UpdateChaseState(){
        //set move speed to chase move speed
        moveSpeed = chaseMoveSpeed;
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));

            // * Raycast from controller to ground to determine if on slope. If is on slope, increase downwards force to stop bumping down slope
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength);
            if (hit.normal != Vector3.up) {
                controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
            }
    }
}