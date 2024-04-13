using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class CameraParent : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 5f;

    [Tab("Settings")]
    [SerializeField] Transform target1;
    [SerializeField] Transform target2;


    // -- Cached References -- \\\

    CinemachineVirtualCamera vCam;

    void Awake() => vCam = GetComponentInChildren<CinemachineVirtualCamera>();

    // Update is called once per frame
    void Update()
    {
        // Get the current position of the camera.
        Vector3 currentPosition = vCam.transform.position;

        // Calculate the midpoint between the two targets.
        Vector3 midpoint = (target1.position + target2.position) / 2f;
        Vector3 directionToMidpointY = new Vector3(currentPosition.x, midpoint.y, 0) - currentPosition; // Only consider y-axis

        // Smoothly rotate camera towards midpoint
        Quaternion targetRotation = Quaternion.LookRotation(directionToMidpointY);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
