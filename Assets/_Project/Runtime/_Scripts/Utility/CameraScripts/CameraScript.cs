#region
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
#endregion

public class CameraScript : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCameraPrefab;
    public Transform player;
    public List<Transform> waypoints = new();

    CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        virtualCamera = Instantiate(virtualCameraPrefab);
        virtualCamera.transform.position = GetComponent<Camera>().transform.position;

        virtualCamera.LookAt = player;

        var trackedDolly = virtualCamera.AddCinemachineComponent<CinemachineTrackedDolly>();

        var smoothPath = new GameObject("DollyTrack").AddComponent<CinemachineSmoothPath>();
        smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[waypoints.Count];

        for (int i = 0; i < waypoints.Count; i++)
        {
            smoothPath.m_Waypoints[i] = new();
            smoothPath.m_Waypoints[i].position = waypoints[i].position;
        }

        trackedDolly.m_Path = smoothPath;
        trackedDolly.m_PathPosition = 0;
    }
}
