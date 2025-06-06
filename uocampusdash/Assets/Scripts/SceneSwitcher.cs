// // using UnityEngine;
// // using UnityEngine.SceneManagement;
// // using System.Collections;
// // using UnityEngine.InputSystem;
// // using Cinemachine;
// // using StarterAssets;

// // public class SceneSwitcher : MonoBehaviour
// // {
// //     public GameObject roomInterior;
// //     public GameObject buildingExterior;
// //     public Transform roomSpawnPoint;
// //     public MissionManager missionManager;
// //     public FoodMenuUI foodMenuUI;

// //     public GameObject playerPrefab;
// //     public Transform campusSpawnPoint;

// //     private GameObject player;

// //     public string museumSceneName = "Museum";
// //     public string campusSceneName = "campus";

// //     private bool canEnter = false;

// //     void Start()
// //     {
// //         player = GameObject.FindGameObjectWithTag("Player");

// //         if (roomInterior != null && roomSpawnPoint == null)
// //         {
// //             var layout = roomInterior.GetComponent<RestaurantLayout>();
// //             if (layout != null)
// //                 roomSpawnPoint = layout.playerSpawnPoint;
// //         }

// //         if (missionManager == null)
// //             missionManager = FindObjectOfType<MissionManager>();

// //         if (foodMenuUI == null)
// //             foodMenuUI = FindObjectOfType<FoodMenuUI>();
// //     }

// //     void Update()
// //     {
// //         if (canEnter && Input.GetKeyDown(KeyCode.E))
// //         {
// //             if (missionManager == null || missionManager.IsMissionActive)
// //             {
// //                 EnterRestaurant();
// //             }
// //         }

// //         if (PlayerReturnPosition.HasTeleportedIntoRoom && Input.GetKeyDown(KeyCode.H))
// //         {
// //             ExitRestaurant();
// //             foodMenuUI?.ForceCloseMenu();
// //         }

// //         if (canEnter && Input.GetKeyDown(KeyCode.B))
// //         {
// //             // Save position before leaving campus
// //             PlayerReturnPosition.LastOutsidePosition = player.transform.position;
// //             PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
// //             PlayerReturnPosition.HasRecordedOutside = true;

// //             SceneManager.LoadScene(museumSceneName);
// //         }
// //     }

// //     void EnterRestaurant()
// //     {
// //         if (player == null) return;

// //         if (roomInterior != null && roomSpawnPoint == null)
// //         {
// //             var layout = roomInterior.GetComponent<RestaurantLayout>();
// //             if (layout != null)
// //                 roomSpawnPoint = layout.playerSpawnPoint;
// //         }

// //         if (roomSpawnPoint == null)
// //         {
// //             Debug.LogError("❌ No spawn point in Restaurant.");
// //             return;
// //         }

// //         PlayerReturnPosition.LastOutsidePosition = player.transform.position;
// //         PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
// //         PlayerReturnPosition.HasTeleportedIntoRoom = true;

// //         roomInterior?.SetActive(true);
// //         buildingExterior?.SetActive(false);

// //         var controller = player.GetComponent<CharacterController>();
// //         if (controller != null)
// //         {
// //             controller.enabled = false;
// //             player.transform.position = roomSpawnPoint.position + Vector3.up * 0.1f;
// //             controller.enabled = true;
// //         }

// //         StartCoroutine(ShowFoodMenuDelayed());
// //         FindAnyObjectByType<TimerManager>()?.PauseTimer();
// //     }

// //     void ExitRestaurant()
// //     {
// //         if (player == null) return;

// //         roomInterior?.SetActive(false);
// //         buildingExterior?.SetActive(true);

// //         var controller = player.GetComponent<CharacterController>();
// //         if (controller != null)
// //         {
// //             controller.enabled = false;
// //             player.transform.position = PlayerReturnPosition.LastOutsidePosition;
// //             player.transform.rotation = PlayerReturnPosition.LastOutsideRotation;
// //             controller.enabled = true;
// //         }

// //         PlayerReturnPosition.HasTeleportedIntoRoom = false;
// //         FindAnyObjectByType<TimerManager>()?.ResumeTimer();
// //     }

// //     IEnumerator ShowFoodMenuDelayed()
// //     {
// //         yield return new WaitForSeconds(1f);
// //         foodMenuUI?.ShowMenu();
// //     }

// //     void OnTriggerEnter(Collider other)
// //     {
// //         if (other.CompareTag("Player"))
// //         {
// //             canEnter = true;
// //             player = other.gameObject;
// //         }
// //     }

// //     void OnTriggerExit(Collider other)
// //     {
// //         if (other.CompareTag("Player"))
// //         {
// //             canEnter = false;
// //             if (!PlayerReturnPosition.HasTeleportedIntoRoom)
// //                 player = null;
// //         }
// //     }
// // }



// // ✅ 完整整合版 SceneSwitcher.cs
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections;
// using UnityEngine.InputSystem;
// using Cinemachine;
// using StarterAssets;

// public class SceneSwitcher : MonoBehaviour
// {
//     public GameObject roomInterior;
//     public GameObject buildingExterior;
//     public Transform roomSpawnPoint;
//     public MissionManager missionManager;
//     public FoodMenuUI foodMenuUI;

//     public GameObject playerPrefab;
//     public Transform campusSpawnPoint;

//     private GameObject player;

//     public string museumSceneName = "Museum";
//     public string campusSceneName = "campus";

//     private bool canEnter = false;

//     void Start()
//     {
//         player = GameObject.FindGameObjectWithTag("Player");

//         if (roomInterior != null && roomSpawnPoint == null)
//         {
//             var layout = roomInterior.GetComponent<RestaurantLayout>();
//             if (layout != null)
//                 roomSpawnPoint = layout.playerSpawnPoint;
//         }

//         if (missionManager == null)
//             missionManager = FindObjectOfType<MissionManager>();

//         if (foodMenuUI == null)
//             foodMenuUI = FindObjectOfType<FoodMenuUI>();
//     }

//     void Update()
//     {
//         if (canEnter && Input.GetKeyDown(KeyCode.E))
//         {
//             if (missionManager == null || missionManager.IsMissionActive)
//             {
//                 EnterRestaurant();
//             }
//         }

//         if (PlayerReturnPosition.HasTeleportedIntoRoom && Input.GetKeyDown(KeyCode.H))
//         {
//             ExitRestaurant();
//             foodMenuUI?.ForceCloseMenu();
//         }

//         if (canEnter && Input.GetKeyDown(KeyCode.B))
//         {
//             PlayerReturnPosition.LastOutsidePosition = player.transform.position;
//             PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
//             PlayerReturnPosition.HasRecordedOutside = true;

//             DontDestroyOnLoad(player);
//             SceneManager.sceneLoaded += OnSceneLoaded;
//             SceneManager.LoadScene(museumSceneName);
//         }

//         if (SceneManager.GetActiveScene().name == museumSceneName && Input.GetKeyDown(KeyCode.H))
//         {
//             SceneManager.sceneLoaded += OnSceneLoaded;
//             SceneManager.LoadScene(campusSceneName);
//         }
//     }

//     void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         if (scene.name == campusSceneName)
//         {
//             RemoveOldPlayers();

//             if (playerPrefab != null && campusSpawnPoint != null)
//             {
//                 player = Instantiate(playerPrefab, campusSpawnPoint.position, Quaternion.identity);
//                 player.name = "Player";
//             }
//         }

//         if (player != null)
//         {
//             var controller = player.GetComponent<CharacterController>();
//             if (controller != null) controller.enabled = true;

//             var input = player.GetComponent<PlayerInput>();
//             if (input != null) {
//                 input.enabled = false;
//                 input.enabled = true;
//             }

//             var vcam = FindObjectOfType<CinemachineVirtualCamera>();
//             var camTarget = player.transform.Find("PlayerCameraRoot");
//             if (vcam != null && camTarget != null)
//             {
//                 vcam.Follow = camTarget;
//                 vcam.LookAt = camTarget;
//             }

//             Cursor.lockState = CursorLockMode.Locked;
//             Cursor.visible = false;
//             StarterAssetsInputs.inputEnabled = true;
//         }

//         PlayerReturnPosition.HasTeleportedIntoRoom = false;
//         SceneManager.sceneLoaded -= OnSceneLoaded;
//     }

//     void RemoveOldPlayers()
//     {
//         var oldPlayers = GameObject.FindGameObjectsWithTag("Player");
//         foreach (var p in oldPlayers)
//         {
//             Destroy(p);
//         }
//     }

//     void EnterRestaurant()
//     {
//         if (player == null) return;

//         if (roomInterior != null && roomSpawnPoint == null)
//         {
//             var layout = roomInterior.GetComponent<RestaurantLayout>();
//             if (layout != null)
//                 roomSpawnPoint = layout.playerSpawnPoint;
//         }

//         if (roomSpawnPoint == null)
//         {
//             Debug.LogError("❌ No spawn point in Restaurant.");
//             return;
//         }

//         PlayerReturnPosition.LastOutsidePosition = player.transform.position;
//         PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
//         PlayerReturnPosition.HasTeleportedIntoRoom = true;

//         roomInterior?.SetActive(true);
//         buildingExterior?.SetActive(false);

//         var controller = player.GetComponent<CharacterController>();
//         if (controller != null)
//         {
//             controller.enabled = false;
//             player.transform.position = roomSpawnPoint.position + Vector3.up * 0.1f;
//             controller.enabled = true;
//         }

//         StartCoroutine(ShowFoodMenuDelayed());
//         FindAnyObjectByType<TimerManager>()?.PauseTimer();
//     }

//     void ExitRestaurant()
//     {
//         if (player == null) return;

//         roomInterior?.SetActive(false);
//         buildingExterior?.SetActive(true);

//         var controller = player.GetComponent<CharacterController>();
//         if (controller != null)
//         {
//             controller.enabled = false;
//             player.transform.position = PlayerReturnPosition.LastOutsidePosition;
//             player.transform.rotation = PlayerReturnPosition.LastOutsideRotation;
//             controller.enabled = true;
//         }

//         PlayerReturnPosition.HasTeleportedIntoRoom = false;
//         FindAnyObjectByType<TimerManager>()?.ResumeTimer();
//     }

//     IEnumerator ShowFoodMenuDelayed()
//     {
//         yield return new WaitForSeconds(1f);
//         foodMenuUI?.ShowMenu();
//     }

//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             canEnter = true;
//             player = other.gameObject;
//         }
//     }

//     void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             canEnter = false;
//             if (!PlayerReturnPosition.HasTeleportedIntoRoom)
//                 player = null;
//         }
//     }
// }



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

    public GameObject playerPrefab;
    public Transform campusSpawnPoint;

    private GameObject player;

    public string museumSceneName = "Museum";
    public string campusSceneName = "campus";

    private bool canEnter = false;

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
            missionManager = FindObjectOfType<MissionManager>();

        if (foodMenuUI == null)
            foodMenuUI = FindObjectOfType<FoodMenuUI>();
    }

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.E))
        {
            if (missionManager == null || missionManager.IsMissionActive)
            {
                EnterRestaurant();
            }
        }

        if (PlayerReturnPosition.HasTeleportedIntoRoom && Input.GetKeyDown(KeyCode.H))
        {
            ExitRestaurant();
            foodMenuUI?.ForceCloseMenu();
        }

        if (canEnter && Input.GetKeyDown(KeyCode.B))
        {
            PlayerReturnPosition.LastOutsidePosition = player.transform.position;
            PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
            PlayerReturnPosition.HasRecordedOutside = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(museumSceneName);
        }

        if (SceneManager.GetActiveScene().name == museumSceneName && Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(campusSceneName);
        }
    }

    void EnterRestaurant()
    {
        if (player == null) return;

        // ✅ 確保取得最新 spawnPoint
        bool wasInactive = !roomInterior.activeSelf;
        if (wasInactive) roomInterior.SetActive(true);

        if (roomSpawnPoint == null)
        {
            var layout = roomInterior.GetComponent<RestaurantLayout>();
            if (layout != null)
                roomSpawnPoint = layout.playerSpawnPoint;
        }

        if (roomSpawnPoint == null)
        {
            Debug.LogError("❌ No spawn point in Restaurant.");
            return;
        }

        if (wasInactive) roomInterior.SetActive(false);

        PlayerReturnPosition.LastOutsidePosition = player.transform.position;
        PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
        PlayerReturnPosition.HasTeleportedIntoRoom = true;

        roomInterior?.SetActive(true);
        buildingExterior?.SetActive(false);

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = roomSpawnPoint.position + Vector3.up * 0.1f;
            controller.enabled = true;
        }

        StartCoroutine(ShowFoodMenuDelayed());
        FindAnyObjectByType<TimerManager>()?.PauseTimer();
    }

    void ExitRestaurant()
    {
        if (player == null) return;

        roomInterior?.SetActive(false);
        buildingExterior?.SetActive(true);

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = PlayerReturnPosition.LastOutsidePosition;
            player.transform.rotation = PlayerReturnPosition.LastOutsideRotation;
            controller.enabled = true;
        }

        PlayerReturnPosition.HasTeleportedIntoRoom = false;
        FindAnyObjectByType<TimerManager>()?.ResumeTimer();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == campusSceneName)
        {
            GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
            if (oldPlayer != null) Destroy(oldPlayer);

            if (playerPrefab == null)
            {
                Debug.LogError("❌ No playerPrefab assigned.");
                return;
            }

            Vector3 spawn = campusSpawnPoint ? campusSpawnPoint.position : Vector3.zero;
            player = Instantiate(playerPrefab, spawn, Quaternion.identity);
            player.name = "Player";
        }

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
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    IEnumerator ShowFoodMenuDelayed()
    {
        yield return new WaitForSeconds(1f);
        foodMenuUI?.ShowMenu();
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
                player = null;
        }
    }
}
