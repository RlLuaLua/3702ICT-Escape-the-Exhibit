using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    float damage = 1;
    private HealthController player;

    void OnStart()
    {
        player = other.gameObject.GetComponent<HealthController>();
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if(player != null)
            player.ReceiveDamage(damage);
    }
}
