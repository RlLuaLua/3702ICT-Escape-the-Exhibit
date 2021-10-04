using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerFSM : MonoBehaviour
{
    protected CharacterController controller;
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
    public float attackRange = 3.0f;
    public float health = 1f;
    protected bool bDead;

    public float attackCoolDown;

    protected Transform playerTransform;// Player Transform
    protected Vector3 destPos; // Next destination position of the NPC Tank
    protected GameObject[] pointList;

    // Start is called before the first frame update
    void Start()
    {
        curState = FSMState.Patrol;
        bDead = false;
        controller = GetComponent<CharacterController>();
        Debug.Log("playerTransform.position");
        // Get the list of patrol points
        //pointList = GameObject.FindGameObjectsWithTag("PatrolPoint");

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
            else if (playerDist <= attackRange)
            {
                curState = FSMState.Attack;
            }
        }
        // Go to dead state if no health left
        else if (health <= 0)
        {
            curState = FSMState.Dead;
        }
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
    public virtual void SpinInteract()
    {
        Debug.Log("hit");
        health -= 1;
    }

    protected void UpdatePatrolState(){

    }

    protected void UpdateDeadState(){
        Debug.Log("dead");
    }

    protected void UpdateChaseState(){

    }

    protected void UpdateAttackState(){

    }
}