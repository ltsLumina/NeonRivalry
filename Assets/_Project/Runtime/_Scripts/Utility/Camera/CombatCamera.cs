using Cinemachine;
using UnityEngine;

public class CombatCamera : MonoBehaviour
{
    [SerializeField] Transform target1;
    [SerializeField] Transform target2;
    public float minDistance = 5f; // Minimum distance between the players
    public float maxDistance = 10f; // Maximum distance between the players
    public float minZPosition = -10f; // Minimum z position of the camera
    public float maxZPosition = -5f; // Maximum z position of the camera
    public float zoomSpeed = 5f; // Zoom speed
    CinemachineVirtualCamera vCam;

    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CenterandZoom();
    }

    void CenterandZoom()
    {
        // Calculate the distance between the two players
        float distance = Vector3.Distance(target1.position, target2.position);

        // Calculate the desired z-position based on the distance
        float desiredZ = Mathf.Lerp(maxZPosition, minZPosition, Mathf.InverseLerp(minDistance, maxDistance, distance));

        // Smoothly adjust the x- and z-position of the Virtual Camera
        Vector3 currentPosition = vCam.transform.position;
        Vector3 midpoint = (target1.position + target2.position) / 2;
        float newZ = Mathf.Lerp(currentPosition.z, desiredZ, Time.deltaTime * zoomSpeed);
        vCam.transform.position = new Vector3(midpoint.x, currentPosition.y, newZ);
    }
}
