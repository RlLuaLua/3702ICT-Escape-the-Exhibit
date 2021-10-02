using UnityEngine;

public class LeverController : Interactable
{
    public GameObject platform;
    public GameObject handle;
    public bool isOn;

    void Start()
    {
        isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        handle.transform.localRotation = Quaternion.Euler(0, 0, isOn ? -45 : 45);
    }

    public override void SpinInteract()
    {
        isOn = !isOn;
        platform.GetComponent<PlatformController>().Interact(gameObject);
            
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, platform.transform.position);
    }
}
