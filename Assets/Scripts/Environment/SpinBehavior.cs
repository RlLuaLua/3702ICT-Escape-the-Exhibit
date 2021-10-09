using UnityEngine;

public class SpinBehavior : MonoBehaviour
{
    public float rotationSpeed = 30.0f;

    void Update()
    {
        transform.Rotate(transform.rotation.x, rotationSpeed * Time.deltaTime, transform.rotation.z);
    }
}