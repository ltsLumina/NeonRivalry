using Cinemachine;
using UnityEngine;

public class VCamScriptName : MonoBehaviour
{
    [SerializeField] Transform target1;
    [SerializeField] Transform target2;
    [SerializeField] Transform followTarget;
    public float minDistance = 5f; // Minimum distance between the players
    public float maxDistance = 10f; // Maximum distance between the players
    public float minZPosition = -10f; // Minimum z position of the camera
    public float maxZPosition = -5f; // Maximum z position of the camera
    public float zoomSpeed = 5f; // Zoom speed
    public float borderPadding = 1f; // Padding to add around the camera's view
    CinemachineVirtualCamera vCam;
    BoxCollider borderCollider;
    Vector3 startPosition;
    //Vector3 midpoint;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        vCam = GetComponent<CinemachineVirtualCamera>();
        borderCollider = GetComponent<BoxCollider>();
        //vCam.m_Follow = followTarget;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Temp();
    }

    void Temp()
    {
        // Calculate the distance between the two players
        float distance = Vector3.Distance(target1.position, target2.position);

        // Calculate the desired z-position based on the distance
        float desiredZ = Mathf.Lerp(maxZPosition, minZPosition, Mathf.InverseLerp(minDistance, maxDistance, distance));

        // Smoothly adjust the z-position of the Virtual Camera
        Vector3 currentPosition = vCam.transform.position;
        Vector3 midpoint = (target1.position + target2.position) / 2;
        float newZ = Mathf.Lerp(currentPosition.z, desiredZ, Time.deltaTime * zoomSpeed);
        vCam.transform.position = new Vector3(midpoint.x, currentPosition.y, newZ);
    }
}
