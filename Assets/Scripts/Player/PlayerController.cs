using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    public float moveSpeed;
    public float turnVelocity;
    public Vector3 velocity;
    public bool isGrounded;

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
        if(Physics.Raycast(transform.position, Vector3.down, 0.1f)) {
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }
        animator.SetBool("isGrounded", isGrounded);

        // If grounded, reset y velocity and set animation bool for isJumping to false
        if (isGrounded) {
            velocity.y = -0.5f;
            animator.SetBool("isJumping", false);
        }
        
        // Find which direction user is pressing on left or right using AD or arrow keys
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0, 0).normalized;
        if ((controller.collisionFlags & CollisionFlags.Above) != 0) {
            if (direction.y > 0) {
                direction.y = 0;
            }
        }

        if (direction.magnitude >= 0.1f) {
            animator.SetBool("isWalking", true);
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, ref turnVelocity, 0.1f) ;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }
        if (!Input.anyKey && isGrounded) {
            animator.SetBool("isWalking", false);
        }
        if(Input.GetKeyDown(KeyCode.W) && isGrounded) {
            velocity.y = Mathf.Sqrt(1.0f * -2.0f * Physics.gravity.y);
            animator.SetBool("isJumping", true);
        }
        if (!isGrounded) {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }
}