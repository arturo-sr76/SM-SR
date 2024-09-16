using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class MovePOV : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "pov_movement";

    void Start()
    {
        ros = ROSConnection.instance;
        ros.Subscribe<TwistMsg>(topicName, MovePOVGameObject);
    }

    void MovePOVGameObject(TwistMsg twist)
    {
        float moveX = (float)twist.linear.x;
        float moveZ = (float)twist.linear.z; // Use Z instead of Y for forward/backward
        float rotateY = (float)twist.angular.z; // Rotate around Y-axis

        // Move the GameObject
        transform.Translate(new Vector3(moveX, 0, moveZ) * Time.deltaTime);

        // Rotate the GameObject
        transform.Rotate(0, rotateY * Time.deltaTime * 100.0f, 0); // Adjust rotation speed as needed
    }
}

