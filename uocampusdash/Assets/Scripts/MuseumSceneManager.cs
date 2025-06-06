using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.InputSystem;
using StarterAssets;

public class MuseumSceneManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform museumSpawnPoint;

    private GameObject player;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (playerPrefab == null || museumSpawnPoint == null)
        {
            Debug.LogError("MuseumSceneManager missing playerPrefab or spawn point.");
            return;
        }

        player = Instantiate(playerPrefab, museumSpawnPoint.position, Quaternion.identity);
        player.name = "Player";

        // Camera setup
        var cam = FindObjectOfType<CinemachineVirtualCamera>();
        var camTarget = player.transform.Find("PlayerCameraRoot");
        if (cam != null && camTarget != null)
        {
            cam.Follow = camTarget;
            cam.LookAt = camTarget;
        }

        // Enable movement
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = true;

        var input = player.GetComponent<PlayerInput>();
        if (input != null)
        {
            input.enabled = false;
            input.enabled = true;
        }

        var thirdPerson = player.GetComponent<ThirdPersonController>();
        if (thirdPerson != null) thirdPerson.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StarterAssetsInputs.inputEnabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.LoadScene("campus");
        }
    }
}
