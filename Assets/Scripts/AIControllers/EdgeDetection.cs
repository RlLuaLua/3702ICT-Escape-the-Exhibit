using UnityEngine;

public class EdgeDetection : MonoBehaviour
{
    public bool isTouching;
    
    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position + transform.TransformDirection(new Vector3(0, 0f, 0.5f)), new Vector3(0, -1f, 0));
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        isTouching = Physics.Raycast(ray, 1f);
    }
}
