using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FoodMenuUI : MonoBehaviour
{
    public TextMeshProUGUI[] menuItems; // Assign in Inspector
    public HungerManager hungerManager;
    public CreditManager creditManager;
    public GameObject menuPanel;
    public TextMeshProUGUI feedbackText; 

    private int currentIndex = 0;
    private bool menuActive = false;
    private bool showingSecretMenu = false;

    private List<FoodMenuItem> regularMenu;
    private List<FoodMenuItem> secretMenu;
    private List<FoodMenuItem> currentMenu;

    public static FoodMenuUI Instance;
    private bool menuForceClosed = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        HideMenu();
        feedbackText.text = "";
        feedbackText.gameObject.SetActive(false);

        regularMenu = new List<FoodMenuItem>
        {
            new FoodMenuItem("Burger", 150, () => hungerManager.ReduceHungerRateTemporary(0.5f, 15f)),
            new FoodMenuItem("Fries", 100, () => hungerManager.ReduceHungerRateTemporary(0.75f, 15f)),
            new FoodMenuItem("Drinks", 50, () => hungerManager.ReduceHungerRateTemporary(0.85f, 15f))
        };

        secretMenu = new List<FoodMenuItem>
        {
            new FoodMenuItem("Gamble", 50, GambleEffect),
            new FoodMenuItem("Magic Time", 80, MagicTimeEffect),
            new FoodMenuItem("Energy Drink", 100, EnergyDrinkEffect)
        };

        currentMenu = regularMenu;
    }

    void Update()
    {
        if (!menuActive) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = (currentIndex - 1 + currentMenu.Count) % currentMenu.Count;
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % currentMenu.Count;
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            TryPurchase(currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        currentMenu = (currentMenu == regularMenu) ? secretMenu : regularMenu;
        showingSecretMenu = !showingSecretMenu;
        currentIndex = 0;
        UpdateHighlight();
        // StartCoroutine(ShowTemporaryFeedback(showingSecretMenu ? "Switched to Secret Menu" : "Switched to Main Menu", 2f));
        Debug.Log(showingSecretMenu ? "Switched to Secret Menu" : "Switched to Main Menu");
    }

    private void SetMenuActive(bool active)
    {
        menuActive = active;
        menuPanel.SetActive(active);

        foreach (var item in menuItems)
            item.gameObject.SetActive(active);
    }

    public void ShowMenu()
    {
        SetMenuActive(true);
        currentIndex = 0;
        currentMenu = regularMenu;
        UpdateHighlight();
        feedbackText.text = "";
        feedbackText.gameObject.SetActive(false);
    }

    public void HideMenu()
    {
        SetMenuActive(false);
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            if (i < currentMenu.Count)
            {
                menuItems[i].text = $"{currentMenu[i].itemName} (${currentMenu[i].price})";
                menuItems[i].color = (i == currentIndex) ? Color.yellow : Color.white;
                menuItems[i].gameObject.SetActive(true);
            }
            else
            {
                menuItems[i].gameObject.SetActive(false);
            }
        }
    }

    private void TryPurchase(int index)
    {
        var item = currentMenu[index];
        if (creditManager.credits < item.price)
        {
            StartCoroutine(ShowTemporaryFeedback($"Not enough credits for {item.itemName}!", 2f));
            HideMenu();
            StartCoroutine(ReopenMenuAfterDelay());
            return;
        }

        creditManager.credits -= item.price;
        creditManager.UpdateCreditUI();
        item.onPurchase?.Invoke();

        StartCoroutine(ShowTemporaryFeedback($"Bought {item.itemName}!", 2f));
        HideMenu();
        StartCoroutine(ReopenMenuAfterSuccess());
    }

    private IEnumerator ReopenMenuAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        feedbackText.text = "";
        feedbackText.gameObject.SetActive(false);
        ShowMenu();
    }

    private IEnumerator ReopenMenuAfterSuccess()
    {
        yield return new WaitForSeconds(2f);
        feedbackText.text = "";
        feedbackText.gameObject.SetActive(false);
        if (!menuForceClosed)
            ShowMenu();
    }

    private void GambleEffect()
    {
        int reward = Random.Range(-50, 151);
        creditManager.credits += reward;
        creditManager.UpdateCreditUI();
        StartCoroutine(ShowTemporaryFeedback($"\ud83c\udfb2 You received {reward} credits!", 2f));
    }

    private void MagicTimeEffect()
    {
        if (MissionManager.Instance != null && MissionManager.Instance.IsMissionActive)
        {
            hungerManager.timerManager.AddTime(10f);
            StartCoroutine(ShowTemporaryFeedback("\u23f3 You gained 10 extra seconds!", 2f));
        }
        else
        {
            StartCoroutine(ShowTemporaryFeedback("\u274c No active mission!", 2f));
        }
    }

    private void EnergyDrinkEffect()
    {
        hungerManager.SetHungerFrozen(60f);
        StartCoroutine(ShowTemporaryFeedback("\u26a1 You're energized! Run freely for 1 minute!", 2f));
    }

    private IEnumerator ShowTemporaryFeedback(string message, float duration)
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = message;
            yield return new WaitForSeconds(duration);
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }
    }

    public void ForceCloseMenu()
    {
        menuForceClosed = true;

        if (menuPanel != null)
            menuPanel.SetActive(false);

        foreach (var item in menuItems)
            if (item != null) item.gameObject.SetActive(false);

        currentMenu = regularMenu;
        menuActive = false;
        currentIndex = 0;

        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }
        Debug.Log("✅ ForceCloseMenu called. Deactivating FoodMenuPanel");

    }
}


[System.Serializable]
public class FoodMenuItem
{
    public string itemName;
    public int price;
    public System.Action onPurchase;

    public FoodMenuItem(string name, int price, System.Action action)
    {
        itemName = name;
        this.price = price;
        onPurchase = action;
    }
}