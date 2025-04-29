using UnityEngine;
using TMPro;

/// <summary>
/// Manages player credits and updates the credit UI.
/// </summary>

public class CreditManager : MonoBehaviour
{
    public static CreditManager Instance; // Singleton instance to allow global access
    public TextMeshProUGUI creditText; // Reference to the UI element that displays credits

    public int credits = 0; // Current total credits
    private int lastDisplayedCredits = -1; // Cache the last value preventing unnecessary updates

    void Awake()
    {
        // Initialize Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes, though we only have one scene now
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates to enforce Singleton
        }
    }

    void Start()
    {
        UpdateCreditUI();
    }

    public void AddCredits(int amount)
    {
        credits += amount;
        UpdateCreditUI(); // Initialize the credit display
    }

    public void UpdateCreditUI()
    {
        // Updates the credit UI text if there’s a change in value.
        if (creditText != null && credits != lastDisplayedCredits)
        {
            creditText.text = $"Credits: {credits}";
            lastDisplayedCredits = credits;
        }
    }

    void Update()
    {
        UpdateCreditUI();
    }
}
