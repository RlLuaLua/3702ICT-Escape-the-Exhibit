using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    public float moveSpeed;
    public float turnVelocity;

    public float jumpForce = 5f;
    public bool isGrounded;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Raycast to below the character to check if character is on ground as set isGrounded
        if(Physics.Raycast(transform.position, Vector3.down, 0.05f)) {
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }
        animator.SetBool("isGrounded", isGrounded);

        // If grounded, reset y direction and set animation bool for isJumping to false
        if (isGrounded) {
            direction.y = -0.5f;
            animator.SetBool("isJumping", false);
        }
        
        // Find which direction user is pressing on left or right using AD or arrow keys
        float horizontal = Input.GetAxisRaw("Horizontal");
        direction = new Vector3(horizontal * moveSpeed, direction.y, 0);

        // If no key is being pressed and player is grounded then set isWalking Animation to false
        if (!Input.anyKey && isGrounded) {
            animator.SetBool("isWalking", false);
        }

        // If W is pressed down and player is on ground then jump by adding jumpForce to y direction
        // and set isJumping animation to true
        if(Input.GetKeyDown(KeyCode.W) && isGrounded) {
            direction.y += jumpForce;
            animator.SetBool("isJumping", true);
        }
        
        // Rotate player
        if(direction.x >= 0.1f || -0.1f >= direction.x) {
            animator.SetBool("isWalking", true);
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, ref turnVelocity, 0.1f) ;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        //If character hits the underside of a block (collides with something above it) prevent sticking
        if ((controller.collisionFlags & CollisionFlags.Above) != 0) {
            if (direction.y > 0) {
                direction.y = 0;
            }
        }

        // Pulls player back to ground each frame
        direction.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(direction * Time.deltaTime);
    }
}