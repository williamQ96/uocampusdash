using UnityEngine;
using TMPro; // For TextMeshPro UI elements

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText; // UI element to display the countdown timer
    public TextMeshProUGUI gameOverText; // UI element to display "Game Over" message
    public float timeLimit = 60f; // Time limit in seconds

    private float remainingTime;
    private bool timerRunning = false;

    void Start()
    {
        remainingTime = timeLimit;
        timerText.gameObject.SetActive(false); // Hide the timer text at the start
        gameOverText.gameObject.SetActive(false); // Hide the Game Over text at the start
    }

    void Update()
    {
        if (timerRunning)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime > 0)
            {
                // Update timer text with the ceiling of remaining time
                timerText.text = "Time: " + Mathf.CeilToInt(remainingTime).ToString();
            }
            else
            {
                // Time ran out
                EndGame();  
            }
        }
    }

    //  Starts or restarts the countdown timer
    public void StartTimer()
    {
        remainingTime = timeLimit;
        timerRunning = true;

        timerText.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
    }

    // Ends the game when time runs out
    private void EndGame()
    {
        timerRunning = false;
        timerText.gameObject.SetActive(false);

        gameOverText.gameObject.SetActive(true);
        gameOverText.text = "Game Over!";

        // Show mission complete panel even when time runs out
        FindObjectOfType<MissionManager>().ShowCompletePanel();
    }

    // Resets the timer state and hides UI
    public void ResetTimer()
    {
    remainingTime = timeLimit;
    timerRunning = false;

    timerText.gameObject.SetActive(false);
    gameOverText.gameObject.SetActive(false);

    enabled = true; // Reactivate the script in case it was disabled
    }
}
