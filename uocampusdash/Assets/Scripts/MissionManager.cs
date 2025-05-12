using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public GameObject player; // 拖你的玩家对象
    public TextMeshProUGUI missionText; // 显示任务的UI
    public GameObject missionCompletePanel; 
    public float successDistance = 20f; // 任务成功的检测距离
    private Transform targetBuilding; // 目标建筑物
    private bool missionStarted = false;

    private BuildingName[] buildings; // 缓存建筑物列表

    void Start()
    {
        buildings = FindObjectsOfType<BuildingName>();
        missionText.text = "Reach: ???"; // 一开始显示问号
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
        if (buildings.Length < 2)
        {
            Debug.LogError("Not enough buildings for mission.");
            return;
        }

        int spawnIndex = Random.Range(0, buildings.Length);
        int targetIndex = Random.Range(0, buildings.Length);

        // 保证出生点和目标不一样
        while (targetIndex == spawnIndex)
        {
            targetIndex = Random.Range(0, buildings.Length);
        }

        // 生成出生点
        Vector3[] directions = new Vector3[]
        {
            Vector3.forward,  // 向前
            Vector3.back,     // 向后
            Vector3.left,     // 向左
            Vector3.right     // 向右
        };

        Vector3 randomDirection = directions[Random.Range(0, directions.Length)];
        Vector3 spawnPos = buildings[spawnIndex].transform.position + randomDirection * 10f; // 推远10米
        spawnPos.y = 0; // 保证出生在地面上
        player.transform.position = spawnPos;

        Debug.Log($"[MissionManager] Player spawned near {buildings[spawnIndex].buildingName} at {spawnPos}");

        // 设置任务目标
        targetBuilding = buildings[targetIndex].transform;
        missionText.text = "Reach: " + buildings[targetIndex].buildingName;
        missionText.gameObject.SetActive(true);

        missionStarted = true; // 标记任务正式开始
    }

    void MissionComplete()
    {
        missionText.text = "Mission Complete!";
        targetBuilding = null;
        FindObjectOfType<TimerManager>().enabled = false; // 停止计时（可选）
            missionCompletePanel.SetActive(true); 
        CreditManager.Instance.AddCredits(100); // 奖励100个Credits
    }
    public void RestartMission()
{
    Debug.Log("[MissionManager] RestartMission called.");

    missionCompletePanel.SetActive(false); // 隐藏胜利面板
    missionText.text = "Reach: ???"; // 重新变成问号

    missionStarted = false; // 任务状态清零
    targetBuilding = null; // 清空上一个目标

    TimerManager timer = FindObjectOfType<TimerManager>();
    timer.ResetTimer(); // 重置Timer
    timer.StartTimer(); // 重新开始倒计时
    StartMission();     // 重新开始任务
}

public void ShowCompletePanel()
{
    missionCompletePanel.SetActive(true);
}

public void BackToMainMenu()
{
    Debug.Log("[MissionManager] Returning to Main Menu.");

    missionCompletePanel.SetActive(false); // 关掉CompletePanel
    missionText.gameObject.SetActive(false); // 隐藏Mission提示
    FindObjectOfType<GameUIManager>().ShowMainMenu(); // 呼叫GameUIManager显示菜单
}
}
