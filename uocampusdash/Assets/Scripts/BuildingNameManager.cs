using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildingNameManager : MonoBehaviour
{
    public Camera mainCamera; // 拖入你的主摄像机
    public GameObject nameUIPrefab; // 拖入你的 BuildingNameUI Prefab
    private List<(Transform building, GameObject nameUI)> trackedBuildings = new List<(Transform, GameObject)>();
    public float maxVisibleDistance = 100f; // 最大可见距离，可以在Inspector里调

    void Start()
    {
        // 找到所有带BuildingName脚本的建筑
        BuildingName[] buildings = FindObjectsOfType<BuildingName>();
        foreach (var building in buildings)
        {
            GameObject nameUI = Instantiate(nameUIPrefab, transform); // 生成名字UI，挂在Canvas下
            nameUI.GetComponent<TextMeshProUGUI>().text = building.buildingName;
            trackedBuildings.Add((building.transform, nameUI));
        }
    }

    void Update()
    {
        foreach (var (building, nameUI) in trackedBuildings)
        {
            if (building != null)
            {
                // 把世界坐标转到屏幕上
                Vector3 screenPos = mainCamera.WorldToScreenPoint(building.position + Vector3.up * 5f);
                nameUI.transform.position = screenPos;

                // 计算摄像机到建筑物的实际距离
                float distance = Vector3.Distance(mainCamera.transform.position, building.position);

                if (distance > maxVisibleDistance)
                {
                    nameUI.SetActive(false); // 太远就隐藏
                }
                else
                {
                    nameUI.SetActive(true);  // 近了就显示
                }
            }
        }
    }
}
