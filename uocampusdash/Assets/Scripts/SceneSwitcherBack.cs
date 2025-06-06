using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;

public class SceneSwitcherBack : MonoBehaviour
{
    public GameObject playerPrefab;           // Drag your Player prefab here
    public Transform campusSpawnPoint;        // Optional: Assign campus spawn point in Inspector

    private GameObject player;

    void Update()
    {
        // B = Enter Museum
        if (Input.GetKeyDown(KeyCode.B))
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                DontDestroyOnLoad(player);  // Temporarily carry over

            SceneManager.sceneLoaded += OnSceneLoadedMuseum;
            SceneManager.LoadScene("Museum");
        }

        // H = Return to Campus
        if (SceneManager.GetActiveScene().name == "Museum" && Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.sceneLoaded += OnSceneLoadedCampus;
            SceneManager.LoadScene("campus");
        }
    }

    // After entering Museum
    void OnSceneLoadedMuseum(Scene scene, LoadSceneMode mode)
    {
        RebindPlayerAndCamera();
        SceneManager.sceneLoaded -= OnSceneLoadedMuseum;
    }

    // After returning to Campus
    void OnSceneLoadedCampus(Scene scene, LoadSceneMode mode)
    {
        // Destroy old player
        GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
        if (oldPlayer != null)
            Destroy(oldPlayer);

        // Respawn a fresh player in campus
        Vector3 spawnPos = campusSpawnPoint != null ? campusSpawnPoint.position : Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        player = Instantiate(playerPrefab, spawnPos, spawnRot);
        player.name = "Player";

        RebindPlayerAndCamera();
        SceneManager.sceneLoaded -= OnSceneLoadedCampus;
    }

    void RebindPlayerAndCamera()
    {
        if (player == null)
        {
            Debug.LogError("❌ Player not assigned.");
            return;
        }

        player.SetActive(true);

        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = true;

        var thirdPerson = player.GetComponent<ThirdPersonController>();
        if (thirdPerson != null) thirdPerson.enabled = true;

        var input = player.GetComponent<PlayerInput>();
        if (input != null)
        {
            input.enabled = false;
            input.enabled = true;
        }

        var cam = FindObjectOfType<CinemachineVirtualCamera>();
        var camTarget = player.transform.Find("PlayerCameraRoot");
        if (cam != null && camTarget != null)
        {
            cam.Follow = camTarget;
            cam.LookAt = camTarget;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StarterAssetsInputs.inputEnabled = true;

        Debug.Log("✅ Player ready in scene: " + SceneManager.GetActiveScene().name);
    }
}
