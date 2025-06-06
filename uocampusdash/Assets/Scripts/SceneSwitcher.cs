using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject roomInterior;
    public GameObject buildingExterior;
    public Transform roomSpawnPoint;
    public MissionManager missionManager;
    public FoodMenuUI foodMenuUI;

    private bool canEnter = false;
    private GameObject player;
    public GameObject playerPrefab;          // Drag your Player prefab in Inspector
    public Transform campusSpawnPoint;       // Set your spawn point for returning to campus


    public string museumSceneName = "Museum";
    public string campusSceneName = "campus";

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (roomInterior != null && roomSpawnPoint == null)
        {
            var layout = roomInterior.GetComponent<RestaurantLayout>();
            if (layout != null)
                roomSpawnPoint = layout.playerSpawnPoint;
        }

        if (missionManager == null)
        {
            missionManager = FindObjectOfType<MissionManager>();
            if (missionManager == null)
                Debug.LogWarning("No MissionManager found.");
        }

        if (foodMenuUI == null)
        {
            foodMenuUI = FindObjectOfType<FoodMenuUI>();
            if (foodMenuUI == null)
                Debug.LogError("FoodMenuUI not found in scene. Please assign manually.");
        }
    }

    IEnumerator ShowFoodMenuDelayed()
    {
        yield return new WaitForSeconds(1f);
        if (foodMenuUI != null)
        {
            Debug.Log("Calling foodMenuUI.ShowMenu()");
            foodMenuUI.ShowMenu();
        }
    }

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Attempting to enter restaurant.");

            if (missionManager == null || missionManager.IsMissionActive)
            {
                EnterRestaurant();
                Debug.Log("Entered restaurant.");
            }
            else
            {
                Debug.LogWarning("You can only enter the restaurant during an active mission.");
            }
        }

        if (PlayerReturnPosition.HasTeleportedIntoRoom && Input.GetKeyDown(KeyCode.H))
        {
            ExitRestaurant();
            if (foodMenuUI != null)
            {
                foodMenuUI.ForceCloseMenu();
            }
        }

        // ✅ Go to Museum Scene
        if (canEnter && Input.GetKeyDown(KeyCode.B) && player != null)
        {
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            var thirdPerson = player.GetComponent<ThirdPersonController>();
            if (thirdPerson != null) thirdPerson.enabled = false;

            DontDestroyOnLoad(player);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(museumSceneName);
        }

        // ✅ Return from Museum to campus
        if (SceneManager.GetActiveScene().name == museumSceneName && Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(campusSceneName);
        }
    }


    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     if (player != null)
    //     {
    //         Debug.Log("OnSceneLoaded: configuring player.");

    //         var controller = player.GetComponent<CharacterController>();
    //         if (controller != null) controller.enabled = true;
    //         else Debug.LogError("CharacterController missing on Player!");

    //         var thirdPerson = player.GetComponent<ThirdPersonController>();
    //         if (thirdPerson != null) thirdPerson.enabled = true;
    //         else Debug.LogError("ThirdPersonController missing on Player!");

    //         var input = player.GetComponent<PlayerInput>();
    //         if (input != null)
    //         {
    //             input.enabled = false;
    //             input.enabled = true;
    //         }
    //         else Debug.LogError("PlayerInput missing on Player!");

    //         var vcam = FindObjectOfType<CinemachineVirtualCamera>();
    //         if (vcam != null)
    //         {
    //             var camRoot = player.transform.Find("PlayerCameraRoot");
    //             if (camRoot != null)
    //             {
    //                 vcam.Follow = camRoot;
    //                 vcam.LookAt = camRoot;
    //             }
    //             else
    //             {
    //                 Debug.LogError("PlayerCameraRoot not found!");
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogError("CinemachineVirtualCamera not found in scene.");
    //         }

    //         Cursor.lockState = CursorLockMode.Locked;
    //         Cursor.visible = false;
    //         StarterAssetsInputs.inputEnabled = true;
    //     }
    //     else
    //     {
    //         Debug.LogError("Player reference is null after scene load.");
    //     }

    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {                                               
        // If returning to campus, destroy old player and create a new one
        if (scene.name == campusSceneName)
        {
            GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
            if (oldPlayer != null)
            {
                Destroy(oldPlayer);
                Debug.Log("🧹 Old player destroyed.");
            }

            if (playerPrefab == null)
            {
                Debug.LogError("❌ PlayerPrefab not assigned in inspector.");
                return;
            }

            Vector3 spawnPos = campusSpawnPoint != null ? campusSpawnPoint.position : Vector3.zero;
            player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player.name = "Player";

            Debug.Log("✅ New player instantiated at spawn point.");
        }

        // Reconnect camera and controls (for both Museum and Campus)
        if (player != null)
        {
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

            var cam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            var camTarget = player.transform.Find("PlayerCameraRoot");
            if (cam != null && camTarget != null)
            {
                cam.Follow = camTarget;
                cam.LookAt = camTarget;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            StarterAssetsInputs.inputEnabled = true;

            Debug.Log("Player control re-enabled.");
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    void EnterRestaurant()
    {
        if (player == null) return;

        bool wasInactive = !roomInterior.activeSelf;
        if (wasInactive)
            roomInterior.SetActive(true);

        var layout = roomInterior.GetComponent<RestaurantLayout>();
        if (layout != null)
        {
            roomSpawnPoint = layout.playerSpawnPoint;
        }
        else
        {
            Debug.LogWarning("RestaurantLayout component not found on roomInterior.");
        }

        if (wasInactive)
            roomInterior.SetActive(false);

        if (roomSpawnPoint == null)
        {
            Debug.LogError("roomSpawnPoint is null.");
            return;
        }

        PlayerReturnPosition.LastOutsidePosition = player.transform.position;
        PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
        PlayerReturnPosition.HasRecordedOutside = true;

        roomInterior.SetActive(true);
        if (buildingExterior != null) buildingExterior.SetActive(false);

        Vector3 targetPos = roomSpawnPoint.position + Vector3.up * 0.1f;
        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = targetPos;
            controller.enabled = true;
        }
        else
        {
            player.transform.position = targetPos;
        }

        PlayerReturnPosition.HasTeleportedIntoRoom = true;

        TimerManager timer = FindAnyObjectByType<TimerManager>();
        if (timer != null)
            timer.PauseTimer();

        StartCoroutine(ShowFoodMenuDelayed());
    }

    void ExitRestaurant()
    {
        if (foodMenuUI != null)
        {
            FoodMenuUI.Instance.ForceCloseMenu();
        }

        if (player == null) return;

        if (roomInterior != null) roomInterior.SetActive(false);
        if (buildingExterior != null) buildingExterior.SetActive(true);

        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = PlayerReturnPosition.LastOutsidePosition;
            player.transform.rotation = PlayerReturnPosition.LastOutsideRotation;
            controller.enabled = true;
        }
        else
        {
            player.transform.position = PlayerReturnPosition.LastOutsidePosition;
            player.transform.rotation = PlayerReturnPosition.LastOutsideRotation;
        }

        PlayerReturnPosition.HasTeleportedIntoRoom = false;

        TimerManager timer = FindAnyObjectByType<TimerManager>();
        if (timer != null)
            timer.ResumeTimer();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
            if (!PlayerReturnPosition.HasTeleportedIntoRoom)
            {
                player = null;
            }
        }
    }
}
