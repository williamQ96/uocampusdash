using UnityEngine;
using TMPro;

public class FoodMenuUI : MonoBehaviour
{
    public TextMeshProUGUI[] menuItems;  // Assign 3 items in Inspector
    private int currentIndex = 0;

    public HungerManager hungerManager;
    public CreditManager creditManager;
    public GameObject menuPanel;

    private float[] hungerReduction = { 0.5f, 0.25f, 0.15f };  // Burger, Fries, Drinks
    private int[] prices = { 150, 100, 50 };

    private bool menuActive = false;
    private float menuTimer = 0f;
    private float delayBeforeShow = 1f;

    void Start()
    {
        // SetMenuActive(false);  // hide on start
    }

    void Update()
    {
        if (!menuActive) return;

        // Navigate left/right
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = (currentIndex - 1 + menuItems.Length) % menuItems.Length;
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % menuItems.Length;
            UpdateHighlight();
        }

        // Confirm selection
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryPurchase(currentIndex);
        }
    }

    public void TriggerMenu()
    {
        menuTimer = 0f;
        Debug.Log("⏳ TriggerMenu called - will show menu in 1s");
        Invoke(nameof(EnableMenu), delayBeforeShow);
    }

    private void EnableMenu()
    {
        Debug.Log("🍔 Food menu is now being shown!");
        SetMenuActive(true);
        UpdateHighlight();
    }


    private void SetMenuActive(bool active)
    {
        menuActive = active;

        if (menuPanel != null)
            menuPanel.SetActive(active);

        foreach (var item in menuItems)
            item.gameObject.SetActive(active);
    }


    private void UpdateHighlight()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }

    private void TryPurchase(int index)
    {
        if (creditManager.credits >= prices[index])
        {
            creditManager.credits -= prices[index];
            hungerManager.ReduceHungerRate(hungerReduction[index]);
            creditManager.UpdateCreditUI();
            SetMenuActive(false); 
            Debug.Log($"✅ Bought {menuItems[index].text}!");
        }
        else
        {
            Debug.Log("❌ Not enough credits.");
        }
    }


    public void ShowMenu()
    {
        Debug.Log("🍔 ShowMenu() called");

        menuActive = true;

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            Debug.Log("✅ menuPanel.SetActive(true)");
        }
        else
        {
            Debug.LogError("❌ menuPanel is NULL");
        }

        foreach (var item in menuItems)
            item.gameObject.SetActive(true);

        currentIndex = 0;
        UpdateHighlight();
    }


    public void HideMenu()
    {
        menuActive = false;

        if (menuPanel != null)
            menuPanel.SetActive(false);

        foreach (var item in menuItems)
            item.gameObject.SetActive(false);

        Debug.Log("❌ Menu hidden after exiting restaurant.");
    }



}
