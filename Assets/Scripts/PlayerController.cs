using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 12f;
    public float sprintSpeed = 20f;
    public float jumpHeight = 1.5f;
    public int startingHealth = 100;
    public float fallDamageThreshold = 9f;
    public float fallDamageMultiplier = 10f;

    private int currentHealth;
    private bool isGrounded;
    private Rigidbody rb;
    private Camera playerCamera;
    
    public float mouseSensitivity = 100f;
    public float maxVerticalAngle = 80f; // Limit vertical camera rotation

    private float verticalRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        currentHealth = startingHealth;
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();
        HandleJump();
        HandleSprint();
        HandleToolUsage();
        HandleMouseLook();
    }

    void MovePlayer()
    {
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        rb.MovePosition(rb.position + moveDirection);
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX); // Rotate the player horizontally

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle); // Clamp vertical rotation

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); // Rotate the camera vertically
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            isGrounded = false;
        }
    }

    void HandleSprint()
    {
        // Sprinting handled in MovePlayer method based on LeftShift key
    }

    void HandleToolUsage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Code to handle swinging a tool (sword, pickaxe, etc.)
            // Example: ToolSwing();
        }

        if (Input.GetMouseButtonDown(1))
        {
            // Code to handle placing blocks
            // Example: PlaceBlock();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if player is grounded
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }

        // Check for fall damage
        if (collision.relativeVelocity.y < -fallDamageThreshold)
        {
            int damage = Mathf.RoundToInt((-collision.relativeVelocity.y - fallDamageThreshold) * fallDamageMultiplier);
            TakeDamage(damage);
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle player death (e.g., respawn, game over)
        Debug.Log("Player died!");
    }
}
