using System.Collections;
using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
    public Camera robotCamera; // Reference to your robot's camera (named "Camera")
    public int captureWidth = 1920; // Width of the captured image
    public int captureHeight = 1080; // Height of the captured image
    public string folder = "CapturedImagesDataset"; // Folder to save images
    public int captureInterval = 30; // Capture an image every 30 frames

    private int screenshotCount = 0; // Keeps track of how many images have been captured

    void Start()
    {
        // Ensure the folder exists
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        // Automatically assign the camera named "Camera"
        robotCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    void Update()
    {
        // Automatically capture an image every 'captureInterval' frames
        if (Time.frameCount % captureInterval == 0)
        {
            StartCoroutine(CaptureImage());
        }
    }

    // Coroutine to capture the camera view and save as a PNG file
    IEnumerator CaptureImage()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame to capture the image

        // Create a RenderTexture to hold the camera view
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        robotCamera.targetTexture = renderTexture; // Set the camera to render to the texture
        Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

        // Render the camera's view
        robotCamera.Render();

        // Set the RenderTexture as active so we can read from it
        RenderTexture.active = renderTexture;

        // Read the pixels from the camera's RenderTexture
        screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenShot.Apply();

        // Save the captured image as a PNG
        byte[] imageBytes = screenShot.EncodeToPNG();
        string filePath = Path.Combine(folder, "screenshot_" + screenshotCount + ".png");
        File.WriteAllBytes(filePath, imageBytes);
        Debug.Log("Captured Image: " + filePath);

        // Clean up
        robotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Increment screenshot count
        screenshotCount++;
    }
}
