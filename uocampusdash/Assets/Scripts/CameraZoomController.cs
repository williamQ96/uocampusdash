using UnityEngine;
using Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private Cinemachine3rdPersonFollow followComponent;

    void Start()
    {
        if (virtualCamera != null)
        {
            followComponent = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }
    }

    void Update()
    {
        if (followComponent != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0f)
            {
                float currentDistance = followComponent.CameraDistance;
                currentDistance -= scrollInput * zoomSpeed;
                currentDistance = Mathf.Clamp(currentDistance, minZoom, maxZoom);
                followComponent.CameraDistance = currentDistance;
            }
        }
    }
}
