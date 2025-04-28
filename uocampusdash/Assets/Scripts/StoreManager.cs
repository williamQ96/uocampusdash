using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; // 当前速度显示
    public TextMeshProUGUI upgradeCostText; // 升级花费显示
    public Button upgradeButton; // 升级按钮

    private float playerSpeed = 1.0f; // 初始速度
    private int upgradeLevel = 0; // 当前升级次数（0开始）
    private int baseUpgradeCost = 100; // 升级基准价格

    void Start()
    {
        UpdateStoreUI();
    }

    public void UpgradeSpeed()
    {
        int currentCost = baseUpgradeCost * (upgradeLevel + 1);

        if (CreditManager.Instance.credits >= currentCost)
        {
            CreditManager.Instance.credits -= currentCost; // 扣credits
            playerSpeed += 0.1f; // 加速度
            upgradeLevel++; // 升一级

            CreditManager.Instance.UpdateCreditUI(); // 刷新Credits显示
            UpdateStoreUI(); // 刷新Store界面
        }
        else
        {
            Debug.Log("Not enough credits to upgrade!");
        }
    }

    private void UpdateStoreUI()
    {
        speedText.text = "Speed: " + playerSpeed.ToString("0.0");
        int nextCost = baseUpgradeCost * (upgradeLevel + 1);
        upgradeCostText.text = "Next Upgrade Cost: " + nextCost.ToString();
    }

    // 提供一个获取当前速度的方法
    public float GetPlayerSpeed()
    {
        return playerSpeed;
    }

    void Update()
{
    UpdateStoreUI();
}
}
