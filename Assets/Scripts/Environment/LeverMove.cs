using UnityEngine;

public class LeverMove : MonoBehaviour
{
    public MoveType curState;
    public MoveType desiredState;
    public GameObject start; //start position of moving element
    public GameObject end; //end position of moving element
    private Transform nextPos;
    public float speed = 1.0f;
    public float turnAroundTime;
    public bool triggersActivator = false;

    public enum MoveType {
        none,
        oneMove,
        Continuous,
        StartSpin
    }

    void Start(){
        if (end) {
            nextPos = end.transform;
        }
    }
    void Update(){
        switch (curState)
        {
            case MoveType.none: UpdateNoneState(); break;
            case MoveType.oneMove: UpdateOneMoveState(); break;
            case MoveType.Continuous: UpdateContinuousState(); break;
            case MoveType.StartSpin: UpdateStartSpinState(); break;
        }

    }
    public void Interact(){
        if (curState == MoveType.none){
            curState = desiredState;
        }else{
            curState = MoveType.none;
        }
    }
    
    protected void UpdateNoneState(){
        transform.position = Vector3.MoveTowards(transform.position, start.transform.position, speed * Time.deltaTime);
    }

    protected void UpdateOneMoveState(){
        transform.position = Vector3.MoveTowards(transform.position, end.transform.position, speed * Time.deltaTime);
        setActivatorCanProceed();
        
    }

    protected void UpdateContinuousState(){
        if(Vector3.Distance(nextPos.position, transform.position) < 0.1){
            if(nextPos == end.transform){
                Invoke("goStart", turnAroundTime);
            }else{
                Invoke("goEnd", turnAroundTime);
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, nextPos.position, speed * Time.deltaTime);
    }

    protected void UpdateStartSpinState() {
        if (!transform.GetComponentInParent<SpinBehavior>())
            transform.gameObject.AddComponent<SpinBehavior>();
    }

    void goStart(){
        nextPos = start.transform;
    }

    void goEnd(){
        nextPos = end.transform;
    }

    void setActivatorCanProceed() {
        if (triggersActivator && Vector3.Distance(transform.position, end.transform.position) <= 0.05f) {
            GameObject.Find("Activator").GetComponentInChildren<SwitchActivatorController>().canProceed = true;
            triggersActivator = false;
        }
    }

}
