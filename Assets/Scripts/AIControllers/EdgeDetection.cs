using UnityEngine;

public class EdgeDetection : MonoBehaviour
{
    public bool isTouching;
    public bool isGrounded;
    public float distance = 0.5f;
    
    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position + transform.TransformDirection(new Vector3(0, 0f, 0.5f)), new Vector3(0, -1f, 0));
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        isTouching = Physics.Raycast(ray, distance);
        Ray ray2 = new Ray(transform.position + transform.TransformDirection(Vector3.forward), Vector3.down);
        Debug.DrawRay(ray2.origin, ray2.direction, Color.magenta);
        RaycastHit hit;
        isGrounded = Physics.Raycast(ray, out hit, 0.1f);
    }
}
