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
    [SerializeField] private TextMeshProUGUI notificationText;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        refreshCount = 0;
        UpdateRefreshCost();
        PopulateRewards();

        allButtons.Clear();
        foreach (var btn in optionButtons) allButtons.Add(btn);
        allButtons.Add(refreshButton);

        DisableMouseRaycasts();
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
                text.color = (i == selectedIndex)
                    ? new Color(1f, 1f, 0f, 1f)
                    : new Color(0f, 0f, 0f, 1f);
        }
    }

    private List<RewardOption> CreateRewardOptions()
    {
        return new List<RewardOption>
        {
            new RewardOption("Speed +1", Tier.Green, 20),
            new RewardOption("Speed +2", Tier.Blue, 40),
            new RewardOption("Speed +4", Tier.Purple, 60),

            new RewardOption("Sprint +5", Tier.Green, 20),
            new RewardOption("Sprint +10", Tier.Blue, 40),
            new RewardOption("Sprint +15", Tier.Purple, 60),

            new RewardOption("Jump +0.5", Tier.Green, 20),
            new RewardOption("Jump +1.0", Tier.Blue, 40),
            new RewardOption("Jump +1.5", Tier.Purple, 60),

            new RewardOption("Jump Timeout -0.1", Tier.Green, 20),
            new RewardOption("Jump Timeout -0.2", Tier.Blue, 40),
            new RewardOption("Jump Timeout -0.3", Tier.Purple, 60),

            new RewardOption("Fall Timeout -0.05", Tier.Green, 20),
            new RewardOption("Fall Timeout -0.1", Tier.Blue, 40),
            new RewardOption("Fall Timeout -0.15", Tier.Purple, 60),
        };
    }

    private void PopulateRewards()
    {
        currentDisplayed = new List<RewardOption>();
        var shuffled = new List<RewardOption>(rewardOptions);
        shuffled.Shuffle();

        for (int i = 0; i < 3; i++)
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
            btn.navigation = new Navigation { mode = Navigation.Mode.None };
        }

        optionButtons[3].onClick.RemoveAllListeners();
        optionButtons[3].onClick.AddListener(() => MissionManager.Instance.RestartMission());
        optionButtons[4].onClick.RemoveAllListeners();
        optionButtons[4].onClick.AddListener(() => MissionManager.Instance.BackToMainMenu());

        refreshButton.onClick.RemoveAllListeners();
        refreshButton.onClick.AddListener(OnRefreshClicked);
    }

    private void TryPurchase(RewardOption reward)
    {
        var cm = CreditManager.Instance;
        if (cm == null || cm.GetCredits() < reward.Cost)
        {
            ShowNotification("Not enough credits!");
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

        ShowNotification($"Purchased: {reward.Name}");
    }

    private void ApplyReward(RewardOption reward)
    {
        var player = UnityEngine.Object.FindFirstObjectByType<ThirdPersonController>();
        if (player == null) return;

        switch (reward.Name)
        {
            case "Speed +1": player.MoveSpeed += 1f; break;
            case "Speed +2": player.MoveSpeed += 2f; break;
            case "Speed +4": player.MoveSpeed += 4f; break;

            case "Sprint +5": player.SprintSpeed += 5f; break;
            case "Sprint +10": player.SprintSpeed += 10f; break;
            case "Sprint +15": player.SprintSpeed += 15f; break;

            case "Jump +0.5": player.JumpHeight += 0.5f; break;
            case "Jump +1.0": player.JumpHeight += 1.0f; break;
            case "Jump +1.5": player.JumpHeight += 1.5f; break;

            case "Jump Timeout -0.1": player.JumpTimeout = Mathf.Max(0, player.JumpTimeout - 0.1f); break;
            case "Jump Timeout -0.2": player.JumpTimeout = Mathf.Max(0, player.JumpTimeout - 0.2f); break;
            case "Jump Timeout -0.3": player.JumpTimeout = Mathf.Max(0, player.JumpTimeout - 0.3f); break;

            case "Fall Timeout -0.05": player.FallTimeout = Mathf.Max(0, player.FallTimeout - 0.05f); break;
            case "Fall Timeout -0.1": player.FallTimeout = Mathf.Max(0, player.FallTimeout - 0.1f); break;
            case "Fall Timeout -0.15": player.FallTimeout = Mathf.Max(0, player.FallTimeout - 0.15f); break;
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
            ShowNotification("Not enough credits to refresh!");
        }
    }

    private void UpdateRefreshCost()
    {
        currentRefreshCost = baseRefreshCost * (int)Math.Pow(2, refreshCount);
        if (refreshText != null)
            refreshText.text = $"Refresh: {currentRefreshCost} Credits";
    }

    private void DisableMouseRaycasts()
    {
        foreach (var graphic in GetComponentsInChildren<Graphic>())
        {
            graphic.raycastTarget = false;
        }
    }

    private void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            CancelInvoke(nameof(ClearNotification));
            Invoke(nameof(ClearNotification), 2f);
        }
    }

    private void ClearNotification()
    {
        if (notificationText != null)
            notificationText.text = "";
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