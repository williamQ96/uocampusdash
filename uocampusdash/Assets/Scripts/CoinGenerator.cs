using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] private GameObject GoldCoins;
    [SerializeField] private Transform roadTransform; // Reference to your road

    [SerializeField] private float roadLength = 100f;
    [SerializeField] private float roadWidth = 10f;
    [SerializeField] private int numberOfCoins = 50;
    [SerializeField] private float coinHeight = 1f; // Height above road

    [SerializeField] private bool usePattern = false;
    [SerializeField] private float spacingBetweenCoins = 2f;
    [SerializeField] private int coinsPerRow = 3;

    private void Start()
    {
        if (usePattern)
        {
            GenerateCoinPattern();
        }
        else
        {
            GenerateRandomCoins();
        }
    }

    private void GenerateRandomCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            float posZ = Random.Range(0, roadLength);
            float posX = Random.Range(-roadWidth / 2, roadWidth / 2);
            Vector3 coinPosition = new Vector3(posX, coinHeight, posZ);

            if (roadTransform != null)
            {
                coinPosition = roadTransform.TransformPoint(coinPosition);
            }

            GameObject coin = Instantiate(GoldCoins, coinPosition, Quaternion.identity, transform);

            // Assign random credit value between 10 and 50
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript != null)
            {
                int randomCredit = Random.Range(10, 51); // 51 is exclusive, so 10–50
                coinScript.SetCreditValue(randomCredit);
            }
        }
    }

    private void GenerateCoinPattern()
    {
        float startZ = 10f;

        for (int row = 0; row < numberOfCoins / coinsPerRow; row++)
        {
            for (int col = 0; col < coinsPerRow; col++)
            {
                float posZ = startZ + row * spacingBetweenCoins;
                float posX = (col - (coinsPerRow - 1) / 2.0f) * spacingBetweenCoins;
                Vector3 coinPosition = new Vector3(posX, coinHeight, posZ);

                if (roadTransform != null)
                {
                    coinPosition = roadTransform.TransformPoint(coinPosition);
                }

                GameObject coin = Instantiate(GoldCoins, coinPosition, Quaternion.identity, transform);

                // Assign random credit value between 10 and 50
                Coin coinScript = coin.GetComponent<Coin>();
                if (coinScript != null)
                {
                    int randomCredit = Random.Range(10, 51);
                    coinScript.SetCreditValue(randomCredit);
                }
            }
        }
    }
}
