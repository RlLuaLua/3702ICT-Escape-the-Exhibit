using UnityEngine;

public class LeverController : Interactable
{
    public GameObject[] platforms;
    
    public GameObject handle;
    public enum Source {
        Player,
        Activator,
        Patroller
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

    public void PatrollerInteract() {
        Interact(Source.Patroller);
    }

    void Interact(Source source) {
        if (!requireActivator && source == Source.Player || requireActivator && source == Source.Activator || source == Source.Patroller) {
            if (toggable) {
                isOn = !isOn;
            }
            if (!toggable && !isOn) {
                isOn = true;
            }
            handle.transform.localRotation = Quaternion.Euler(0, 0, isOn ? -45 : 45);
            timesInteracted++;
            
            foreach(GameObject platform in platforms){
                platform.GetComponent<LeverMove>().Interact();
            }
        }
    }

    void OnDrawGizmos()
    {
        foreach(GameObject platform in platforms){
            Gizmos.DrawLine(transform.position, platform.transform.position);
        }
    }
}
