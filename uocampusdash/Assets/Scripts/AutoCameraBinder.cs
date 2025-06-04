using UnityEngine;
using Cinemachine;

public class AutoCameraBinder : MonoBehaviour
{
    public string playerTag = "Player";
    public string cameraRootName = "PlayerCameraRoot";

    void Start()
    {
        StartCoroutine(DelayedBind());
    }

    System.Collections.IEnumerator DelayedBind()
    {
        yield return new WaitForSeconds(0.2f); 

        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam == null)
        {
            Debug.LogError("❌ No CinemachineVirtualCamera component found!");
            yield break;
        }

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError("❌ No player with tag 'Player' found.");
            yield break;
        }

        Transform cameraRoot = player.transform.Find(cameraRootName);
        if (cameraRoot == null)
        {
            Debug.LogError($"❌ Could not find child transform named '{cameraRootName}' under player.");
            yield break;
        }

        vcam.Follow = cameraRoot;
        vcam.LookAt = cameraRoot;

        Debug.Log("✅ Cinemachine camera successfully bound to player.");
    }
}
