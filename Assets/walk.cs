using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walk : MonoBehaviour
{
    public float speed = 0.3f;        // Speed of movement
    public float maxDistance = 10.0f; // Maximum distance to travel before reversing
    private float initialZ;           // Initial z position of the object
    private bool movingForward = false; // Start by moving backward
    private bool firstRouteComplete = false; // Track if the first route is complete

    // Start is called before the first frame update
    void Start()
    {
        initialZ = transform.position.z; // Record the initial z position
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new z position
        float newZ;
        float currentMaxDistance = firstRouteComplete ? maxDistance / 2.0f : maxDistance;

        if (movingForward)
        {
            newZ = transform.position.z + speed * Time.deltaTime;
            if (newZ > initialZ + currentMaxDistance)
            {
                newZ = initialZ + currentMaxDistance;
                movingForward = false; // Reverse direction
                firstRouteComplete = true; // Mark first route as complete
            }
        }
        else
        {
            newZ = transform.position.z - speed * Time.deltaTime;
            if (newZ < initialZ - currentMaxDistance)
            {
                newZ = initialZ - currentMaxDistance;
                movingForward = true; // Reverse direction
                firstRouteComplete = true; // Mark first route as complete
            }
        }

        // Update the position
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
    }
}

