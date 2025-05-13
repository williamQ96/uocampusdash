using UnityEngine;

public class Coin : MonoBehaviour
{
    private int creditValue;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private AudioClip collectionSound;
    [SerializeField] private GameObject collectionEffect;

    public void SetCreditValue(int value)
    {
        creditValue = value;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerArmature"))
        {
            CreditManager.Instance.AddCredits(creditValue);

            if (collectionSound != null)
            {
                AudioSource.PlayClipAtPoint(collectionSound, transform.position);
            }

            if (collectionEffect != null)
            {
                Instantiate(collectionEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
