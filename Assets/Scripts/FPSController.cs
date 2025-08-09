using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -19.62f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Camera playerCamera;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Auto-assign references if not set
        if (playerBody == null)
            playerBody = transform;
            
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
            
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        
        Debug.Log("FPSController started successfully!");
    }
    
    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }
    
    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // Debug mouse input
        // if (mouseX != 0 || mouseY != 0)
        //     Debug.Log("Mouse look: " + mouseX + ", " + mouseY);
        
        // Rotate player body left/right
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
        
        // Rotate camera up/down
        if (playerCamera != null)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
    
    void HandleMovement()
    {
        // Check if controller exists
        if (controller == null)
        {
            Debug.LogError("Character Controller is missing!");
            return;
        }
        
        // Check if grounded
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // Debug movement input
        // if (x != 0 || z != 0)
        //     Debug.Log("Movement input: " + x + ", " + z);
        
        // Calculate movement direction
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Determine speed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // Apply movement
        if (move.magnitude > 0)
        {
            controller.Move(move * currentSpeed * Time.deltaTime);
            // Debug.Log("Moving with speed: " + currentSpeed);
        }
        
        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // Debug.Log("Jumping!");
        }
        
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
