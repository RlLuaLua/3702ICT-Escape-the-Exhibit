using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    float damage = 1;

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<HealthController>())
        other.gameObject.GetComponent<HealthController>().ReceiveDamage(damage);
    }
}
