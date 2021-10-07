using UnityEngine;

public class ShooterFSM : Interactable
{
    protected CharacterController controller;
    public enum FSMState
    {
        None,
        Patrol,
        Dead,
        Chase,
        Shoot
    }

    public FSMState curState;
    public GameObject[] waypointList;
    private Vector3 moveDirection;
    public int currentWaypoint = 0;
    public float slopeForce = 5.0f;
    public float slopeForceRayLength = 2.0f;
    private float moveSpeed;
    public float patrolMoveSpeed = 3.0f;
    public float chaseMoveSpeed = 5.0f;

    public float chaseRange = 5.0f;
    public float attackRange = 3.0f;
    public float health = 1f;
    public float damage = 1f;

    public float shootRate = 3.0f;
    protected float elapsedTime;

    public GameObject bullet;
    public GameObject bulletSpawnPoint;

    protected Transform playerTransform; // Player Transform

    // Start is called before the first frame update
    void Start()
    {
        curState = FSMState.Patrol;
        elapsedTime = 0.0f;
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
            case FSMState.Shoot: UpdateShootState(); break;
        }

        float playerDist = Vector3.Distance(transform.position, playerTransform.position);

        if (health > 0)
        {
            if (CanSeePlayer() && playerDist <= chaseRange && playerDist <= attackRange)
            {
                curState = FSMState.Shoot;
            }
            else if (CanSeePlayer() && playerDist <= chaseRange && playerDist >= attackRange)
            {
                curState = FSMState.Chase;
            }
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

        elapsedTime += Time.deltaTime;

        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;
        direction.z = 0;

        Debug.DrawRay(transform.position, direction);
    }

    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;
        direction.z = 0;
        return Physics.Raycast(transform.position, direction, out hit) && hit.transform.tag == "Player";
    }

    private void MoveForward()
    {
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
            playerTransform.gameObject.GetComponent<HealthController>().ReceiveDamage(damage);
        }
    }
    //if hit by spinattack lose 1 health
    public override void SpinInteract()
    {
        Debug.Log("hit");
        health -= 1;
    }

    protected void UpdatePatrolState()
    {
        //set move speed to patrol move speed
        moveSpeed = patrolMoveSpeed;
        transform.LookAt(new Vector3(waypointList[currentWaypoint].transform.position.x, transform.position.y, transform.position.z));

        // * Raycast from controller to ground to determine if on slope. If is on slope, increase downwards force to stop bumping down slope
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength);

        if (hit.normal != Vector3.up)
        {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

        if (Mathf.Abs(transform.position.x - waypointList[currentWaypoint].transform.position.x) <= 0.05)
        {
            if (currentWaypoint + 1 < waypointList.Length)
            {
                Debug.Log("called");
                currentWaypoint++;
            }
            else
            {
                currentWaypoint = 0;
            }
        }

        MoveForward();
    }

    protected void UpdateDeadState()
    {
        moveSpeed = 0f;
        Destroy(gameObject, 1.5f);
    }

    protected void UpdateChaseState()
    {
        //set move speed to chase move speed
        moveSpeed = chaseMoveSpeed;
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));

        // * Raycast from controller to ground to determine if on slope. If is on slope, increase downwards force to stop bumping down slope
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength);

        if (hit.normal != Vector3.up)
        {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

        MoveForward();
    }

    protected void UpdateShootState()
    {
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, transform.position.z));
        if (elapsedTime >= shootRate)
        {
            if ((bulletSpawnPoint) & (bullet))
            {
                // Shoot the bullet
                Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
            }
            elapsedTime = 0.0f;
        }
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

        if (curState == FSMState.Chase || curState == FSMState.Shoot)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}