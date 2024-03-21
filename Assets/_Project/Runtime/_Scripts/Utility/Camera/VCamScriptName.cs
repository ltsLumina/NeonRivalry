using UnityEngine;
using Cinemachine;

public class VCamScriptName : MonoBehaviour
{
    [SerializeField] Transform target1;
    [SerializeField] Transform target2;
    [SerializeField] Transform followTarget;
    [SerializeField] Vector3 maxPosition;
    CinemachineVirtualCamera vCam;
    Vector3 startPosition;
    private Vector3 midpoint;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        vCam = GetComponent<CinemachineVirtualCamera>();
        //vCam.m_Follow = followTarget;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Temp();
    }

    private void Temp()
{
    // Calculate the midpoint between target1 and target2
    Vector3 midpoint = (target1.position + target2.position) / 2f;

    Debug.Log(midpoint.x);

    // Set the x-position of the follow target based on the x-position of the midpoint
    followTarget.position = new Vector3(midpoint.x, followTarget.position.y, followTarget.position.z);
}
}
