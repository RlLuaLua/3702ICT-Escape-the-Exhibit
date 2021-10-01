using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBehavior : MonoBehaviour
{
    public float rotationSpeed = 3.0f;

    void Update()
    {
        transform.Rotate(transform.rotation.x, rotationSpeed * Time.deltaTime, transform.rotation.z);
    }
}

