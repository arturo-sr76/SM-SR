using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;    // Movement speed
    public float lookSpeed = 2.0f;    // Mouse look sensitivity

    private CharacterController controller;
    private Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get the input from arrow keys or WASD
        float moveForward = Input.GetAxis("Vertical");   // Up/Down or W/S
        float moveStrafe = Input.GetAxis("Horizontal");  // Left/Right or A/D

        // Move the player in the direction the camera is facing
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Keep the movement aligned with the ground (ignore the camera's vertical angle)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate the desired direction to move
        Vector3 moveDirection = forward * moveForward + right * moveStrafe;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Mouse look rotation
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = -Input.GetAxis("Mouse Y") * lookSpeed;

        // Rotate the player around the y-axis (horizontal rotation)
        transform.Rotate(0, mouseX, 0);

        // Rotate the camera up and down (vertical rotation)
        cameraTransform.Rotate(mouseY, 0, 0);

        // Prevent the camera from flipping upside down by clamping the vertical rotation
        Vector3 cameraRotation = cameraTransform.eulerAngles;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -80f, 80f);
        cameraTransform.eulerAngles = cameraRotation;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Re-lock the cursor when the application gains focus
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

