using UnityEngine;
using TMPro;

/// <summary>
/// Manages mission flow: assigning target, tracking player progress, and handling mission completion.
/// </summary>

public class MissionManager : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public TextMeshProUGUI missionText; // UI element to display mission target
    public GameObject missionCompletePanel; // Panel to show on mission completion
    public float successDistance = 10f; // Distance threshold for mission success

    private Transform targetBuilding; // Mission target
    private bool missionStarted = false; // Flag to track if mission is in progress

    private BuildingName[] buildings; // Cached list of all buildings

    void Start()
    {
        buildings = FindObjectsOfType<BuildingName>();
        missionText.text = "Reach: ???"; // Default display
        missionText.gameObject.SetActive(true);
        missionCompletePanel.SetActive(false); // Hide mission complete panel initially
    }

    void Update()
    {
        if (missionStarted && targetBuilding != null)
        {
            float distance = Vector3.Distance(player.transform.position, targetBuilding.position);
            if (distance <= successDistance)
            {
                MissionComplete();
            }
        }
    }

    // Starts a new mission by selecting random buildings for spawn and target
    public void StartMission()
    {
        if (buildings.Length < 2)
        {
            Debug.LogError("Not enough buildings for mission.");
            return;
        }

        int spawnIndex = Random.Range(0, buildings.Length);
        int targetIndex = Random.Range(0, buildings.Length);

        // Ensure target and spawn buildings are different
        while (targetIndex == spawnIndex)
        {
            targetIndex = Random.Range(0, buildings.Length);
        }

        // Generate a random spawn position offset from the spawn building
        Vector3[] directions = new Vector3[]
        {
            Vector3.forward,  
            Vector3.back,     
            Vector3.left,     
            Vector3.right  
        };

        Vector3 randomDirection = directions[Random.Range(0, directions.Length)];
        Vector3 spawnPos = buildings[spawnIndex].transform.position + randomDirection * 10f; 
        spawnPos.y = 0; // Ensure spawn is on ground level
        player.transform.position = spawnPos;

        Debug.Log($"[MissionManager] Player spawned near {buildings[spawnIndex].buildingName} at {spawnPos}");

        // Set the target
        targetBuilding = buildings[targetIndex].transform;
        missionText.text = "Reach: " + buildings[targetIndex].buildingName;
        missionText.gameObject.SetActive(true);

        missionStarted = true; 
    }

    // Called when the player reaches the target
    void MissionComplete()
    {
        missionText.text = "Mission Complete!";
        targetBuilding = null;
        FindObjectOfType<TimerManager>().enabled = false; // Stop the timer
            missionCompletePanel.SetActive(true); 
        CreditManager.Instance.AddCredits(100); // Add rewards
    }

    //  Resets and restarts the mission
    public void RestartMission()
    {
        Debug.Log("[MissionManager] RestartMission called.");

        missionCompletePanel.SetActive(false);  //Hide mission complete panel initially
        missionText.text = "Reach: ???"; 

        missionStarted = false; 
        targetBuilding = null; 

        TimerManager timer = FindObjectOfType<TimerManager>();
        timer.ResetTimer(); 
        timer.StartTimer(); 
        StartMission();     // Restart another mission
    }

    // Shows the mission complete panel
    public void ShowCompletePanel()
    {
        missionCompletePanel.SetActive(true);
    }

    // Returns to the main menu and hides mission UI.
    public void BackToMainMenu()
    {
        Debug.Log("[MissionManager] Returning to Main Menu.");

        missionCompletePanel.SetActive(false);
        missionText.gameObject.SetActive(false);

        GameUIManager uiManager = FindObjectOfType<GameUIManager>();
        if (uiManager != null)
        {
            uiManager.ShowMainMenu();
        }
    }
}