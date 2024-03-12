using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCamScriptName : MonoBehaviour
{
    [SerializeField] Transform target1;
    [SerializeField] Transform target2;
    [SerializeField] Transform followTarget;
    CinemachineVirtualCamera vCam;
    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = followTarget.position;
        vCam = GetComponent<CinemachineVirtualCamera>();
        vCam.m_Follow = followTarget;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float target = (target1.position.x - target2.position.x)/2 * 10;
        vCam.m_Lens.FieldOfView = target;

        // Calculate the midpoint between pointA and pointB
        Vector3 midpoint = (target1.position + target2.position) / 2f;

        // Set the position of the virtual camera to the midpoint
        followTarget.position = midpoint;

        if (midpoint.z >= startPosition.z)
        {
            followTarget.position = startPosition;
        }
    }
}
