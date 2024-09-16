using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    private enum State { Moving, Rotating, AvoidingTurnLeft, AvoidingMoveForward, AvoidingTurnRight, MoveForwardAfterRight }
    private State currentState; // Current state of the robot

    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 90f; // Degrees per second
    [SerializeField] private float distanceThreshold = 0.2f;
    [SerializeField] private float rotationThreshold = 1f;
    [SerializeField] private float lidarRange = 0.5f;
    [SerializeField] private float avoidanceMoveDistance = 1f; // 1 meter for AvoidingMoveForward
    [SerializeField] private float afterRightMoveDistance = 2f; // 2 meters for MoveForwardAfterRight

    private Transform currentWaypoint;
    private Transform previousWaypoint;  // To store the previous waypoint
    private Vector3 avoidForwardTargetPosition; // Separate variable for AvoidingMoveForward
    private Vector3 afterRightForwardTargetPosition; // Separate variable for MoveForwardAfterRight
    private Quaternion targetRotation;
    private Vector3 closestPointOnPath;  // To store the closest point on the original path
    private bool isTurningLeft = false;  // Flag to check if turning left has started
    private bool isTurningRight = false; // Flag to check if turning right has started
    private bool returningToPath = false;  // Flag to indicate if the robot is returning to the path

    // Start is called before the first frame update
    void Start()
    {
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);

        currentState = State.Rotating; // Start with rotating towards the first waypoint
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                HandleMovingState();
                break;

            case State.Rotating:
                HandleRotatingState();
                break;

            case State.AvoidingTurnLeft:
                HandleAvoidingTurnLeft();
                break;

            case State.AvoidingMoveForward:
                HandleAvoidingMoveForward();
                break;

            case State.AvoidingTurnRight:
                HandleAvoidingTurnRight();
                break;

            case State.MoveForwardAfterRight:
                HandleMoveForwardAfterRight(); // New state to move forward after right turn
                break;
        }
    }

    // Moving State: Move towards the waypoint or return to the original path
    void HandleMovingState()
    {
        Debug.Log("Current state: Moving");

        // If returning to the original path
        if (returningToPath)
        {
            // Move towards the closest point on the original path
            transform.position = Vector3.MoveTowards(transform.position, closestPointOnPath, moveSpeed * Time.deltaTime);

            // Check if the robot has reached the closest point on the path
            if (Vector3.Distance(transform.position, closestPointOnPath) < distanceThreshold)
            {
                returningToPath = false; // Done returning to the path
                Debug.Log("Returned to original path, switching to Rotating state");

                // Now that we’ve returned to the path, switch to the Rotating state
                currentState = State.Rotating;
                return;
            }
            return; // Stop further movement to the waypoint until the path is reached
        }

        // Check for obstacles with Lidar
        if (DetectObstacle())
        {
            currentState = State.AvoidingTurnLeft;
            Debug.Log("Obstacle detected, switching to AvoidingTurnLeft state");
            return;
        }

        // Move towards the waypoint
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);
        Debug.Log("Moving towards: " + currentWaypoint.position);

        // If the robot reaches the waypoint, switch to the Rotating state
        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            previousWaypoint = currentWaypoint;  // Store the current waypoint as the previous one
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            currentState = State.Rotating;
            Debug.Log("Reached waypoint, switching to Rotating state");
        }
    }


    // Rotating State: Rotate towards the next waypoint
    void HandleRotatingState()
    {
        Debug.Log("Current state: Rotating");

        Vector3 direction = (currentWaypoint.position - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction);
        Quaternion adjustment = Quaternion.Euler(0, -90, 0);
        targetRotation *= adjustment;

        // Rotate smoothly
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        Debug.Log("Rotating towards: " + currentWaypoint.position);

        // Check if the rotation is almost complete
        if (Quaternion.Angle(transform.rotation, targetRotation) < rotationThreshold)
        {
            currentState = State.Moving; // Start moving once the robot is facing the target
            Debug.Log("Finished rotating, switching to Moving state");
        }
    }

    // Smooth AvoidingTurnLeft State: Turn 90 degrees to the left gradually
    void HandleAvoidingTurnLeft()
    {
        Debug.Log("Current state: AvoidingTurnLeft");

        // Set the target rotation for 90 degrees to the left, but do it once
        if (!isTurningLeft) // Ensuring it's set only once
        {
            // Rotate the robot by 90 degrees to the left relative to its current Y rotation
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
            Debug.Log("Setting target rotation to 90 degrees left, current rotation: " + transform.eulerAngles + ", target: " + targetRotation.eulerAngles);
            isTurningLeft = true; // Mark that turning has started
        }

        // Rotate smoothly towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // Check if the rotation is complete
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            isTurningLeft = false; // Reset for the next rotation
            currentState = State.AvoidingMoveForward; // Transition to the next state
            avoidForwardTargetPosition = transform.position + transform.right * avoidanceMoveDistance; // Set target for forward movement
            Debug.Log("Finished turning left, switching to AvoidingMoveForward state");
        }
    }

    // Smooth AvoidingMoveForward State: Move forward by 1 unit gradually
    void HandleAvoidingMoveForward()
    {
        Debug.Log("Current state: AvoidingMoveForward");

        // Move 1 meter forward using avoidForwardTargetPosition
        transform.position = Vector3.MoveTowards(transform.position, avoidForwardTargetPosition, moveSpeed * Time.deltaTime);

        // Check if the robot has reached the target position
        if (Vector3.Distance(transform.position, avoidForwardTargetPosition) < 0.01f)
        {
            currentState = State.AvoidingTurnRight; // Switch to the next state
            Debug.Log("Finished moving forward, switching to AvoidingTurnRight state");
        }
    }

    // Smooth AvoidingTurnRight State: Turn 90 degrees to the right gradually
    void HandleAvoidingTurnRight()
    {
        Debug.Log("Current state: AvoidingTurnRight");

        // Set the target rotation for 90 degrees to the right, but do it once
        if (!isTurningRight) // Ensuring it's set only once
        {
            // Rotate the robot by 90 degrees to the right relative to its current Y rotation
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
            Debug.Log("Setting target rotation to 90 degrees right, current rotation: " + transform.eulerAngles + ", target: " + targetRotation.eulerAngles);
            isTurningRight = true; // Mark that turning has started
        }

        // Rotate smoothly towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // Check if the rotation is complete
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            isTurningRight = false; // Reset for the next rotation

            // Check if there is an obstacle in front
            if (!DetectObstacle())  // If no obstacle, move forward by 2 meters
            {
                currentState = State.MoveForwardAfterRight; // New state for moving forward after the right turn
                afterRightForwardTargetPosition = transform.position + transform.right * afterRightMoveDistance;
                Debug.Log("Path clear, moving forward 2 meters after right turn");
            }
            else
            {
                // If there is an obstacle, calculate the closest point on the original path
                closestPointOnPath = GetClosestPointOnLine(previousWaypoint.position, currentWaypoint.position, transform.position);
                returningToPath = true; // Start moving towards the closest point on the original path
                currentState = State.Moving; // Switch to moving state to return to the path
                Debug.Log("Obstacle still detected, returning to path");
            }
        }
    }

    void HandleMoveForwardAfterRight()
    {
        Debug.Log("Current state: MoveForwardAfterRight");

        // Step 1: Move 2 meters forward
        if (afterRightForwardTargetPosition == Vector3.zero) // Ensure afterRightForwardTargetPosition is set only once
        {
            afterRightForwardTargetPosition = transform.position + transform.right * afterRightMoveDistance; // Move 2 meters forward
            Debug.Log("Setting target position to move " + afterRightMoveDistance + " meters forward, Target Position: " + afterRightForwardTargetPosition);
        }

        // Move forward smoothly by the specified distance
        transform.position = Vector3.MoveTowards(transform.position, afterRightForwardTargetPosition, moveSpeed * Time.deltaTime);
        Debug.Log("Current position: " + transform.position + ", Moving towards: " + afterRightForwardTargetPosition);

        // Check if the robot has moved the specified distance forward
        if (Vector3.Distance(transform.position, afterRightForwardTargetPosition) < 0.01f)
        {
            // Step 2: After moving forward, perform the 90-degree right turn
            if (!isTurningRight) // Ensure rotation happens only once
            {
                targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
                Debug.Log("Setting target rotation for 90 degrees to the right, Target Rotation: " + targetRotation.eulerAngles);
                isTurningRight = true; // Mark that turning has started
            }

            // Rotate smoothly towards the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            // Check if the rotation is complete
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isTurningRight = false; // Reset the flag for the next use
                afterRightForwardTargetPosition = Vector3.zero; // Reset afterRightForwardTargetPosition

                // After turning, calculate the closest point on the original path
                closestPointOnPath = GetClosestPointOnLine(previousWaypoint.position, currentWaypoint.position, transform.position);
                returningToPath = true; // Start moving towards the closest point on the original path
                currentState = State.Moving; // Switch to moving state to return to the path
                Debug.Log("Finished turning right, returning to path.");
            }
        }
    }



    // Function to detect obstacles in front of the robot using Lidar
    bool DetectObstacle()
    {
        Debug.DrawRay(transform.position, transform.right * lidarRange, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, lidarRange))
        {
            Debug.Log("Obstacle detected: " + hit.collider.name + " at distance " + hit.distance);
            return true; // Obstacle detected
        }

        Debug.Log("No obstacle detected");
        return false; // No obstacle
    }

    // Calculate the closest point on a line segment from `A` to `B` given a point `P`
    Vector3 GetClosestPointOnLine(Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 AP = P - A;
        Vector3 AB = B - A;
        float magnitudeAB = AB.sqrMagnitude; // Magnitude squared of AB
        float ABAPproduct = Vector3.Dot(AP, AB); // Dot product of AP and AB
        float distance = ABAPproduct / magnitudeAB; // Normalized distance along the line

        // Clamp distance to the line segment [A, B]
        distance = Mathf.Clamp01(distance);

        // Return the closest point
        return A + AB * distance;
    }
}
