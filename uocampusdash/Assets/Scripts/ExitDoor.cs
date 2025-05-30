using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public Transform returnSpawnPoint; // where player should appear outside
    public Animator doorAnimator;      // attach door animation controller
    public AudioSource doorSound;      // attach AudioSource with open sound
    private bool playerInZone = false;
    private GameObject player;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
            TeleportPlayer();
        }
    }

    void OpenDoor()
    {
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (doorSound != null)
            doorSound.Play();
    }

    void TeleportPlayer()
    {
        if (player != null)
        {
            Vector3 returnPosition = PlayerReturnPosition.LastOutsidePosition;

            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = returnPosition;
                controller.enabled = true;
            }
            else
            {
                player.transform.position = returnPosition;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            player = null;
        }
    }
}
