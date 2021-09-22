using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float max_health;
    float cur_health;
    [SerializeField]
    float invulntimer;
    public float gracePeriod;
    public bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        cur_health = max_health;
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
        Debug.Log(cur_health);
    }
}