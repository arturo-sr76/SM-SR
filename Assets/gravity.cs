using UnityEngine;

public class SimulatedGravity : MonoBehaviour
{
    public float gravity = -9.81f; // Earth's gravity in m/s^2
    private Vector3 velocity;      // Current velocity of the object

    void Update()
    {
        // Simulate gravity by applying a downward force to the velocity
        velocity.y += gravity * Time.deltaTime;

        // Move the object based on the calculated velocity
        transform.position += velocity * Time.deltaTime;

        // Optional: Implement simple ground collision check to stop the object
        if (transform.position.y <= 0)
        {
            // If the object hits the ground, stop the downward movement
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            velocity.y = 0;
        }
    }
}

