using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    public float jumpForce;
    private float friction;

    public float turnTime = 0.02f;//time it takes for the character to turn
    float turnVelocity;

    public CharacterController controller;
    private Vector3 moveDirection;
    public float gravityScale;
    // Start is called before the first frame update
    void Start()
    {
        //RB = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");

        moveDirection = new Vector3(horizontal * moveSpeed, moveDirection.y, 0f);
        if(controller.isGrounded)//if the player is touching the ground
        {
            moveDirection.y = Physics.gravity.y * gravityScale; //stops downward acceleration from increasing while on the ground
            if(Input.GetButtonDown("Jump")) //player can only jump while grounded
            {
                moveDirection.y += jumpForce;
            }
        }

        //code for rotating player
        if(moveDirection.x >= 0.1f || -0.1f >= moveDirection.x){
            float targetDir = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg -90;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetDir, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }


        //final movement direction for this frame   
        moveDirection.y += (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
    }
}
