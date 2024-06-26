using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 1.5f;
    public int startingHealth = 100;
    public float fallDamageThreshold = 9f;
    public float fallDamageMultiplier = 10f;
    public float maxDistance = 10f;

    public GameObject hand;
    public GameObject sword;
    public GameObject pickaxe;

    private int currentHealth;
    private bool isGrounded;
    private Rigidbody rb;

    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float maxLookUpAngle = 90f;
    public float maxLookDownAngle = -90f;

    private float verticalRotation = 0f;
    
    public LayerMask targetLayerMask;
    
    public LayerMask blockLayerMask;
    
    public LayerMask ignoredLayers;

    public TerrainManager terrainManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = startingHealth;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, maxLookDownAngle, maxLookUpAngle);

        transform.Rotate(Vector3.up * mouseX);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Rotate player horizontally based on mouse input
        transform.Rotate(Vector3.up * mouseX);
        rb.angularVelocity = Vector3.zero;
        MovePlayer();
        HandleJump();
        HandleSprint();
        HandleToolUsage();
    }

    private void FixedUpdate()
    {
        // Falling and fall damage
        if (rb.velocity.y < -fallDamageThreshold)
        {
            int damage = Mathf.RoundToInt(Mathf.Abs(rb.velocity.y - fallDamageThreshold));
            TakeDamage(damage);
        }
    }

    void MovePlayer()
    {
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        rb.MovePosition(rb.position + moveDirection);
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
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxDistance, blockLayerMask))
            {
                BlockWrapper rayBlockWrapper = hit.collider.GetComponent<BlockWrapper>();
                if (rayBlockWrapper != null)
                {
                    Block hitBlock = terrainManager.getBlock(rayBlockWrapper.getChunkNumber(), rayBlockWrapper.getBlockNumber());
                    hitBlock.Break();
                    int x = hitBlock.getArrX();
                    int y = hitBlock.getArrY();
                    int z = hitBlock.getArrZ();
                    hitBlock.GetChunk().BlockExposedToAir(x, y, z);
                    rayBlockWrapper.Break();
                }
                
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            // Code to handle placing blocks
            // Example: PlaceBlock();
        }
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with ignored layers
        if (((1 << collision.gameObject.layer) & ignoredLayers) != 0)
        {
            // Collided with an ignored layer, so return without processing
            return;
        }
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
