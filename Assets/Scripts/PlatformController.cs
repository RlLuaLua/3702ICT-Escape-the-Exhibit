using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{

    public GameObject trigger;
    public float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        if (trigger.GetComponent<LeverController>().isOn) {
            transform.Rotate(transform.rotation.x, rotationSpeed * Time.deltaTime, transform.rotation.z);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, trigger.transform.position);
    }
}
