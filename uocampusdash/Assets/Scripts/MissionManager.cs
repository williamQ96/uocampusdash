using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public GameObject player; // 拖玩家进去
    public TextMeshProUGUI missionText; // 显示任务的UI
    public float successDistance = 10f; // 距离多少米算成功

    private Transform targetBuilding; // 目标建筑物

    void Start()
    {
        StartMission();
    }

    public void StartMission()
    {
        // 随机选择一个出生点
        BuildingName[] buildings = FindObjectsOfType<BuildingName>();
        if (buildings.Length < 2)
        {
            Debug.LogError("Not enough buildings for mission.");
            return;
        }

        int spawnIndex = Random.Range(0, buildings.Length);
        int targetIndex = Random.Range(0, buildings.Length);

        // 防止出生点和目标是同一个
        while (targetIndex == spawnIndex)
        {
            targetIndex = Random.Range(0, buildings.Length);
        }

        // 把玩家放到出生点旁边
        Vector3 spawnPos = buildings[spawnIndex].transform.position + new Vector3(5f, 0, 5f); // 偏移一点点，避免重叠
        player.transform.position = spawnPos;

        // 设置目标建筑
        targetBuilding = buildings[targetIndex].transform;
        missionText.text = "Reach: " + buildings[targetIndex].buildingName;
        missionText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (targetBuilding != null)
        {
            float distance = Vector3.Distance(player.transform.position, targetBuilding.position);
            if (distance <= successDistance)
            {
                MissionComplete();
            }
        }
    }

    void MissionComplete()
    {
        missionText.text = "Mission Complete!";
        targetBuilding = null; // 停止检测
        FindObjectOfType<TimerManager>().enabled = false; // 停止倒计时（可选）
    }
}
