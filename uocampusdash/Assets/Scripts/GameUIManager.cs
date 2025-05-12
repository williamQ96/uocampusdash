using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public GameObject[] menuButtons; // Array for Start, Store, Exit
    private int selectedIndex = 0;
    public TextMeshProUGUI exitText;
    public GameObject storePanel;

    void Start()
    {
        UpdateButtonVisuals();
        AssignButtonClicks();
    }

    void Update()
    {
        if (!StarterAssets.StarterAssetsInputs.inputEnabled)
        {
            HandleKeyboardNavigation();
        }
    }

    void HandleKeyboardNavigation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = menuButtons.Length - 1;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            if (selectedIndex >= menuButtons.Length) selectedIndex = 0;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            PerformSelectedAction();
        }
    }

    void PerformSelectedAction()
    {
        if (selectedIndex == 0)
        {
            StartGame();
        }
        else if (selectedIndex == 1)
        {
            OpenStore();
        }
        else if (selectedIndex == 2)
        {
            ExitGame();
        }
    }

    void UpdateButtonVisuals()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            var image = menuButtons[i].GetComponent<Image>();
            if (image != null)
            {
                image.color = (i == selectedIndex) ? Color.yellow : Color.white;
            }
        }
    }

    void AssignButtonClicks()
    {
        // Assign the button click events programmatically
        for (int i = 0; i < menuButtons.Length; i++)
        {
            Button button = menuButtons[i].GetComponent<Button>();
            if (button != null)
            {
                int index = i; // Capture the index for closure
                button.onClick.AddListener(() => OnButtonClicked(index));
            }
        }
    }

    void OnButtonClicked(int index)
    {
        selectedIndex = index;
        UpdateButtonVisuals();
        PerformSelectedAction();
    }

    public void StartGame()
    {
        StarterAssets.StarterAssetsInputs.inputEnabled = true;
        foreach (var button in menuButtons)
        {
            button.SetActive(false);
        }
        FindObjectOfType<TimerManager>().StartTimer();
        FindObjectOfType<MissionManager>().StartMission();
    }

    public void OpenStore()
    {
        if (storePanel != null)
        {
            storePanel.SetActive(true);
            foreach (var button in menuButtons)
            {
                button.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("StorePanel is not assigned!");
        }
    }

    public void ExitGame()
    {
        Debug.Log("[GameUIManager] ExitGame called.");
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

    public void ShowMainMenu()
    {
        StarterAssets.StarterAssetsInputs.inputEnabled = false;
        selectedIndex = 0;
        foreach (var button in menuButtons)
        {
            button.SetActive(true);
        }
        UpdateButtonVisuals();
    }
}
