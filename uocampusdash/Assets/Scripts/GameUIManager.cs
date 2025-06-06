using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        if (!StarterAssetsInputs.inputEnabled)
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
                    StarterAssetsInputs.inputEnabled = true;

                    // 👉 Hide the exit message if it's visible
                    if (exitText != null)
                        exitText.gameObject.SetActive(false);

                    foreach (var btn in menuButtons)
                        btn.SetActive(false);

                    FindObjectOfType<TimerManager>()?.StartTimer();
                    FindObjectOfType<MissionManager>()?.StartMission();
                    break;


                    case 1: // Exit Game
                        StartCoroutine(ExitGameSequence());
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
        StarterAssetsInputs.inputEnabled = false;
        selectedIndex = 0;
        foreach (var btn in menuButtons)
            btn.SetActive(true);
        UpdateButtonVisuals();
    }

    IEnumerator ExitGameSequence()
    {
        Debug.Log("[UI] ExitGame called.");

#if UNITY_WEBGL
        if (exitText != null)
        {
            exitText.gameObject.SetActive(true);
            exitText.text = "Thanks for playing!";
        }

        yield return new WaitForSeconds(3f);  // Wait 3 seconds to show message
        // WebGL does not support Application.Quit, so do nothing after
#else
        if (exitText != null)
        {
            exitText.gameObject.SetActive(true);
            exitText.text = "Thanks for playing!";
        }

        yield return new WaitForSeconds(1f);
        Application.Quit();
#endif
    }
}
