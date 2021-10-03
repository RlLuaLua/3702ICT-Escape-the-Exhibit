using UnityEngine;

public class LeverController : Interactable
{
    public GameObject platform;
    public GameObject handle;
    public enum Source {
        Player,
        Activator
    }
    public bool requireActivator = false;
    public bool toggable = false;
    public bool isOn = false;
    public int timesInteracted = 0;

    public override void SpinInteract()
    {
        Interact(Source.Player);
    }

    public void ActivatorInteract() {
        Interact(Source.Activator);
    }

    void Interact(Source source) {
        if (!requireActivator && source == Source.Player || requireActivator && source == Source.Activator) {
            if (toggable) {
                isOn = !isOn;
            }
            if (!toggable && !isOn) {
                isOn = true;
            }
            handle.transform.localRotation = Quaternion.Euler(0, 0, isOn ? -45 : 45);
            timesInteracted++;
            platform.GetComponent<PlatformController>().Interact(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, platform.transform.position);
    }
}
