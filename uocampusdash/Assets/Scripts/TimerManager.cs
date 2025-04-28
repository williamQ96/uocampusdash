using UnityEngine;
using TMPro; // 使用TextMeshPro

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText; // 计时器文本UI
    public TextMeshProUGUI gameOverText; // Game Over文本UI
    public float timeLimit = 60f; // 一分钟
    private float remainingTime;
    private bool timerRunning = false;

    void Start()
    {
        remainingTime = timeLimit;
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
        remainingTime = timeLimit;
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
    }
}
