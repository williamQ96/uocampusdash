using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject roomInterior;
    public GameObject buildingExterior;
    public Transform roomSpawnPoint;
    public MissionManager missionManager;

    private bool canEnter = false;
    private GameObject player;

    void Start()
    {
        if (roomInterior != null && roomSpawnPoint == null)
        {
            var layout = roomInterior.GetComponent<RestaurantLayout>();
            if (layout != null)
                roomSpawnPoint = layout.playerSpawnPoint;
        }

        // Fallback: find MissionManager in scene
        if (missionManager == null)
        {
            missionManager = FindObjectOfType<MissionManager>();
            if (missionManager == null)
                Debug.LogWarning("⚠️ No MissionManager found.");
        }
    }


    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.E))
        {
            if (missionManager == null || missionManager.IsMissionActive)
            {
                EnterRestaurant();
            }
            else
            {
                Debug.LogWarning("⛔ You can only enter the restaurant during an active mission.");
            }
        }

        if (PlayerReturnPosition.HasTeleportedIntoRoom && Input.GetKeyDown(KeyCode.H)) // Press H - come back to where the player was in main scene
        {
            ExitRestaurant();
        }

        if (Input.GetKeyDown(KeyCode.B)) // Press B - switch scene
        {
            SceneManager.LoadScene("BRP Sample Scene");
        }

        // Press H - Return to main scene if currently in BRP Sample Scene
        if (SceneManager.GetActiveScene().name == "BRP Sample Scene" && Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.LoadScene("campus"); 
        }
    }

    void EnterRestaurant()
    {
        if (player == null) return;

        // Record outside position once
        if (!PlayerReturnPosition.HasRecordedOutside)
        {
            PlayerReturnPosition.LastOutsidePosition = player.transform.position;
            PlayerReturnPosition.LastOutsideRotation = player.transform.rotation;
            PlayerReturnPosition.HasRecordedOutside = true;
        }

        if (roomInterior != null) roomInterior.SetActive(true);
        if (buildingExterior != null) buildingExterior.SetActive(false);

        // Move to interior
        if (roomSpawnPoint != null)
        {
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
        }
    }

    void ExitRestaurant()
    {
        if (player == null) return;

        if (roomInterior != null) roomInterior.SetActive(false);
        if (buildingExterior != null) buildingExterior.SetActive(true);

        // Move back outside
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

        // Reset flags
        PlayerReturnPosition.HasTeleportedIntoRoom = false;
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

            // Keep player reference if inside room
            if (!PlayerReturnPosition.HasTeleportedIntoRoom)
            {
                player = null;
            }
        }
    }
}
