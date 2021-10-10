using UnityEngine;

public class ShooterFSM : Interactable
{
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

        float playerDist = Vector3.Distance(transform.position, playerTransform.position);

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
        animator.Play("Walk", -1);
        
        moveSpeed = patrolMoveSpeed; //set move speed to patrol move speed
        transform.LookAt(new Vector3(waypointList[currentWaypoint].transform.position.x, transform.position.y, transform.position.z));
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
        MoveForward();
    }

    protected void UpdateChaseState()
    {
        if (transform.GetComponent<EdgeDetection>().isTouching) {
            animator.Play("Run", -1);
            //set move speed to chase move speed
            moveSpeed = chaseMoveSpeed;
            transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));
            MoveForward();
        }
        else {
            animator.Play("Taunt", -1);
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
            if (bulletSpawnPoint & bullet)
            {
                Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
            }
        }
    }

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
        Debug.Log("hit");
        health -= 1;
    }

    // Animation Event at end of "Die" animation triggers this GameObject to be destroyed
    void Destroy() {
        Destroy(gameObject, 1.5f);
    }

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