using UnityEngine;

public enum FoodType { Burger, Fries, Drinks }

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance;

    [Range(0f, 1f)]
    public float hungerReductionFactor = 1.0f; // 1 = no reduction, closer to 0 = stronger reduction

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectFood(FoodType type)
    {
        if (CreditManager.Instance == null)
        {
            Debug.LogError("❌ CreditManager.Instance not found.");
            return;
        }

        switch (type)
        {
            case FoodType.Burger:
                if (CreditManager.Instance.credits >= 150)
                {
                    CreditManager.Instance.AddCredits(-150);
                    hungerReductionFactor = 0.5f;
                    Debug.Log("🍔 Bought Burger, hunger rate reduced by 50%");
                }
                else Debug.Log("❌ Not enough credits for Burger.");
                break;

            case FoodType.Fries:
                if (CreditManager.Instance.credits >= 100)
                {
                    CreditManager.Instance.AddCredits(-100);
                    hungerReductionFactor = 0.75f;
                    Debug.Log("🍟 Bought Fries, hunger rate reduced by 25%");
                }
                else Debug.Log("❌ Not enough credits for Fries.");
                break;

            case FoodType.Drinks:
                if (CreditManager.Instance.credits >= 50)
                {
                    CreditManager.Instance.AddCredits(-50);
                    hungerReductionFactor = 0.85f;
                    Debug.Log("🥤 Bought Drink, hunger rate reduced by 15%");
                }
                else Debug.Log("❌ Not enough credits for Drink.");
                break;
        }
    }
}
