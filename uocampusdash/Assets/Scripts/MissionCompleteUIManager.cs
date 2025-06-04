using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionCompleteUIManager : MonoBehaviour
{
    public static MissionCompleteUIManager Instance;

    public GameObject rewardPanel; // ← only this needed now

    private Button[] currentButtons;
    private int selectedIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        selectedIndex = 0;
        UpdateButtonVisuals();
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

    public void ShowRewardPanel()
    {
        rewardPanel.SetActive(true);
        currentButtons = rewardPanel.GetComponentsInChildren<Button>();
        selectedIndex = 0;
        UpdateButtonVisuals();
    }

    public void HideRewardPanel()
    {
        rewardPanel.SetActive(false);
        currentButtons = null;
    }
}
