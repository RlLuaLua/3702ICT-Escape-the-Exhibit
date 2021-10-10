using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    float damage = 1;

    // Update is called once per frame
    public void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
            GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>().ReceiveDamage(damage);
    }
}