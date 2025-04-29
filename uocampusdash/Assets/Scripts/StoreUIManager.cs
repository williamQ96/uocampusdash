using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the player's navigation within the in-game store UI using the keyboard
/// </summary>

public class StoreUIManager : MonoBehaviour
{
    public GameObject[] storeButtons; // Array of buttons in the store UI
    private int selectedIndex = 0; // Currently selected button index

    void Start()
    {
        UpdateButtonVisuals(); // Initialize button visuals
    }

    void Update()
    {
        HandleNavigationInput(); // Check for up/down/enter input
    }


    // Handles user keyboard input
    private void HandleNavigationInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = storeButtons.Length - 1;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            if (selectedIndex >= storeButtons.Length) selectedIndex = 0;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log($"[StoreUIManager] ENTER pressed. Selected index: {selectedIndex}");

            switch (selectedIndex)
            {
                case 0:
                    FindObjectOfType<StoreManager>().UpgradeSpeed();
                    break;
                case 1:
                    FindObjectOfType<GameUIManager>().ShowMainMenu();
                    gameObject.SetActive(false); // Hide the store panel
                    break;
                default:
                    Debug.LogWarning("[StoreUIManager] Invalid button index.");
                    break;
            }
        }
    }


    void UpdateButtonVisuals()
    {
        for (int i = 0; i < storeButtons.Length; i++)
        {
            var image = storeButtons[i].GetComponent<Image>();
            if (image != null)
            {
                if (i == selectedIndex)
                    image.color = Color.yellow;
                else
                    image.color = Color.white;
            }
        }
    }
}
