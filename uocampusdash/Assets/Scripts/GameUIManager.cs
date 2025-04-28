using UnityEngine;
using UnityEngine.UI; // 加载Button组件

public class GameUIManager : MonoBehaviour
{
    public GameObject[] menuButtons; // 把所有的按钮放到数组里，比如第一个是"Start"，第二个是"Exit"
    private int selectedIndex = 0; // 当前选中的按钮索引

    void Start()
    {
        UpdateButtonVisuals();
    }

    void Update()
    {
        if (!StarterAssets.StarterAssetsInputs.inputEnabled)
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
                Debug.Log("ENTER Pressed! Current selection: " + selectedIndex);

                if (selectedIndex == 0)
                {
                    // 第一个按钮，开始游戏
                    StarterAssets.StarterAssetsInputs.inputEnabled = true;
                    foreach (var button in menuButtons)
                    {
                        button.SetActive(false);
                    }
                    FindObjectOfType<TimerManager>().StartTimer();
                }
                else if (selectedIndex == 1)
                {
                    // 第二个按钮，暂时不做任何事情
                    Debug.Log("Second button selected, no action yet.");
                }
            }
        }
    }

    void UpdateButtonVisuals()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            var image = menuButtons[i].GetComponent<Image>();
            if (image != null)
            {
                if (i == selectedIndex)
                    image.color = Color.yellow; // 选中的按钮变黄色
                else
                    image.color = Color.white; // 其他按钮恢复白色
            }
        }
    }
}
