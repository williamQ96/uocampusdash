using UnityEngine;
using Cinemachine;

public class FreeLookZoom : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    [Min(0)] public float zoomSpeed = 2f;
    [Min(0)] public float minRadius = 2f, maxRadius = 15f;

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            for (int i = 0; i < freeLookCam.m_Orbits.Length; i++)
            {
                var o = freeLookCam.m_Orbits[i];
                o.m_Radius = Mathf.Clamp(o.m_Radius - scroll * zoomSpeed, minRadius, maxRadius);
                freeLookCam.m_Orbits[i] = o;
            }
        }
    }
}
