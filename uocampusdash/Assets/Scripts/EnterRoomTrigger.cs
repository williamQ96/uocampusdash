using UnityEngine;

public class EnterRoomTrigger : MonoBehaviour
{
    public GameObject roomInterior;       // Assign restaurant in Inspector 
    public GameObject buildingExterior;   // Disable exterior when entering the room

    private bool canEnter = false;

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.E)) // press E to trigger enter room
        {
            Debug.Log("Player pressed E to enter the room");

            roomInterior.SetActive(true);             // Show the room
            if (buildingExterior != null)
                buildingExterior.SetActive(false);    // Hide the outside 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            canEnter = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canEnter = false;
    }
}
