using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMoverWithLidar : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 2f; // Speed of rotation
    [SerializeField] private float distanceThreshold = 0.2f;
    [SerializeField] private float rotationThreshold = 0.1f; // Threshold to stop rotation

    [SerializeField] private float lidarMaxDistance = 3.0f;  // Maximum Lidar detection distance
    [SerializeField] private int numberOfLidarRays = 360;    // Number of Lidar rays (resolution)
    [SerializeField] private float lidarAngleIncrement;      // Angle between each Lidar ray

    private Transform currentWaypoint;
    private bool isRotating = true;
    private bool avoidingObstacle = false;  // Flag to indicate if the robot is dodging an obstacle
    private Vector3 avoidDirection;

    void Start()
    {
        lidarAngleIncrement = 360f / numberOfLidarRays; // Set Lidar resolution
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;

        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
    }

    void Update()
    {
        if (avoidingObstacle)
        {
            AvoidObstacle();
        }
        else if (CheckForObstacles())
        {
            avoidingObstacle = true;
            SetAvoidDirection(); // Set the direction to avoid the obstacle
        }
        else
        {
            // Rotate towards the waypoint first
            if (isRotating)
            {
                RotateTowardsWaypoint();
            }
            else
            {
                MoveTowardsWaypoint();
            }
        }
    }

    // Lidar Simulation integrated into the waypoint system
    bool CheckForObstacles()
    {
        for (int i = 0; i < numberOfLidarRays; i++)
        {
            float angle = i * lidarAngleIncrement;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, lidarMaxDistance))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                return true; // Obstacle detected
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + direction * lidarMaxDistance, Color.green);
            }
        }
        return false; // No obstacles detected
    }

    // Set direction to avoid obstacles (basic example: move to the right)
    void SetAvoidDirection()
    {
        avoidDirection = transform.right; // Simple example: dodge to the right
    }

    // Avoid obstacle by moving in the avoid direction
    void AvoidObstacle()
    {
        transform.position += avoidDirection * moveSpeed * Time.deltaTime;

        // After moving a bit, resume normal behavior (basic approach)
        avoidingObstacle = false;
    }

    // Rotate towards the current waypoint
    void RotateTowardsWaypoint()
    {
        Vector3 direction = (currentWaypoint.position - transform.position).normalized;

        // Adjust for robot's front (right side as front)
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion adjustment = Quaternion.Euler(0, -90, 0);  // Adjust for robot's front being the right side
        targetRotation *= adjustment;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) < rotationThreshold)
        {
            isRotating = false; // Start moving once the robot is facing the target
        }
    }

    // Move towards the current waypoint
    void MoveTowardsWaypoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            isRotating = true; // Start rotating towards the next waypoint
        }
    }
}
