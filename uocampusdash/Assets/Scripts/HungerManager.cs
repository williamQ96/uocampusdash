using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    private bool hungerFrozen = false;

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
        if (MissionManager.Instance != null && MissionManager.Instance.IsMissionActive && !hungerFrozen)
        {
            float remainingTime = timerManager != null ? timerManager.GetRemainingTime() : 60f;
            int level = MissionManager.Instance.GetCurrentLevel();

            // Linear interpolation: level 1 = slow hunger, level 10 = fast
            float baseRate = Mathf.Lerp(0.05f, 0.5f, level / 10f);
            float adjustedRate = baseRate * hungerReductionFactor;

            currentHunger += adjustedRate * Time.deltaTime * 5f;
            currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

            Debug.Log($"\ud83d\udcc9 Current Hunger Speed Multiplier: x{hungerReductionFactor:F2}");

            if (hungerSlider != null)
                hungerSlider.value = currentHunger;

            float percent = (currentHunger / maxHunger) * 100f;
            if (hungerText != null)
                hungerText.text = $"Hunger: {percent:F0}%";

            if (currentHunger >= maxHunger && !hasTriggeredHungerGameOver)
            {
                hasTriggeredHungerGameOver = true;
                Debug.Log("\ud83d\udc80 Hunger reached 100%! Game Over due to starvation.");

                if (gameOverText != null)
                {
                    gameOverText.text = "You starved!";
                    gameOverText.gameObject.SetActive(true);
                }

                if (timerManager != null)
                    timerManager.enabled = false;

                MissionManager.Instance.OnMissionFailure();
            }
        }
    }

    public void ResetHunger()
    {
        currentHunger = 0f;
        hungerReductionFactor = 1.0f;
        hasTriggeredHungerGameOver = false;

        if (hungerSlider != null)
            hungerSlider.value = currentHunger;

        if (hungerText != null)
            hungerText.text = "Hunger: 0%";
    }

    // Called when food is consumed — temporary effect
    public void ReduceHungerRateTemporary(float factor, float duration)
    {
        StopAllCoroutines(); // prevent stacking multiple effects
        StartCoroutine(ReduceTemporarily(factor, duration));
    }

    private IEnumerator ReduceTemporarily(float factor, float duration)
    {
        hungerReductionFactor = factor;
        Debug.Log($"\ud83c\udf7d Hunger speed temporarily reduced to x{factor} for {duration} sec");

        yield return new WaitForSeconds(duration);

        hungerReductionFactor = 1.0f;
        Debug.Log("\ud83d\udd01 Hunger speed reset to normal (x1.0)");
    }

    public void SetHungerFrozen(float duration)
    {
        StartCoroutine(FreezeHunger(duration));
    }

    private IEnumerator FreezeHunger(float duration)
    {
        hungerFrozen = true;
        Debug.Log("\u2744\ufe0f Hunger is now frozen.");
        yield return new WaitForSeconds(duration);
        hungerFrozen = false;
        Debug.Log("\ud83d\udd01 Hunger is unfrozen.");
    }
}
