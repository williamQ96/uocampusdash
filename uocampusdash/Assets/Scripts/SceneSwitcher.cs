using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;

public class SceneSwitcher : MonoBehaviour
{
    // References for restaurant and exterior building
    public GameObject roomInterior;
    public GameObject buildingExterior;
    public Transform roomSpawnPoint; // Entry point inside the restaurant

    // UI and mission references
    public MissionManager missionManager;
    public FoodMenuUI foodMenuUI;

    // Player spawning
    public GameObject playerPrefab;
    public Transform campusSpawnPoint;

    private GameObject player;

    // Scene names
    public string museumSceneName = "Museum";
    public string campusSceneName = "campus";

    private bool canEnter = false; // Player is within trigger area

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Delay to allow other Start() methods (like RestaurantLayout) to complete
        StartCoroutine(InitializeSpawnPointDelayed());

        if (missionManager == null)
            missionManager = FindObjectOfType<MissionManager>();

        if (foodMenuUI == null)
            foodMenuUI = FindObjectOfType<FoodMenuUI>();
    }

    IEnumerator InitializeSpawnPointDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        EnsureSpawnPoint(); // Ensures spawn point is valid after layout builds
    }

    void Update()
    {
        // Press E to enter restaurant if allowed
        if (canEnter && Input.GetKeyDown(KeyCode.E))
        {
            if (missionManager == null || missionManager.IsMissionActive)
            {
                EnterRestaurant();
            }
        }

        // Press H anytime to close food menu and optionally exit restaurant or return to campus
        if (Input.GetKeyDown(KeyCode.H))
        {
            foodMenuUI?.ForceCloseMenu();

            if (PlayerReturnPosition.HasTeleportedIntoRoom)
            {
                ExitRestaurant();
                foodMenuUI.ForceCloseMenu();
            }
            else if (SceneManager.GetActiveScene().name == museumSceneName)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(campusSceneName);
            }
        }

        // Press B to enter museum (record return position)
        if (canEnter && Input.GetKeyDown(KeyCode.B))
        {
            PlayerReturnPosition.LastOutsidePosition = player.transform.position;
            PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
            PlayerReturnPosition.HasRecordedOutside = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(museumSceneName);
        }
    }

    void EnterRestaurant()
    {
        if (player == null) return;

        PlayerReturnPosition.LastOutsidePosition = player.transform.position;
        PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
        PlayerReturnPosition.HasTeleportedIntoRoom = true;

        roomInterior?.SetActive(true);
        buildingExterior?.SetActive(false);

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = new Vector3(0, 10.1f, 0);
            controller.enabled = true;
        }

        foodMenuUI.ResetForceClose(); // Reset menu force-close flag
        StartCoroutine(ShowFoodMenuDelayed()); // Always trigger menu
        FindAnyObjectByType<TimerManager>()?.PauseTimer();
    }




    void ExitRestaurant()
    {
        if (player == null) return;

        // Reactivate exterior
        roomInterior?.SetActive(false);
        buildingExterior?.SetActive(true);

        // Move player back to last recorded outside position
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
            // Destroy old player if needed
            GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
            if (oldPlayer != null) Destroy(oldPlayer);

            if (playerPrefab == null)
            {
                Debug.LogError("❌ No playerPrefab assigned.");
                return;
            }

            // Spawn new player
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

    // Ensures the room spawn point is retrieved from RestaurantLayout if needed
    void EnsureSpawnPoint()
    {
        if (roomSpawnPoint == null && roomInterior != null)
        {
            var layout = roomInterior.GetComponent<RestaurantLayout>();
            if (layout != null && layout.playerSpawnPoint != null)
            {
                roomSpawnPoint = layout.playerSpawnPoint;
                Debug.Log("✅ Spawn point initialized at " + roomSpawnPoint.position);
            }
            else
            {
                Debug.LogWarning("❌ RestaurantLayout or playerSpawnPoint not ready.");
            }
        }
    }
}
