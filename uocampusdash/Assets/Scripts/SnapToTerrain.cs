using UnityEngine;

public class SnapToTerrain : MonoBehaviour
{
    public float raycastHeight = 100f;

    void Start()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * raycastHeight;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastHeight * 2))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }
}
