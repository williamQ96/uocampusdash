using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class RewardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private Button refreshButton;
    [SerializeField] private TextMeshProUGUI refreshText;

    [Header("Refresh Settings")]
    [SerializeField] private int baseRefreshCost = 10;

    private List<RewardOption> rewardOptions;
    private List<RewardOption> currentDisplayed;
    private int refreshCount;
    private int currentRefreshCost;

    private void Awake()
    {
        if (optionButtons == null || optionButtons.Length == 0)
            Debug.LogError("[RewardManager] No option buttons assigned.");
    }

    private void Start()
    {
        // Initialize reward options list
        rewardOptions = CreateRewardOptions();

        // Hook up refresh button
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefreshClicked);

        // Initial UI and unlock cursor
        UpdateRefreshCost();
        PopulateRewards();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.R))
            OnRefreshClicked();
        if (currentDisplayed != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && currentDisplayed.Count > 0)
                TryPurchase(currentDisplayed[0]);
            if (Input.GetKeyDown(KeyCode.Alpha2) && currentDisplayed.Count > 1)
                TryPurchase(currentDisplayed[1]);
            if (Input.GetKeyDown(KeyCode.Alpha3) && currentDisplayed.Count > 2)
                TryPurchase(currentDisplayed[2]);
        }
    }

    // Create all possible rewards
    private List<RewardOption> CreateRewardOptions()
    {
        return new List<RewardOption>
        {
            new RewardOption("Speed +1", Tier.Green, 10),
            new RewardOption("Speed +2", Tier.Blue, 20),
            new RewardOption("Speed +3", Tier.Purple, 30),
            new RewardOption("Sprint Limit +2s", Tier.Green, 10),
            new RewardOption("Sprint Limit +5s", Tier.Blue, 20),
            new RewardOption("Jump +1", Tier.Purple, 30)
        };
    }

    // Shuffle and display rewards on buttons
    private void PopulateRewards()
    {
        if (optionButtons == null) return;

        currentDisplayed = new List<RewardOption>();
        var shuffled = new List<RewardOption>(rewardOptions);
        shuffled.Shuffle();

        for (int i = 0; i < optionButtons.Length; i++)
        {
            var btn = optionButtons[i];
            if (i >= shuffled.Count)
            {
                btn.gameObject.SetActive(false);
                continue;
            }
            btn.gameObject.SetActive(true);
            var reward = shuffled[i];
            currentDisplayed.Add(reward);

            // Update button UI
            var bg = btn.GetComponent<Image>();
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (bg != null) bg.color = GetTierColor(reward.Tier);
            if (text != null)
            {
                text.text = $"{reward.Name}\nCost: {reward.Cost} Credits";
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.Center;
            }

            // Assign click handler
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => TryPurchase(reward));
        }
    }

    // Attempt to purchase a reward
    private void TryPurchase(RewardOption reward)
    {
        var cm = CreditManager.Instance;
        if (cm == null)
        {
            Debug.LogWarning("[RewardManager] CreditManager not found in scene.");
            return;
        }
        if (cm.GetCredits() >= reward.Cost)
        {
            cm.AddCredits(-reward.Cost);
            ApplyReward(reward);
            // Gray out purchased option
            int idx = currentDisplayed.IndexOf(reward);
            if (idx >= 0 && idx < optionButtons.Length)
            {
                var btn = optionButtons[idx];
                // Disable interactivity
                btn.interactable = false;
                // Tint background to gray
                var img = btn.GetComponent<Image>();
                if (img != null) img.color = Color.gray;
                // Dim text
                var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }
        else
        {
            Debug.Log("[RewardManager] Not enough credits to purchase reward.");
        }
    }

    // Apply reward effects to player
    private void ApplyReward(RewardOption reward)
    {
        var player = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>();
        if (player == null)
        {
            Debug.LogWarning("[RewardManager] ThirdPersonController not found.");
            return;
        }
        switch (reward.Name)
        {
            case "Speed +1": player.MoveSpeed += 1f; player.SprintSpeed += 1.5f; break;
            case "Speed +2": player.MoveSpeed += 2f; player.SprintSpeed += 3f; break;
            case "Speed +3": player.MoveSpeed += 3f; player.SprintSpeed += 4.5f; break;
            case "Sprint Limit +2s": player.sprintLimit += 2f; break;
            case "Sprint Limit +5s": player.sprintLimit += 5f; break;
            case "Jump +1": player.JumpHeight += 0.5f; break;
        }
        Debug.Log($"[RewardManager] Applied reward: {reward.Name}");
    }

    // Refresh button handler
    public void OnRefreshClicked()
    {
        var cm = CreditManager.Instance;
        if (cm == null) return;
        if (cm.GetCredits() >= currentRefreshCost)
        {
            cm.AddCredits(-currentRefreshCost);
            refreshCount++;
            UpdateRefreshCost();
            PopulateRewards();
        }
        else
            Debug.Log("[RewardManager] Not enough credits to refresh.");
    }

    // Update refresh cost display
    private void UpdateRefreshCost()
    {
        currentRefreshCost = baseRefreshCost * (int)Math.Pow(2, refreshCount);
        if (refreshText != null)
            refreshText.text = $"Refresh: {currentRefreshCost} Credits";
    }

    // Get background color by tier
    private Color GetTierColor(Tier tier) => tier switch
    {
        Tier.Green => new Color(0.1f, 0.6f, 0.1f, 0.8f),
        Tier.Blue => new Color(0.1f, 0.4f, 0.8f, 0.8f),
        Tier.Purple => new Color(0.5f, 0.1f, 0.6f, 0.8f),
        _ => Color.white,
    };
}

public enum Tier { Green, Blue, Purple }

public class RewardOption
{
    public string Name { get; }
    public Tier Tier { get; }
    public int Cost { get; }
    public RewardOption(string name, Tier tier, int cost)
    {
        Name = name;
        Tier = tier;
        Cost = cost;
    }
}

public static class ListExtensions
{
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
