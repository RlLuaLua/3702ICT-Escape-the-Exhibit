using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float max_health
    {
        get; private set;
    }

    public float cur_health
    {
        get; private set;
    }

    [SerializeField]
    float start_health = 3;

    [SerializeField]
    float invulntimer;

    public float gracePeriod;
    public bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        max_health = 5;
        cur_health = start_health;
        invulntimer = 0;
    }

    void Update()
    {
        invulntimer -= Time.deltaTime;    //Subtract the time since last frame from the timer.
        if (invulntimer < 0)
        {
            invulntimer = 0;
        }
    }

    public void ReceiveDamage( float damageAmount )
    {
        if (!alive)
        {
            return;
        }
        if (invulntimer > 0)
        {
            Debug.Log(invulntimer);
            return; //dont deal damage if damage has been taken in set time
        }

        cur_health -= damageAmount;
        invulntimer += gracePeriod;//set timer for damage invulnerability

        Mathf.Clamp(cur_health, 0, max_health);
        
        Debug.Log(cur_health);
    }

    public void AddHealth(float healthAmount)
    {
        Debug.Log("Adding health " + healthAmount);
        cur_health += healthAmount;

        Mathf.Clamp(cur_health, 0, max_health);
    }
}
