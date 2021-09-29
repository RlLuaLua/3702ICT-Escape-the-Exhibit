using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    public float jumpForce;
    private float friction;

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
        if(controller.isGrounded)
        {
            moveDirection.y = Physics.gravity.y * gravityScale; //stops downward acceleration from increasing while on the ground
            if(Input.GetButtonDown("Jump")) //player can only jump while grounded
            {
                moveDirection.y += jumpForce;
            }
        }else

        moveDirection.y += (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
    }
}
