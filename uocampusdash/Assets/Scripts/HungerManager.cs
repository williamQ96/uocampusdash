using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HungerManager : MonoBehaviour
{
    public Slider hungerSlider;
    public TextMeshProUGUI hungerText;
    public TimerManager timerManager;
    public MissionManager missionManager;

    public float maxHunger = 100f;
    private float currentHunger;

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
            Debug.Log($"✅ IsMissionActive: {MissionManager.Instance.IsMissionActive}");
        }

        if (MissionManager.Instance != null && MissionManager.Instance.IsMissionActive)
        {
        
          float remainingTime = timerManager != null ? timerManager.GetRemainingTime() : 60f;
          int level = MissionManager.Instance.GetCurrentLevel(); 

          // Linear interpolation
          float hungerRate = Mathf.Lerp(0.05f, 0.5f, level / 10f); // Level higher - hunger increases
          currentHunger += hungerRate * Time.deltaTime * 5f;
          currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

          // Update the percentage
          hungerSlider.value = currentHunger;

          float percent = (currentHunger / maxHunger) * 100f;
          if (hungerText != null)
              hungerText.text = $"Hunger: {percent:F0}%";
        }
    }

    public void ResetHunger()
    {
        currentHunger = 0f;
        if (hungerSlider != null)
            hungerSlider.value = currentHunger;
    }

}
