using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCameraPrefab;
    public Transform tPlayer;
    public Transform tFollowTarget;
    public Transform[] tWaypoints;

    private Camera _camera;
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        _camera = Camera.main;
        virtualCamera = Instantiate(virtualCameraPrefab);
        var brain = GetComponent<Camera>().gameObject.AddComponent<CinemachineBrain>();
        virtualCamera.transform.position = GetComponent<Camera>().transform.position;

        virtualCamera.LookAt = tPlayer;

        CinemachineTrackedDolly trackedDolly = virtualCamera.AddCinemachineComponent<CinemachineTrackedDolly>();

        CinemachineSmoothPath smoothPath = new GameObject("DollyTrack").AddComponent<CinemachineSmoothPath>();
        smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[tWaypoints.Length];
        for (int i = 0; i < tWaypoints.Length; i++)
        {
            smoothPath.m_Waypoints[i] = new CinemachineSmoothPath.Waypoint();
            smoothPath.m_Waypoints[i].position = tWaypoints[i].position;
        }

        trackedDolly.m_Path = smoothPath;
        trackedDolly.m_PathPosition = 0;
    }
}