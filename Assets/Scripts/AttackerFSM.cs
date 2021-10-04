using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerFSM : MonoBehaviour
{
    public enum FSMState
    {
        None,
        Patrol,
        Dead,
        Chase,
        Attack
    }
    public FSMState curState;

    //ranges for State Changes
    public float moveSpeed = 12.0f;
    public float chaseRange = 12.0f;
    public float attackRange = 12.0f;
    public float health = 1f;

    public float attackCoolDown;

    protected Transform playerTransform;// Player Transform
    protected Vector3 destPos; // Next destination position of the NPC Tank
    protected GameObject[] pointList;

    // Start is called before the first frame update
    void Start()
    {
        curState = FSMState.Patrol;
        bDead = false;

        // Get the list of patrol points
        pointList = GameObject.FindGameObjectsWithTag("PatrolPoint");
        FindNextPoint();  // Set a random destination point first

        // Get the target (Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");
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

        if (health > 0) {
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
            // Enter attack state when inside attack range
            else if (playerDist <= attackRange && stopRange < playerDist)
            {
                curState = FSMState.Attack;
            }
        }
        // Go to dead state if no health left
        else if (health <= 0)
        {
            curState = FSMState.Dead;
        }

        //if hit by spinattack lose 1 health
        public virtual void SpinInteract()
        {
            health-=1;
        }

        // Check the collision with the player
        void OnCollisionEnter(Collision collision)
        {
            // Reduce health
            if (collision.gameObject.tag == "Player")
            {
                
            }
        }
    }
}
