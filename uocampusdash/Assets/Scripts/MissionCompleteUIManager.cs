using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionCompleteUIManager : MonoBehaviour
{
    public Button[] buttons; // 拖动要控制的按钮进去
    private int selectedIndex = 0;

    void OnEnable()
    {
        UpdateButtonVisuals();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = buttons.Length - 1;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            if (selectedIndex >= buttons.Length) selectedIndex = 0;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            buttons[selectedIndex].onClick.Invoke(); // 回车模拟点击
        }
    }

    void UpdateButtonVisuals()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            TextMeshProUGUI text = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                if (i == selectedIndex)
                {
                    text.color = Color.yellow; // 选中按钮的字变黄色
                }
                else
                {
                    text.color = Color.white; // 非选中按钮的字变白色
                }
            }
        }
    }
}
