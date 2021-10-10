using UnityEngine;

public class ShooterFSM : Interactable
{
    public AudioSource shot;
    protected CharacterController controller;
    protected Animator animator;
    protected Transform playerTransform; // Player Transform

    public enum FSMState
    {
        None,
        Patrol,
        Dead,
        Chase,
        Shoot
    }

    public FSMState curState;

    // Movement
    public GameObject[] waypointList;
    private Vector3 moveDirection;
    public int currentWaypoint = 0;
    public float slopeForce = 5.0f;
    public float slopeForceRayLength = 2.0f;
    private float moveSpeed;
    public float patrolMoveSpeed = 3.0f;
    public float chaseMoveSpeed = 5.0f;

    // State Ranges
    public float chaseRange = 5.0f;
    public float attackRange = 3.0f;

    // Health
    public float health = 1f;

    // Attack
    public float damage = 1f;
    public GameObject bullet;
    public GameObject bulletSpawnPoint;


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
            case FSMState.Shoot: UpdateShootState(); break;
        }

        // Check distance between player and this object
        float playerDist = Vector3.Distance(transform.position, playerTransform.position);

        /* If health is greater than 0:
            - If in line of sight and within chase and attack distance then enter Shoot state
            - Else if in line of sight, shoot animation is not playing, 
              player is inside chase range but outside attack range then enter Chase state
            - If Shoot animation is not playing and either not in line of sight or outside chase range
              then enter Patrol state
            If health is less than or equal to 0, enter Dead state
        */
        if (health > 0)
        {
            if (CanSeePlayer() && playerDist <= chaseRange && playerDist <= attackRange)
            {
                curState = FSMState.Shoot;
            }
            else if (CanSeePlayer() && !isAnimationStatePlaying("Shoot") && playerDist <= chaseRange && playerDist >= attackRange) 
            {
                curState = FSMState.Chase;
            }
            else if (!isAnimationStatePlaying("Shoot") && (!CanSeePlayer() || playerDist > chaseRange))
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

    protected void UpdatePatrolState()
    {
        animator.Play("Walk", -1); // Start playing Walk animation on the first layer it is found on
        moveSpeed = patrolMoveSpeed; //set move speed to patrol move speed
        /* Rotate to look at current waypoint, keeping the same y and z position*/
        transform.LookAt(new Vector3(waypointList[currentWaypoint].transform.position.x, transform.position.y, transform.position.z));
        /* If within 0.05 of current waypoint x position, if not end of waypoint list go to next in array
           else go to start of array
        */
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

        /* If will be touching ground then move forwards, otherwise go to start or end point depending
           on current target
        */
        if (GetComponent<EdgeDetection>().isTouching) {
            MoveForward();
        }
        else {
            if(currentWaypoint == 0) {
                transform.LookAt(new Vector3(waypointList[1].transform.position.x, transform.position.y, transform.position.z));
                currentWaypoint = 1;
            }
            else {
                transform.LookAt(new Vector3(waypointList[0].transform.position.x, transform.position.y, transform.position.z));
                currentWaypoint = 0;
            }
        }
    }

    protected void UpdateChaseState()
    {
        /* If will be touching ground in 0.05f then: */
        if (transform.GetComponent<EdgeDetection>().isTouching) {
            animator.Play("Run", -1); // Play Run animation on first layer it is found on
            moveSpeed = chaseMoveSpeed; //set move speed to chase move speed
            transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));
            MoveForward();
        }
        else {
            animator.Play("Taunt", -1); // Play Taunt animation on first layer it is found on
        }
    }

    protected void UpdateShootState()
    {
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));
        if (Mathf.Abs((transform.position.y - playerTransform.position.y)) > 0.05 && !isAnimationStatePlaying("Shoot")) {
            animator.Play("Taunt", -1);
        }
        else {
            animator.Play("Shoot", -1);
        }
    }
    protected void UpdateDeadState()
    {
        animator.Play("Die", -1);
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>(), transform.GetComponent<Collider>());
    }

    void Shoot() {
        if (curState == FSMState.Shoot) {
            if (bulletSpawnPoint & bullet) // bullet and spawn point exist
            {
                Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
                shot.Play();
            }
        }
    }

    /* Cast a Ray towards the player. If the ray collides with the Player then canSeePlayer is true. Return canSeePlayer*/
    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 direction = new Vector3(playerTransform.position.x, playerTransform.position.y + 0.5f, 0) - new Vector3(transform.position.x, transform.position.y + 0.5f, 0);
        direction.z = 0;
        bool canSeePlayer = Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), direction, out hit) && hit.collider.gameObject.tag == "Player";
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), direction, Color.cyan);
        return canSeePlayer;
    }

    private void MoveForward()
    {
        moveDirection = new Vector3(transform.forward.x * moveSpeed, moveDirection.y, 0f);
        moveDirection.y += (Physics.gravity.y * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * 2.0f);
        if (hit.normal != Vector3.up)
        {
            controller.Move(Vector3.down * controller.height / 2 * 5.0f * Time.deltaTime);
        }
    }

    // If hit by spinattack lose 1 health
    public override void SpinInteract()
    {
        health -= 1;
    }

    // Animation Event at end of "Die" animation triggers this GameObject to be destroyed
    void Destroy() {
        Destroy(gameObject, 1.5f);
    }

    // If current animation name matches parameter and that it not played one time, then animation
    // still playing.
    bool isAnimationStatePlaying(string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        foreach (var waypoint in waypointList)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(waypoint.transform.position, 0.5f);
        }
    }
}