using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Tf2;  // Import tf2_msgs
using System;

public class OdometryPublisher : MonoBehaviour
{
    public string odomTopicName = "odom";
    public string tfTopicName = "/tf"; // Publish to the /tf topic
    public float publishRate = 0.1f;  // 10 Hz

    private ROSConnection ros;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private float elapsed_time = 0.0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<OdometryMsg>(odomTopicName);
        ros.RegisterPublisher<TFMessageMsg>(tfTopicName); // Register TF topic

        // Initialize the last position and rotation
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        elapsed_time += Time.deltaTime;

        if (elapsed_time >= publishRate)
        {
            PublishOdometry();
            PublishTransform(); // Publish transform to /tf
            elapsed_time = 0.0f;
        }
    }

    void PublishOdometry()
    {
        // Create the odometry message
        OdometryMsg odom = new OdometryMsg();

        // Use int for seconds and calculate nanoseconds
        float currentTimeFloat = Time.time;
        int sec = Mathf.FloorToInt(currentTimeFloat);
        uint nanosec = (uint)((currentTimeFloat - (float)sec) * 1e9); // Explicitly use float for accurate calculation

        // Set time and frame IDs
        odom.header.stamp.sec = sec;  // Ensure sec is int
        odom.header.stamp.nanosec = nanosec; // nanosec is uint

        odom.header.frame_id = "odom";
        odom.child_frame_id = "base_link";

        // Set position
        Vector3 currentPosition = transform.position;
        odom.pose.pose.position = new PointMsg(currentPosition.x, currentPosition.y, currentPosition.z);

        // Set orientation
        Quaternion currentRotation = transform.rotation;
        odom.pose.pose.orientation = new QuaternionMsg(currentRotation.x, currentRotation.y, currentRotation.z, currentRotation.w);

        // Calculate linear velocity
        Vector3 velocity = (currentPosition - lastPosition) / publishRate;
        odom.twist.twist.linear = new Vector3Msg(velocity.x, velocity.y, velocity.z);

        // Calculate angular velocity
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(lastRotation);
        Vector3 angularVelocity = deltaRotation.eulerAngles / publishRate;
        angularVelocity = angularVelocity * Mathf.Deg2Rad; // Convert to radians

        // Only consider yaw (rotation around y-axis) for 2D navigation
        odom.twist.twist.angular = new Vector3Msg(0, angularVelocity.y, 0);

        // Publish the odometry message
        ros.Publish(odomTopicName, odom);

        // Update last position and rotation
        lastPosition = currentPosition;
        lastRotation = currentRotation;
    }

    void PublishTransform()
    {
        // Publish odom -> base_link transform
        PublishTF("odom", "base_link", transform.position, transform.rotation);

        // Publish map -> odom transform (Assuming odom is at origin)
        PublishTF("map", "odom", Vector3.zero, Quaternion.identity);
    }

    void PublishTF(string parentFrame, string childFrame, Vector3 position, Quaternion rotation)
    {
        float currentTimeFloat = Time.time;
        int sec = Mathf.FloorToInt(currentTimeFloat);
        uint nanosec = (uint)((currentTimeFloat - (float)sec) * 1e9); // Explicitly use float for accurate calculation

        // Create the current time message
        TimeMsg currentTime = new TimeMsg
        {
            sec = sec, // Ensure correct type
            nanosec = nanosec
        };

        // Create the transform message
        TransformStampedMsg transformStamped = new TransformStampedMsg
        {
            header = new HeaderMsg
            {
                frame_id = parentFrame,
                stamp = currentTime
            },
            child_frame_id = childFrame,
            transform = new TransformMsg
            {
                translation = new Vector3Msg(position.x, position.y, position.z),
                rotation = new QuaternionMsg(rotation.x, rotation.y, rotation.z, rotation.w)
            }
        };

        // Wrap the TransformStamped message in a TFMessage
        TFMessageMsg tfMessage = new TFMessageMsg(new TransformStampedMsg[] { transformStamped });

        // Publish the transform
        ros.Publish(tfTopicName, tfMessage);
    }
}

