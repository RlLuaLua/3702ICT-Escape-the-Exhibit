using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public AudioSource death;
    public float max_health = 5;
    public float cur_health;
    public float start_health = 3;

    [SerializeField]
    float invulntimer;

    public float gracePeriod;
    public bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
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
            death.Play();
            return;
        }
        if (invulntimer > 0)
        {

            return; //dont deal damage if damage has been taken in set time
        }
        
        cur_health -= damageAmount;
        GetComponent<Animator>().Play("RecieveHit", -1);
        invulntimer += gracePeriod;//set timer for damage invulnerability

        Mathf.Clamp(cur_health, 0, max_health);
        }

    public void AddHealth(float healthAmount)
    {
        cur_health += healthAmount;

        Mathf.Clamp(cur_health, 0, max_health);
    }
}
