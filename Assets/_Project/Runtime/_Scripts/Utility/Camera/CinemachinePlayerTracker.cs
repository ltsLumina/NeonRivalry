using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

// Previously called 'CameraScript'.
public class CinemachinePlayerTracker : MonoBehaviour
{
    // -- Inspector Fields --
    
    [SerializeField] CinemachineVirtualCamera virtualCameraPrefab;
    [SerializeField] Transform player;
    [SerializeField] List<Transform> waypoints = new ();
    
    // -- Cached References --
    
    CinemachineVirtualCamera virtualCamera;
    
    void Start()
    {
        virtualCamera = SetupVirtualCamera();
        var trackedDolly = SetupTrackedDolly();
        SetupSmoothPath(trackedDolly);
    }

    CinemachineVirtualCamera SetupVirtualCamera()
    {
        var vCam = Instantiate(virtualCameraPrefab);
        vCam.transform.position = GetComponent<Camera>().transform.position;
        vCam.LookAt = player;
        return vCam;
    }

    CinemachineTrackedDolly SetupTrackedDolly() => virtualCamera.AddCinemachineComponent<CinemachineTrackedDolly>();

    void SetupSmoothPath(CinemachineTrackedDolly trackedDolly)
    {
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