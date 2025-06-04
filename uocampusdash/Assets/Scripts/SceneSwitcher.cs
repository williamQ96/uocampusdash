using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject roomInterior; // The interior scene of the building
    public GameObject buildingExterior; // The exterior model of the building
    public Transform roomSpawnPoint; // Where player appears in the interior
    public MissionManager missionManager; // Reference to mission manager
    public FoodMenuUI foodMenuUI; // Reference to the food menu script

    private bool canEnter = false; // Player is in range to enter
    private GameObject player; // Reference to the player

    void Start()
    {
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
                Debug.LogWarning("⚠️ No MissionManager found.");
        }

        if (foodMenuUI == null)
        {
            foodMenuUI = FindObjectOfType<FoodMenuUI>();
            if (foodMenuUI == null)
                Debug.LogError("❌ FoodMenuUI not found in scene. Please assign manually.");
        }
    }

    IEnumerator ShowFoodMenuDelayed()
    {
        yield return new WaitForSeconds(1f);
        if (foodMenuUI != null)
        {
            Debug.Log("✅ Calling foodMenuUI.ShowMenu()");
            foodMenuUI.ShowMenu();
        }
        else
        {
            Debug.LogError("❌ foodMenuUI is NOT assigned in Inspector!");
        }
    }

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("🟡 E pressed, attempting to enter restaurant.");

            if (missionManager == null || missionManager.IsMissionActive)
            {
                EnterRestaurant();
                Debug.Log("✅ Entered restaurant, starting menu coroutine.");
            }
            else
            {
                Debug.LogWarning("⛔ You can only enter the restaurant during an active mission.");
            }
        }

        if (PlayerReturnPosition.HasTeleportedIntoRoom && Input.GetKeyDown(KeyCode.H))
        {
            ExitRestaurant();
            if (FoodMenuUI.Instance != null)
            {
                FoodMenuUI.Instance.ForceCloseMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("BRP Sample Scene");
        }

        if (SceneManager.GetActiveScene().name == "BRP Sample Scene" && Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.LoadScene("campus");
        }
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
            Debug.LogWarning("❌ RestaurantLayout component not found on roomInterior.");
        }

        if (wasInactive)
            roomInterior.SetActive(false);

        if (roomSpawnPoint == null)
        {
            Debug.LogError("❌ roomSpawnPoint is null. Cannot teleport.");
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

        // ✅ Always show menu when entering
        StartCoroutine(ShowFoodMenuDelayed());
    }


    void ExitRestaurant()
    {
        if (foodMenuUI != null)
        {
            foodMenuUI.ForceCloseMenu(); 
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
