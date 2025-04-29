using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles keyboard navigation and visual feedback for buttons on the "Mission Complete" screen.
/// </summary>

public class MissionCompleteUIManager : MonoBehaviour
{
    public Button[] buttons; // Buttons to control via keyboard 
    private int selectedIndex = 0; // Current selected button index

    void OnEnable()
    {
        UpdateButtonVisuals();
    }

    void Update()
    {
        // Navigate up
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
            UpdateButtonVisuals();
        }
        // Navigate down
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % buttons.Length;
            UpdateButtonVisuals();
        }
        // Select current button
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedIndex >= 0 && selectedIndex < buttons.Length)
            {
                buttons[selectedIndex].onClick.Invoke(); // Simulate button click
            }
        }
    }   

    // Visually highlight the selected button and dim others
    void UpdateButtonVisuals()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            TextMeshProUGUI text = buttons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (text != null)
            {
                if (i == selectedIndex)
                {
                    text.color = Color.yellow; // Selected button: YELLOW
                }
                else
                {
                    text.color = Color.white; // Other buttons: WHITE
                }
            }
        }
    }
}
