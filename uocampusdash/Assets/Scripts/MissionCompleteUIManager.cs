using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MissionCompleteUIManager : MonoBehaviour
{
    public static MissionCompleteUIManager Instance;

    public GameObject successMenu;
    public GameObject failureMenu;

    private Button[] currentButtons;
    private int selectedIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (currentButtons == null || currentButtons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + currentButtons.Length) % currentButtons.Length;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % currentButtons.Length;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            currentButtons[selectedIndex].onClick.Invoke();
        }
    }

    void UpdateButtonVisuals()
    {
        for (int i = 0; i < currentButtons.Length; i++)
        {
            var text = currentButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.color = (i == selectedIndex) ? Color.yellow : Color.white;
        }
    }

    public void ShowSuccessMenu()
    {
        successMenu.SetActive(true);
        failureMenu.SetActive(false);

        currentButtons = successMenu.GetComponentsInChildren<Button>();
        selectedIndex = 0;
        UpdateButtonVisuals();
        gameObject.SetActive(true);
    }

    public void ShowFailureMenu()
    {
        Debug.Log("[UI] Showing Success Menu ✅");
        successMenu.SetActive(false);
        failureMenu.SetActive(true);
        currentButtons = failureMenu.GetComponentsInChildren<Button>();
        selectedIndex = 0;
        UpdateButtonVisuals();
        gameObject.SetActive(true);
    }

    public void HideAllMenus()
    {
        successMenu.SetActive(false);
        failureMenu.SetActive(false);
        currentButtons = null;
        gameObject.SetActive(false);
    }
}
