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

    private Vector3 cameraOffset;
    private float currentZoom;
    private bool isLeftMouseHeld = false;
    private bool isRightMouseHeld = false;

    void Start()
    {
        currentZoom = cameraDistance;
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Only rotate camera or player if a mouse button is held
        if ((isLeftMouseHeld || isRightMouseHeld) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

            // Rotate camera view (up/down) without changing player direction
            playerCameraRoot.Rotate(-mouseY, 0, 0); // Vertical view

            // Clamp vertical view angle to prevent flipping
            playerCameraRoot.localRotation = Quaternion.Euler(
                Mathf.Clamp(playerCameraRoot.localRotation.eulerAngles.x, -60f, 60f),
                0,
                0
            );

            // If right mouse is held, change player direction
            if (isRightMouseHeld)
            {
                // Directly rotate player to match camera view direction
                player.Rotate(0, mouseX, 0);
            }
        }
    }

    void UpdateCameraPosition()
    {
        // Adjust camera position based on zoom
        cameraOffset = new Vector3(0, 0, -currentZoom);
        transform.position = playerCameraRoot.position + playerCameraRoot.rotation * cameraOffset;
        transform.LookAt(playerCameraRoot.position);
    }
}
