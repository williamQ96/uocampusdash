using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 500f;

    public Transform playerBody;
    public Vector3 cameraOffset = new Vector3(0f, 0.5f, 0f); // Lower camera (0.5 = chest level)

    public float upwardTiltAngle = -15f; // Negative = tilt up

    void Start()
    {
        // Lock the cursor to center
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // Only allow horizontal rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // Rotate the player horizontally (yaw)
        playerBody.Rotate(Vector3.up * mouseX);

        // Apply fixed upward tilt (pitch)
        transform.localRotation = Quaternion.Euler(upwardTiltAngle, 0f, 0f);

        // Position camera low, facing upward
        transform.position = playerBody.position + cameraOffset;
    }
}
