using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class RewardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button[] optionButtons;   // First 3: rewards, Last 2: Continue + Menu
    [SerializeField] private Button refreshButton;
    [SerializeField] private TextMeshProUGUI refreshText;

    [Header("Refresh Settings")]
    [SerializeField] private int baseRefreshCost = 10;

    private List<RewardOption> rewardOptions;
    private List<RewardOption> currentDisplayed;
    private int refreshCount;
    private int currentRefreshCost;
    private int selectedIndex = 0;
    private List<Button> allButtons = new List<Button>();

    private void Awake()
    {
        rewardOptions = CreateRewardOptions();
        if (optionButtons == null || optionButtons.Length < 5)
            Debug.LogError("[RewardManager] Expected at least 5 option buttons (3 rewards + 2 nav).");
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        refreshCount = 0;
        UpdateRefreshCost();
        PopulateRewards();

        // Collect all buttons in order
        allButtons.Clear();
        for (int i = 0; i < optionButtons.Length; i++)
            allButtons.Add(optionButtons[i]);
        allButtons.Add(refreshButton);  // 6th button

        selectedIndex = 0;
        UpdateVisuals();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + allButtons.Count) % allButtons.Count;
            UpdateVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % allButtons.Count;
            UpdateVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            allButtons[selectedIndex].onClick.Invoke();
        }
    }


    private void UpdateVisuals()
    {
        for (int i = 0; i < allButtons.Count; i++)
        {
            var text = allButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.color = (i == selectedIndex) ? Color.yellow : Color.white;
        }
    }

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

    private void PopulateRewards()
    {
        currentDisplayed = new List<RewardOption>();
        var shuffled = new List<RewardOption>(rewardOptions);
        shuffled.Shuffle();

        for (int i = 0; i < 3; i++) // Assume first 3 buttons are reward slots
        {
            var btn = optionButtons[i];
            var reward = shuffled[i];
            currentDisplayed.Add(reward);

            btn.gameObject.SetActive(true);
            btn.interactable = true;

            var bg = btn.GetComponent<Image>();
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (bg != null) bg.color = GetTierColor(reward.Tier);
            if (text != null)
            {
                text.text = $"{reward.Name}\nCost: {reward.Cost} Credits";
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.Center;
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => TryPurchase(reward));
        }

        // Setup Continue
        optionButtons[3].onClick.RemoveAllListeners();
        optionButtons[3].onClick.AddListener(() => MissionManager.Instance.ContinueMission());

        // Setup Return to Menu
        optionButtons[4].onClick.RemoveAllListeners();
        optionButtons[4].onClick.AddListener(() => MissionManager.Instance.BackToMainMenu());
    }

    private void TryPurchase(RewardOption reward)
    {
        var cm = CreditManager.Instance;
        if (cm == null || cm.GetCredits() < reward.Cost)
        {
            Debug.Log("[RewardManager] Not enough credits.");
            return;
        }

        cm.AddCredits(-reward.Cost);
        ApplyReward(reward);

        int idx = currentDisplayed.IndexOf(reward);
        if (idx >= 0 && idx < optionButtons.Length)
        {
            var btn = optionButtons[idx];
            btn.interactable = false;

            var img = btn.GetComponent<Image>();
            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (img != null) img.color = Color.gray;
            if (txt != null) txt.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }

        Debug.Log($"[RewardManager] Purchased: {reward.Name}");
    }

    private void ApplyReward(RewardOption reward)
    {
        var player = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>();
        if (player == null) return;

        switch (reward.Name)
        {
            case "Speed +1": player.MoveSpeed += 1f; player.SprintSpeed += 1.5f; break;
            case "Speed +2": player.MoveSpeed += 2f; player.SprintSpeed += 3f; break;
            case "Speed +3": player.MoveSpeed += 3f; player.SprintSpeed += 4.5f; break;
            case "Jump +1": player.JumpHeight += 0.5f; break;
        }
    }

    public void OnRefreshClicked()
    {
        var cm = CreditManager.Instance;
        if (cm != null && cm.GetCredits() >= currentRefreshCost)
        {
            cm.AddCredits(-currentRefreshCost);
            refreshCount++;
            UpdateRefreshCost();
            PopulateRewards();
        }
        else
        {
            Debug.Log("[RewardManager] Not enough credits to refresh.");
        }
    }

    private void UpdateRefreshCost()
    {
        currentRefreshCost = baseRefreshCost * (int)Math.Pow(2, refreshCount);
        if (refreshText != null)
            refreshText.text = $"Refresh: {currentRefreshCost} Credits";
    }

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