using UnityEngine;
using System.Collections;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform spawnPoint; // assign in Inspector

    void Start()
{
    StartCoroutine(DelayedPositioning());
}

IEnumerator DelayedPositioning()
{
    yield return null;

    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null)
    {
        Debug.LogWarning("❗ Player not found!");
        yield break;
    }

    MuseumRoomBuilder builder = FindObjectOfType<MuseumRoomBuilder>();
    if (builder == null)
    {
        Debug.LogWarning("❌ MuseumRoomBuilder not found in scene.");
        yield break;
    }

    // 取得地板中心與高度
    Vector3 spawnPosition = builder.FloorCenter + Vector3.up * 0.1f;

    CharacterController controller = player.GetComponent<CharacterController>();
    if (controller != null) controller.enabled = false;

    player.transform.position = spawnPosition;
    player.transform.rotation = Quaternion.identity;

    if (controller != null) controller.enabled = true;

    Debug.Log("✅ Player positioned at floor center.");
    }

}
