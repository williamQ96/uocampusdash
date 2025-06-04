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

        missionManager ??= FindObjectOfType<MissionManager>();
        foodMenuUI ??= FindObjectOfType<FoodMenuUI>();
    }

    IEnumerator ShowFoodMenuDelayed()
    {
        yield return new WaitForSeconds(1f);
        if (foodMenuUI != null)
            foodMenuUI.ShowMenu();
    }

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.E) && player != null)
        {
            if (missionManager == null || missionManager.IsMissionActive)
            {
                EnterRestaurant();
                StartCoroutine(ShowFoodMenuDelayed());
            }
        }

        if (PlayerReturnPosition.HasTeleportedIntoRoom && Input.GetKeyDown(KeyCode.H))
        {
            ExitRestaurant();
            foodMenuUI?.ForceCloseMenu();
        }

        if (canEnter && Input.GetKeyDown(KeyCode.B) && player != null)
        {
            // Disable movement before scene change
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            var thirdPerson = player.GetComponent<ThirdPersonController>();
            if (thirdPerson != null) thirdPerson.enabled = false;

            DontDestroyOnLoad(player);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(museumSceneName);
        }

        if (SceneManager.GetActiveScene().name == museumSceneName && Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(campusSceneName);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player != null)
        {
            player.SetActive(true);

            // ✅ Re-bind Cinemachine camera
            var vcam = FindObjectOfType<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                var camRoot = player.transform.Find("PlayerCameraRoot");
                vcam.Follow = camRoot;
                vcam.LookAt = camRoot;
            }

            // ✅ Re-enable movement scripts
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

            // ✅ Re-enable input + cursor lock
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StarterAssetsInputs.inputEnabled = true;

            Debug.Log($"🟢 Player ready in scene '{scene.name}', inputEnabled: {StarterAssetsInputs.inputEnabled}");
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void EnterRestaurant()
    {
        if (player == null || roomSpawnPoint == null) return;

        PlayerReturnPosition.LastOutsidePosition = player.transform.position;
        PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
        PlayerReturnPosition.HasRecordedOutside = true;

        roomInterior?.SetActive(true);
        buildingExterior?.SetActive(false);

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = roomSpawnPoint.position + Vector3.up * 0.1f;
            controller.enabled = true;
        }

        PlayerReturnPosition.HasTeleportedIntoRoom = true;
        FindObjectOfType<TimerManager>()?.PauseTimer();
    }

    void ExitRestaurant()
    {
        foodMenuUI?.ForceCloseMenu();
        if (player == null) return;

        roomInterior?.SetActive(false);
        buildingExterior?.SetActive(true);

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = PlayerReturnPosition.LastOutsidePosition;
            player.transform.rotation = PlayerReturnPosition.LastOutsideRotation;
            controller.enabled = true;
        }

        PlayerReturnPosition.HasTeleportedIntoRoom = false;
        FindObjectOfType<TimerManager>()?.ResumeTimer();
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
