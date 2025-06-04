using UnityEngine;

public class CameraIntroSwitcher : MonoBehaviour
{
    public Camera droneCamera;
    public Camera mainCamera;
    public GameObject dronePath;

    void Start()
    {
        // Only show drone cam at start
        if (droneCamera != null)
        {
            droneCamera.enabled = true;
            droneCamera.GetComponent<AudioListener>().enabled = true;
        }

        if (mainCamera != null)
        {
            mainCamera.enabled = false;
            mainCamera.GetComponent<AudioListener>().enabled = false;
        }
    }

    public void SwitchToMainCamera()
    {
        if (droneCamera != null)
        {
            droneCamera.enabled = false;
            droneCamera.GetComponent<AudioListener>().enabled = false;
        }

        if (mainCamera != null)
        {
            mainCamera.enabled = true;
            mainCamera.GetComponent<AudioListener>().enabled = true;
        }

        if (dronePath != null)
        {
            dronePath.SetActive(false); // Optional: stop drone animations
        }

        Debug.Log("Switched to Main Camera");
    }
}
