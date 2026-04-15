using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 7f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Gravity & Jumping")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private Vector3 velocity;
    private bool IsGrounded;

    [Header("Combat")]
    public float attackDuration = 0.8f; 
    private bool isAttacking = false;

    [Header("References")]
    public Transform cam;
    [SerializeField] private Animator animator;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        IsGrounded = controller.isGrounded;
        if (animator != null)
        {
            animator.SetBool("IsGrounded", IsGrounded);
        }
        HandleAttack();
        if (isAttacking) return;
        HandleMovement();
        HandleGravityAndJump();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        float inputMagnitude = direction.magnitude;
        float animationSpeed = 0f;

        if (inputMagnitude > 0)
        {
            animationSpeed = isSprinting ? 1f : 0.5f;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", animationSpeed, 0.1f, Time.deltaTime);
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
    }

    private void HandleGravityAndJump()
    {
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && IsGrounded && !isAttacking)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                animator.SetFloat("Speed", 0f);
            }

            isAttacking = true;
            Invoke(nameof(ResetAttack), attackDuration);
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
}