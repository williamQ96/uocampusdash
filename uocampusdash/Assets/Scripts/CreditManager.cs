using UnityEngine;
using TMPro;

public class CreditManager : MonoBehaviour
{
    public static CreditManager Instance; // 单例模式，保证全局唯一
    public int credits = 0; // 当前总Credits
    public TextMeshProUGUI creditText; // 连接到UI上显示

    void Awake()
    {
        // 单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景也不会消失（虽然现在我们只有一个场景）
        }
        else
        {
            Destroy(gameObject); // 避免重复
        }
    }

    void Start()
    {
        UpdateCreditUI();
    }

    public void AddCredits(int amount)
    {
        credits += amount;
        UpdateCreditUI();
    }

    public void UpdateCreditUI()
    {
        if (creditText != null)
        {
            creditText.text = "Credits: " + credits.ToString();
        }
    }

    void Update()
    {
        UpdateCreditUI();
    }
}
