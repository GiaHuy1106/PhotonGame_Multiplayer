using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState { Locomotion, Attacking, Jumping }

    [Header("State")]
    public PlayerState currentState = PlayerState.Locomotion;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 7f;
    public float turnSmoothTime = 0.15f;
    private float turnSmoothVelocity;

    [Header("Gravity & Jumping")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float jumpCooldown = 1f;
    private float jumpTimer = 0f;
    private Vector3 velocity;

    [Header("Custom Ground Check")]
    public Transform groundCheck;     
    public float groundDistance = 0.3f; 
    public LayerMask groundMask;      
    private bool isGrounded;

    [Header("References")]
    public Transform cam;
    [SerializeField] private Animator animator;
    private CharacterController controller;

    [Header("Auto Aim Settings")]
    public float autoAimRange = 10f;
    public float rotationSpeed = 10f;
    private Transform currentAimTarget;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (animator != null) animator.SetBool("IsGrounded", isGrounded);
        ApplyGravity();

        if (jumpTimer > 0) jumpTimer -= Time.deltaTime;
        if (!isGrounded && currentState == PlayerState.Locomotion)
        {
            currentState = PlayerState.Jumping;
            if (animator != null) animator.Play("Jump_Air", 0, 0f);
        }
        switch (currentState)
        {
            case PlayerState.Locomotion:
                HandleMovement();
                HandleJumpInput();
                HandleAttackInput();
                break;

            case PlayerState.Jumping:
                HandleMovement();
                if (isGrounded && velocity.y < 0)
                {
                    currentState = PlayerState.Locomotion;
                }
                break;

            case PlayerState.Attacking:
                SmoothAutoAim();
                break;
        }
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

        if (inputMagnitude > 0) animationSpeed = isSprinting ? 1f : 0.5f;
        if (animator != null) animator.SetFloat("Speed", animationSpeed, 0.15f, Time.deltaTime);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && jumpTimer <= 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpTimer = jumpCooldown;
            if (animator != null) animator.SetTrigger("Jump");
            currentState = PlayerState.Jumping;
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleAttackInput()
    {
        if (Input.GetButtonDown("Fire1") && isGrounded)
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null) currentAimTarget = closestEnemy.transform;
            else currentAimTarget = null;

            if (animator != null)
            {
                animator.SetTrigger("Attack");
                animator.SetFloat("Speed", 0f);
            }
            currentState = PlayerState.Attacking;
        }
    }

    public void ResetAttack()
    {
        currentState = PlayerState.Locomotion;
    }

    private void SmoothAutoAim()
    {
        if (currentAimTarget != null)
        {
            Vector3 direction = (currentAimTarget.position - transform.position).normalized;
            direction.y = 0; 

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float curDistance = Vector3.Distance(enemy.transform.position, position);
            if (curDistance < distance && curDistance <= autoAimRange)
            {
                closest = enemy;
                distance = curDistance;
            }
        }
        return closest;
    }
}