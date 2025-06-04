using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform player;
    public Transform spawnPoint;

    void Start()
    {
        if (player != null && spawnPoint != null)
        {
            player.position = spawnPoint.position;
            player.rotation = spawnPoint.rotation;
        }
    }
}

