using UnityEngine;
using TMPro;
using System.Collections;

public class FoodMenuUI : MonoBehaviour
{
    public TextMeshProUGUI[] menuItems;  // Assign: Burgers, Fries, Drinks
    public HungerManager hungerManager;
    public CreditManager creditManager;
    public GameObject menuPanel;
    public TextMeshProUGUI feedbackText; // Assign in Inspector (should NOT be under menuPanel)

    private int currentIndex = 0;
    private bool menuActive = false;

    private float[] hungerReduction = { 0.5f, 0.75f, 0.85f }; // Burger, Fries, Drinks → speed multipliers
    private int[] prices = { 150, 100, 50 };

    private float reductionDuration = 15f; 

    void Start()
    {
        HideMenu();
        if (feedbackText != null)
            feedbackText.text = "";  // Keep it active, just empty text
    }

    void Update()
    {
        if (!menuActive) return;

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
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            TryPurchase(currentIndex);
        }
    }

    private void SetMenuActive(bool active)
    {
        menuActive = active;
        if (menuPanel != null)
            menuPanel.SetActive(active);

        foreach (var item in menuItems)
            item.gameObject.SetActive(active);
    }

    public void ShowMenu()
    {
        SetMenuActive(true);
        currentIndex = 0;
        UpdateHighlight();

        if (feedbackText != null)
            feedbackText.text = "";  // clear feedback when menu shows
    }

    public void HideMenu()
    {
        SetMenuActive(false);
        // Keep feedbackText visible
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
        int price = prices[index];
        float reductionFactor = hungerReduction[index];

        if (creditManager.credits < price)
        {
            Debug.Log("❌ Not enough credits.");
            if (feedbackText != null)
            {
                feedbackText.text = $"Not enough credits for {menuItems[index].text}!";
                feedbackText.gameObject.SetActive(true);
            }

            HideMenu();
            StartCoroutine(ReopenMenuAfterDelay());
            return;
        }

        // ✅ Purchase successful
        creditManager.credits -= price;
        creditManager.UpdateCreditUI();

        hungerManager.ReduceHungerRateTemporary(reductionFactor, reductionDuration);

        Debug.Log($"✅ Bought {menuItems[index].text}!");
        if (feedbackText != null)
        {
            feedbackText.text = $"Bought {menuItems[index].text}!";
            feedbackText.gameObject.SetActive(true);
        }

        HideMenu(); 
        StartCoroutine(ReopenMenuAfterSuccess());
    }

    private IEnumerator ReopenMenuAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        if (feedbackText != null)
            feedbackText.text = "";

        ShowMenu();
    }

    private IEnumerator HideFeedbackOnly()
    {
        yield return new WaitForSeconds(3f);
        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ReopenMenuAfterSuccess()
    {
        yield return new WaitForSeconds(2f);

        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }

        ShowMenu(); 
    }
}
