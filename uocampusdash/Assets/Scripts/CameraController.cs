using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;        // The player or character to follow
    public Transform playerCameraRoot; // The pivot for the camera (child of player)
    public float cameraDistance = 5f;  // Default camera distance
    public float cameraSensitivity = 3f; // Mouse sensitivity for view
    public float zoomSpeed = 2f;     // Speed of zooming in/out
    public float minZoom = 2f;       // Minimum camera distance
    public float maxZoom = 15f;      // Maximum camera distance
    
    // Default Camera Settings
    public float defaultAngle = 15f;  // Default angle of the camera
    public float defaultDistance = 5f; // Default distance from the player

    private Vector3 cameraOffset;
    private float currentZoom;
    private bool isLeftMouseHeld = false;
    private bool isRightMouseHeld = false;

    void Start()
    {
        // Set default camera distance and angle
        currentZoom = defaultDistance;
        playerCameraRoot.localRotation = Quaternion.Euler(defaultAngle, 0, 0);
        
        // Set camera to default position
        cameraOffset = new Vector3(0, 0, -currentZoom);
        UpdateCameraPosition();
    }

    void Update()
    {
        HandleMouseInput();
        UpdateCameraPosition();
    }

    void HandleMouseInput()
    {
        // Mouse wheel for zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Left Mouse Button - Rotate Camera (View Only)
        if (Input.GetMouseButtonDown(0))
        {
            isLeftMouseHeld = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isLeftMouseHeld = false;
        }

        // Right Mouse Button - Rotate Camera + Player Direction
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseHeld = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseHeld = false;
        }

        // Only unlock cursor if neither button is held
        if (!isLeftMouseHeld && !isRightMouseHeld)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Rotate camera based on mouse movement
        if (isLeftMouseHeld || isRightMouseHeld)
        {
            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

            // Rotate camera pivot (view) without changing player direction
            playerCameraRoot.Rotate(-mouseY, 0, 0);

            // Clamp vertical view angle to prevent flipping
            playerCameraRoot.localRotation = Quaternion.Euler(
                Mathf.Clamp(playerCameraRoot.localRotation.eulerAngles.x, -60f, 60f),
                0,
                0
            );

            // If right button, also rotate player direction
            if (isRightMouseHeld)
            {
                player.Rotate(0, mouseX, 0);
            }
        }
    }

    void UpdateCameraPosition()
    {
        // Adjust camera position based on zoom
        cameraOffset = new Vector3(0, 0, -currentZoom);
        Vector3 targetPosition = playerCameraRoot.position + playerCameraRoot.rotation * cameraOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
        transform.LookAt(playerCameraRoot.position);
    }
}
