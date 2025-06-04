using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Cinemachine;

public class SceneSwitcherBack : MonoBehaviour
{
    private GameObject player;

    void Update()
    {
        // B = Enter Museum
        if (Input.GetKeyDown(KeyCode.B))
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                DontDestroyOnLoad(player);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Museum");
        }

        // H = Return to Campus
        if (SceneManager.GetActiveScene().name == "Museum" && Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("campus");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");

    if (player != null)
    {
        // Force rebind PlayerInput
        var input = player.GetComponent<PlayerInput>();
        if (input != null)
        {
            input.enabled = false;
            input.enabled = true;
        }

        // Rebind Cinemachine camera
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            var camRoot = player.transform.Find("PlayerCameraRoot");
            vcam.Follow = camRoot;
            vcam.LookAt = camRoot;
        }

        // 🔒 Lock and hide mouse cursor to allow look around
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ✅ Enable movement input
        StarterAssetsInputs.inputEnabled = true;
    }

    SceneManager.sceneLoaded -= OnSceneLoaded;
}

}
