using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI missionText;
    public GameObject missionCompletePanel;
    public float successDistance = 10f;
    public float minDistanceFromTarget = 20f; // ✅ Minimum allowed distance between spawn and target

    private Transform targetBuilding;
    private bool missionStarted = false;

    private BuildingName[] buildings;

    public bool IsMissionActive => missionStarted && targetBuilding != null;

    void Start()
    {
        buildings = FindObjectsOfType<BuildingName>();
        missionText.text = "Reach: ???";
        missionText.gameObject.SetActive(true);
        missionCompletePanel.SetActive(false);
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

    public void StartMission()
    {
        Debug.Log("[MissionManager] StartMission() called");

        if (buildings.Length < 2)
        {
            Debug.LogError("Not enough buildings for mission.");
            return;
        }

        int spawnIndex, targetIndex;
        Vector3 spawnPos;

        // ✅ Retry until we find a pair with enough distance
        int maxTries = 20;
        int tries = 0;

        do
        {
            spawnIndex = Random.Range(0, buildings.Length);
            targetIndex = Random.Range(0, buildings.Length);
            while (targetIndex == spawnIndex)
            {
                targetIndex = Random.Range(0, buildings.Length);
            }

            Vector3[] directions = new Vector3[]
            {
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right
            };

            Vector3 randomDirection = directions[Random.Range(0, directions.Length)];
            spawnPos = buildings[spawnIndex].transform.position + randomDirection * 10f;
            spawnPos.y = 0;

            tries++;
        } while (tries < maxTries &&
                 Vector3.Distance(spawnPos, buildings[targetIndex].transform.position) < minDistanceFromTarget);

        if (tries == maxTries)
        {
            Debug.LogWarning("⚠️ Couldn't find a distant enough spawn/target pair after many tries.");
        }

        // ✅ Place player if not just teleported from restaurant
        if (!PlayerReturnPosition.HasTeleportedIntoRoom)
        {
            player.transform.position = spawnPos;
            Debug.Log($"[MissionManager] Player spawned near {buildings[spawnIndex].buildingName} at {spawnPos}");
        }
        else
        {
            Debug.Log("[MissionManager] Skipped spawning because player teleported into room.");
            PlayerReturnPosition.HasTeleportedIntoRoom = false; // reset after use
        }

        // Set mission target
        targetBuilding = buildings[targetIndex].transform;
        missionText.text = "Reach: " + buildings[targetIndex].buildingName;
        missionText.gameObject.SetActive(true);

        missionStarted = true;
    }

    void MissionComplete()
    {
        missionText.text = "Mission Complete!";
        targetBuilding = null;
        Object.FindFirstObjectByType<TimerManager>().enabled = false;
        missionCompletePanel.SetActive(true);
        CreditManager.Instance.AddCredits(100);
    }

    public void RestartMission()
    {
        Debug.Log("[MissionManager] RestartMission called.");

        missionCompletePanel.SetActive(false);
        missionText.text = "Reach: ???";
        missionStarted = false;
        targetBuilding = null;

        TimerManager timer = Object.FindFirstObjectByType<TimerManager>();
        timer.ResetTimer();
        timer.StartTimer();

        StartMission();
    }

    public void ShowCompletePanel()
    {
        missionCompletePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        Debug.Log("[MissionManager] Returning to Main Menu.");
        missionCompletePanel.SetActive(false);
        missionText.gameObject.SetActive(false);
        Object.FindFirstObjectByType<GameUIManager>().ShowMainMenu();
    }
}
