using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HungerManager : MonoBehaviour
{
    public Slider hungerSlider;
    public TextMeshProUGUI hungerText;
    public TimerManager timerManager;
    public MissionManager missionManager;
    public TextMeshProUGUI gameOverText;

    public float maxHunger = 100f;
    private float currentHunger;
    private bool hasTriggeredHungerGameOver = false;
    public float hungerReductionFactor = 1.0f;

    void Start()
    {
        if (hungerSlider != null)
        {
            hungerSlider.maxValue = maxHunger;
            hungerSlider.value = 0;
        }

        if (hungerText != null)
            hungerText.text = "Hunger: 0%";
    }

    void Update()
    {
        Debug.Log($"🔥 Update() Called. MissionManager.Instance = {MissionManager.Instance}");

        if (MissionManager.Instance != null)
        {
            // Debug.Log($"✅ IsMissionActive: {MissionManager.Instance.IsMissionActive}");
        }

        if (MissionManager.Instance != null && MissionManager.Instance.IsMissionActive)
        {
        
          float remainingTime = timerManager != null ? timerManager.GetRemainingTime() : 60f;
          int level = MissionManager.Instance.GetCurrentLevel(); 

          // Linear interpolation
          float baseRate = Mathf.Lerp(0.05f, 0.5f, level / 10f); // Level higher - hunger increases

          float adjustedRate = baseRate * (FoodManager.Instance != null ? FoodManager.Instance.hungerReductionFactor : 1.0f);

          currentHunger += adjustedRate * Time.deltaTime * 5f;
          currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

          // Update the percentage
          hungerSlider.value = currentHunger;
          float percent = (currentHunger / maxHunger) * 100f;
          if (hungerText != null)
              hungerText.text = $"Hunger: {percent:F0}%";

          // Game Over if hunger is max and mission is still active
          if (currentHunger >= maxHunger && MissionManager.Instance.IsMissionActive)
          {
              Debug.Log("💀 Hunger reached 100%! Game Over due to starvation.");

            if (gameOverText != null)
            {
                gameOverText.text = "You starved!";
                gameOverText.gameObject.SetActive(true);
            }

            var timer = FindObjectOfType<TimerManager>();
            if (timer != null) timer.enabled = false;

            MissionManager.Instance.OnMissionFailure();
          }
        }
    }

    public void ResetHunger()
    {
        currentHunger = 0f;
        hungerReductionFactor = 1.0f; // Reset to the normal hungry speed
        if (hungerSlider != null)
            hungerSlider.value = currentHunger;
    }

    // After eating food from restaurant
    public void ReduceHungerRate(float factor)
    {
        hungerReductionFactor = factor;
        Debug.Log($"✅ Hunger reduction factor set to {factor}");
    }


}
