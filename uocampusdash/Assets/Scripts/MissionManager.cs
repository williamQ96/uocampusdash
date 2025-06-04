using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class MissionManager : MonoBehaviour
{
    public GameObject player;
    // Text for displaying current mission target (e.g., "Reach: X")
    public TextMeshProUGUI missionText;
    public GameObject missionCompletePanel;
    public TextMeshProUGUI levelText;

    // Distance threshold for completing a mission
    public float successDistance = 10f;
    // Minimum distance between spawn and target
    public float minDistanceFromTarget = 20f;

    private Transform targetBuilding;
    private bool missionStarted = false;
    private int currentLevel = 0;

    private BuildingName[] buildings;

    // Public accessor to check if mission is ongoing and has a valid target
    public bool IsMissionActive => missionStarted && targetBuilding != null;

    public HungerManager hungerManager;

    public static MissionManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Filter out empty name buildings
// Only include buildings that are children of the "buildings" GameObject
GameObject buildingRoot = GameObject.Find("buildings");

if (buildingRoot != null)
{
    buildings = buildingRoot.GetComponentsInChildren<BuildingName>(true)
                            .Where(b => !string.IsNullOrWhiteSpace(b.buildingName))
                            .ToArray();
}
else
{
    Debug.LogError("❌ Cannot find GameObject named 'buildings' in hierarchy.");
    buildings = new BuildingName[0];
}

        // Initialize level display
        if (levelText != null)
            levelText.text = "Level: 0";
        else
            Debug.LogWarning("⚠️ levelText not assigned.");

        // Initialize mission text
        if (missionText != null)
        {
            missionText.text = "Reach: ???";
            missionText.gameObject.SetActive(true);
        }
        else
            Debug.LogWarning("⚠️ missionText not assigned.");

        // Hide completion panel at start
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);

        // MissionCompleteUIManager.Instance.ShowSuccessMenu(); // For testing success menu
    }

    void Update()
    {
        // If a mission is active and has a target, check distance
        Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        Vector3 targetXZ = new Vector3(targetBuilding.position.x, 0, targetBuilding.position.z);
        float flatDistance = Vector3.Distance(playerXZ, targetXZ);

        if (flatDistance <= successDistance)
        {
            OnMissionSuccess();
        }
    }


    public void StartMission()
    {
        Debug.Log("[MissionManager] StartMission() called");

        if (hungerManager != null)
        {
            hungerManager.ResetHunger();
        }

        if (buildings.Length < 2)
        {
            Debug.LogError("❌ Not enough buildings for mission.");
            return;
        }

        int spawnIndex, targetIndex;
        Vector3 spawnPos;
        int tries = 0, maxTries = 20;

        // Keep retrying until spawn and target buildings are far enough apart
        do
        {
            spawnIndex = Random.Range(0, buildings.Length);
            targetIndex = Random.Range(0, buildings.Length);
            while (targetIndex == spawnIndex)
                targetIndex = Random.Range(0, buildings.Length);

            Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            Vector3 randomDir = directions[Random.Range(0, directions.Length)];
            spawnPos = buildings[spawnIndex].transform.position + randomDir * 10f;
            spawnPos.y = 2f;
            tries++;
        } while (tries < maxTries &&
                 Vector3.Distance(spawnPos, buildings[targetIndex].transform.position) < minDistanceFromTarget);

        if (tries == maxTries)
            Debug.LogWarning("⚠️ Could not find distant spawn/target pair.");

        // Move player to new position (if not teleporting from indoor)
        if (!PlayerReturnPosition.HasTeleportedIntoRoom)
        {
            player.transform.position = spawnPos;
            Debug.Log($"[MissionManager] Player spawned near {buildings[spawnIndex].buildingName} at {spawnPos}");
        }
        else
        {
            Debug.Log("[MissionManager] Skipped spawn due to room teleport.");
            PlayerReturnPosition.HasTeleportedIntoRoom = false;
        }



        // Set mission target
        // targetBuilding = buildings[targetIndex].transform;
        var buildingComponent = buildings[targetIndex];
        string buildingName = string.IsNullOrWhiteSpace(buildingComponent.buildingName)
                              ? "???"
                              : buildingComponent.buildingName;

        targetBuilding = buildingComponent.transform;

        if (missionText != null)
        {
            missionText.text = "Reach: " + buildingName;
            missionText.gameObject.SetActive(true);
        }

        missionStarted = true;
    }

    
private void OnMissionSuccess()
{
    missionStarted = false;
    targetBuilding = null;

    if (missionText != null)
        missionText.text = "Mission Complete!";

    var timer = Object.FindFirstObjectByType<TimerManager>();
    if (timer != null)
        timer.enabled = false;

    CreditManager.Instance.AddCredits(100);
    IncreaseLevel();

    MissionCompleteUIManager.Instance.ShowRewardMenu();
}



public void OnMissionFailure()
{
    missionStarted = false;
    targetBuilding = null;

    var timer = Object.FindFirstObjectByType<TimerManager>();
    if (timer != null)
        timer.enabled = false;

    MissionCompleteUIManager.Instance.ShowRewardMenu(); // Same reward panel
}

 
    public void AddCreditAndLevel()
    {
        CreditManager.Instance.AddCredits(100);
    }

    public void IncreaseLevel()
    {
        currentLevel++;
        if (levelText != null)
            levelText.text = "Level: " + currentLevel;
    }


    public void RestartMission()
    {
        Debug.Log("[MissionManager] RestartMission called.");

        if (hungerManager != null)
        {
            hungerManager.ResetHunger();
        }

        // Hide success / failure menu
        MissionCompleteUIManager.Instance.HideAllMenus();

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);

        if (missionText != null)
            missionText.text = "Reach: ???";

        missionStarted = false;
        targetBuilding = null;

        // Reset timer
        var timer = Object.FindFirstObjectByType<TimerManager>();
        if (timer != null)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }

        StartMission();
    }


    public void BackToMainMenu()
    {
        Debug.Log("[MissionManager] Returning to Main Menu.");

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);

        if (missionText != null)
            missionText.gameObject.SetActive(false);

        var ui = Object.FindFirstObjectByType<GameUIManager>();
        if (ui != null)
            ui.ShowMainMenu();
    }

    public void ShowCompletePanel() => missionCompletePanel?.SetActive(true);

    // This method is called by the "Continue" button after successful mission
    public void ContinueMission()
    {
        Debug.Log("[MissionManager] ContinueMission() called");
        
        if (levelText != null)
            levelText.text = "Level: " + currentLevel;

        CreditManager.Instance.AddCredits(100);
        MissionCompleteUIManager.Instance.HideAllMenus();  // Hide both success/failure menu

        RestartMission(); // Start a new round
    }

    public void ExitGame()
    {
        Debug.Log("[MissionManager] Exiting to main menu.");

        // Hide success/failure menu
        MissionCompleteUIManager.Instance.HideAllMenus();

        if (missionText != null)
            missionText.gameObject.SetActive(false);

        // Show the main menu
        GameUIManager ui = Object.FindFirstObjectByType<GameUIManager>();
        if (ui != null)
            ui.ShowMainMenu();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

}
