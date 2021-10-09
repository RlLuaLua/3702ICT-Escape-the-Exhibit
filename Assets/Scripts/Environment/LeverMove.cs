using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverMove : MonoBehaviour
{
    public MoveType curState;
    public MoveType desiredState;
    public GameObject start;//start position of moving element
    public GameObject end;//end position of moving element
    private Transform nextPos;
    public float speed;
    public float turnAroundTime;

    public enum MoveType {
        none,
        oneMove,
        Continuous
    }

    void Start(){
        nextPos = end.transform;
    }
    void Update(){
        switch (curState)
        {
            case MoveType.none: UpdateNoneState(); break;
            case MoveType.oneMove: UpdateOneMoveState(); break;
            case MoveType.Continuous: UpdateContinuousState(); break;
        }

    }
    public void Interact(){
        if (curState == MoveType.none){
            curState = desiredState;
        }else{
            curState = MoveType.none;
        }
        Debug.Log("yeeha");
    }
    
    protected void UpdateNoneState(){
        transform.position = Vector3.MoveTowards(transform.position, start.transform.position, speed * Time.deltaTime);
    }

    protected void UpdateOneMoveState(){
        transform.position = Vector3.MoveTowards(transform.position, end.transform.position, speed * Time.deltaTime);
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
    void goStart(){
        nextPos = start.transform;
    }

    void goEnd(){
        nextPos = end.transform;
    }

}
