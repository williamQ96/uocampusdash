using UnityEngine;
using UnityEngine.UI; // 加载Button组件
using UnityEngine.SceneManagement; 
using TMPro; 

/// <summary>
/// Manages the main menu UI logic, including starting the game, opening the store and exit game.
/// </summary>

public class GameUIManager : MonoBehaviour
{
    public GameObject[] menuButtons; // All menu buttons ex: index 0 = "Start", index 1 = "Store", index 2 = "Exit"
    public TextMeshProUGUI exitText;
    public GameObject storePanel; // Reference to the store UI panel

    private int selectedIndex = 0; // Index of currently selected menu item

    void Start()
    {
        UpdateButtonVisuals(); // Highlight the default selected button
    }

    void Update()
    {
        // Only handle menu navigation when player input is disabled
        if (!StarterAssets.StarterAssetsInputs.inputEnabled)
        {
            // Navigate up
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
                UpdateButtonVisuals();
            }
            // Navigate down
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = (selectedIndex + 1) % menuButtons.Length;
                UpdateButtonVisuals();
            }
            // Confirm selection
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("ENTER Pressed! Current selection: " + selectedIndex);

                switch (selectedIndex)
                {
                    case 0: // Start Game
                        StarterAssets.StarterAssetsInputs.inputEnabled = true;

                        foreach (var button in menuButtons)
                            button.SetActive(false); // Hide menu buttons

                        FindObjectOfType<TimerManager>()?.StartTimer();
                        FindObjectOfType<MissionManager>()?.StartMission();
                        break;

                    case 1: // Open Store
                        Debug.Log("Store selected.");

                        if (storePanel != null)
                        {
                            storePanel.SetActive(true); // Show store panel
                            foreach (var button in menuButtons)
                                button.SetActive(false); // Hide menu buttons
                        }
                        else
                        {
                            Debug.LogError("StorePanel is not assigned in the inspector!");
                        }
                        break;
                }
            }
        }
    }

    // Updates the visual color of buttons based on current selection
    void UpdateButtonVisuals()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            var image = menuButtons[i].GetComponent<Image>();
            if (image != null)
            {
                if (i == selectedIndex)
                    image.color = Color.yellow; // Selected button: YELLOW
                else
                    image.color = Color.white; // Other buttons: WHITE
            }
        }
    }

// Called to re-enable and show the main menu
public void ShowMainMenu()
{
    StarterAssets.StarterAssetsInputs.inputEnabled = false; // Prohibit player move
    selectedIndex = 0; // When hit start
    foreach (var button in menuButtons)
    {
        button.SetActive(true); // Show the menu again
    }
    UpdateButtonVisuals(); 
}

// Handles exiting the game/showing exit message 
public void ExitGame()
{
    Debug.Log("[GameUIManager] ExitGame called.");

#if UNITY_WEBGL
    // WebGL cannot quit app, so show thank-you message instead
    if (exitText != null)
    {
        exitText.gameObject.SetActive(true);
        exitText.text = "Thanks for playing!";
    }
#else
    // Quit the application
    Application.Quit(); 
#endif
}

}
