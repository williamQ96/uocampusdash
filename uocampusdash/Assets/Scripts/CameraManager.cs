using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraPrefab;

    void Awake()
    {
        if (FindObjectOfType<Cinemachine.CinemachineVirtualCamera>() == null)
        {
            GameObject cam = Instantiate(cameraPrefab);
            cam.name = "PlayerFollowCamera";
        }
    }
}
