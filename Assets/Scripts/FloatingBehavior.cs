using UnityEngine;

public class FloatingBehavior : MonoBehaviour
{
    public float hoverSpeed = 0.5f;
    public float hoverHeight = 0.5f;

    private Vector3 tempPos;
    private Vector3 posOffset;

    void Start()
    {
        posOffset = transform.position;
    }

    void Update()
    {
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * hoverSpeed) * hoverHeight;
        transform.position = tempPos;
    }
}

