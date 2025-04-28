using UnityEngine;
using UnityEngine.UI; // 加载Button组件
using UnityEngine.SceneManagement; 
using TMPro; 

public class GameUIManager : MonoBehaviour
{
    public GameObject[] menuButtons; // 把所有的按钮放到数组里，比如第一个是"Start"，第二个是"Exit"
    private int selectedIndex = 0; // 当前选中的按钮索引
    public TextMeshProUGUI exitText;
    public GameObject storePanel;

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
                    FindObjectOfType<MissionManager>().StartMission();
                }
                else if (selectedIndex == 1)
                {
                    Debug.Log("Store selected.");

                    if (storePanel != null)
                    {
                        storePanel.SetActive(true); // 打开Store窗口
                        foreach (var button in menuButtons)
                        {
                            button.SetActive(false); // 隐藏Start/Store/Exit按钮
                        }
                    }
                    else
                    {
                        Debug.LogError("StorePanel is not assigned!");
                    }
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

    public void ShowMainMenu()
{
    StarterAssets.StarterAssetsInputs.inputEnabled = false; // 禁用玩家移动
    selectedIndex = 0; // 选中Start
    foreach (var button in menuButtons)
    {
        button.SetActive(true); // 把Start/Store/Exit按钮重新显示出来
    }
    UpdateButtonVisuals(); // 刷新选中高亮
}

public void ExitGame()
{
    Debug.Log("[GameUIManager] ExitGame called.");

#if UNITY_WEBGL
    // WebGL版本无法直接退出，显示感谢文本
    if (exitText != null)
    {
        exitText.gameObject.SetActive(true);
        exitText.text = "Thanks for playing!";
    }
#else
    // 其他版本可以直接退出
    Application.Quit();
#endif
}

}
