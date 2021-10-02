using UnityEngine;

public class PlatformController : MonoBehaviour
{
    protected bool spin = false;
    protected bool move = false;
    protected float moveDistance;
    protected Vector3 moveTarget;
    public float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        if (spin) {
            transform.Rotate(transform.rotation.x, rotationSpeed * Time.deltaTime, transform.rotation.z);
        }
        if (move) {
            if (!(Vector3.Distance(transform.position, moveTarget) == 0.0f)) {
                transform.position = Vector3.MoveTowards(transform.position, moveTarget, Time.deltaTime);
            }
            else {
                move = false;
            }
        }
    }

    public void Interact(GameObject trigger) {
        if (trigger.name == "Lever1")
            Spin(trigger);
        if (trigger.name == "Lever2")
            Move(trigger);
    }
    void Move(GameObject trigger) {
        moveDistance = trigger.gameObject.GetComponent<LeverController>().isOn ? 2.0f : -2.0f;
        moveTarget = new Vector3(transform.position.x, transform.position.y - moveDistance, transform.position.z);
        move = true;
    }

    void Spin(GameObject trigger) {
        spin = !spin;
    }
}
