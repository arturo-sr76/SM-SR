using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;  // Import the necessary message types

public class LidarPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "scan";
    public float maxDistance = 3.0f;
    public int numberOfRays = 360;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<LaserScanMsg>(topicName);
    }

    void Update()
    {
        LaserScanMsg lidarData = SimulateLidar();
        ros.Publish(topicName, lidarData);
    }

    LaserScanMsg SimulateLidar()
    {
        // Calculate the number of rays in the range 90 to 110 degrees
        int startRay = Mathf.CeilToInt(70f / 360f * numberOfRays);
        int endRay = Mathf.FloorToInt(130f / 360f * numberOfRays);
        int selectedRays = endRay - startRay + 1;

        // Create a new LaserScanMsg
        LaserScanMsg scan = new LaserScanMsg
        {
            angle_min = Mathf.Deg2Rad * 70f,  // Start at 90 degrees
            angle_max = Mathf.Deg2Rad * 130f, // End at 110 degrees
            angle_increment = (Mathf.Deg2Rad * 20f) / selectedRays, // Spread over 20 degrees
            time_increment = 0,
            scan_time = 0,
            range_min = 0.0f,
            range_max = maxDistance,
            ranges = new float[selectedRays]
        };

        for (int i = startRay; i <= endRay; i++)
        {
            float angle = i * (360f / numberOfRays);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            int rayIndex = i - startRay; // Adjust index for the selected range

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                scan.ranges[rayIndex] = hit.distance;
            }
            else
            {
                scan.ranges[rayIndex] = maxDistance;
            }
        }

        return scan;
    }
}

