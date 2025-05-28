using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public GameObject[] menuButtons;        // 0 = Start, 1 = Exit
    public TextMeshProUGUI exitText;        // Assign in Inspector

    private int selectedIndex = 0;

    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
        // Only navigate when inputEnabled is false
        if (!StarterAssets.StarterAssetsInputs.inputEnabled)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
                UpdateButtonVisuals();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = (selectedIndex + 1) % menuButtons.Length;
                UpdateButtonVisuals();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log($"[UI] ENTER pressed on menu index {selectedIndex}");
                switch (selectedIndex)
                {
                    case 0: // Start Game
                        StarterAssets.StarterAssetsInputs.inputEnabled = true;
                        foreach (var btn in menuButtons) btn.SetActive(false);
                        FindObjectOfType<TimerManager>()?.StartTimer();
                        FindObjectOfType<MissionManager>()?.StartMission();
                        break;

                    case 1: // Exit Game
                        ExitGame();
                        break;
                }
            }
        }
    }

    void UpdateButtonVisuals()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            var img = menuButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == selectedIndex) ? Color.yellow : Color.white;
        }
    }

    public void ShowMainMenu()
    {
        StarterAssets.StarterAssetsInputs.inputEnabled = false;
        selectedIndex = 0;
        foreach (var btn in menuButtons)
            btn.SetActive(true);
        UpdateButtonVisuals();
    }

    public void ExitGame()
    {
        Debug.Log("[UI] ExitGame called.");
#if UNITY_WEBGL
        if (exitText != null)
        {
            exitText.gameObject.SetActive(true);
            exitText.text = "Thanks for playing!";
        }
#else
        Application.Quit();
#endif
    }
}
