using UnityEngine;
using TMPro; // 使用TextMeshPro

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText; // 计时器文本UI
    public TextMeshProUGUI gameOverText; // Game Over文本UI
    public float minTimeLimit = 20f; // 最小时间限制 (20秒)
    public float maxTimeLimit = 80f; // 最大时间限制 (1分20秒)
    private float remainingTime;
    private bool timerRunning = false;

    void Start()
    {
        remainingTime = GetRandomTimeLimit();
        timerText.gameObject.SetActive(false); // 一开始隐藏计时器
        gameOverText.gameObject.SetActive(false); // 一开始隐藏Game Over
    }

    void Update()
    {
        if (timerRunning)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime > 0)
            {
                timerText.text = "Time: " + Mathf.CeilToInt(remainingTime).ToString();
            }
            else
            {
                EndGame();
            }
        }
    }

    public void StartTimer()
    {
        remainingTime = GetRandomTimeLimit();
        timerRunning = true;
        timerText.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
    }

    private void EndGame()
    {
        timerRunning = false;
        timerText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = "Game Over!";

        FindObjectOfType<MissionManager>().ShowCompletePanel();
    }

    public void ResetTimer()
    {
        remainingTime = GetRandomTimeLimit();
        timerRunning = false;
        timerText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        enabled = true; // 重新激活Timer脚本
    }

    // Method to get a random time between 20s and 80s
    private float GetRandomTimeLimit()
    {
        return Random.Range(minTimeLimit, maxTimeLimit);
    }
}
