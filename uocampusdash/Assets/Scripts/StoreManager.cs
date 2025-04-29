using UnityEngine;
using TMPro;
using UnityEngine.UI;

 /// <summary>
/// Handles and displays the upgrade logic
/// </summary>

public class StoreManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; // UI text displaying current speed
    public TextMeshProUGUI upgradeCostText; // UI text displaying next upgrade cost
    public Button upgradeButton; // Button to trigger the upgrade

    private float playerSpeed = 1.0f; // Initial player speed
    private int upgradeLevel = 0; // Current upgrade level
    private int baseUpgradeCost = 100; // Base cost 

    void Start()
    {
        UpdateStoreUI();
    }

    public void UpgradeSpeed()
    {
        int currentCost = baseUpgradeCost * (upgradeLevel + 1);

        if (CreditManager.Instance.credits >= currentCost)
        {
            CreditManager.Instance.credits -= currentCost; // Deduct credits
            playerSpeed += 0.1f; // Increase speed
            upgradeLevel++; // Increment level

            CreditManager.Instance.UpdateCreditUI(); // Refresh credits UI
            UpdateStoreUI(); // Refresh store UI
        }
        else
        {
            Debug.Log("Not enough credits to upgrade!");
        }
    }

    // Updates the UI elements to reflect the current speed and upgrade cost
    private void UpdateStoreUI()
    {
        speedText.text = "Speed: " + playerSpeed.ToString("0.0");
        int nextCost = baseUpgradeCost * (upgradeLevel + 1);
        upgradeCostText.text = "Next Upgrade Cost: " + nextCost.ToString();
    }

    // Get the current player speed
    public float GetPlayerSpeed()
    {
        return playerSpeed;
    }

    void Update()
{
    UpdateStoreUI();
}
}
