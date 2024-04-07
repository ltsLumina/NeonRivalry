#region
using Cinemachine;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
#endregion

public class CameraController : MonoBehaviour
{
    [Tab("Camera")]
    [Tooltip("The minimum distance between the players.")]
    [SerializeField] float minDistance = 5f;

    [Tooltip("The maximum distance between the players.")]
    [SerializeField] float maxDistance = 10f;

    [Tooltip("The minimum z-position of the camera.")]
    [SerializeField] float minZPosition = -10f;

    [Tooltip("The maximum z-position of the camera.")]
    [SerializeField] float maxZPosition = -5f;

    [Tooltip("The speed at which the camera zooms in and out.")]
    [SerializeField] float zoomSpeed = 50f;

    [Tab("Settings")]
    [SerializeField, ReadOnly] Transform target1;
    [SerializeField, ReadOnly] Transform target2;

    // -- Cached References -- \\

    CinemachineVirtualCamera vCam;

    void Awake() => vCam = GetComponentInChildren<CinemachineVirtualCamera>();

    void OnEnable() => InputDeviceManager.OnPlayerJoin += OnPlayerJoined;
    void OnDisable() => InputDeviceManager.OnPlayerJoin -= OnPlayerJoined;

    void OnPlayerJoined(PlayerInput playerInput, int playerID)
    {
        // Get the PlayerController transform of the joined player
        Transform playerTransform = playerInput.GetComponentInParent<PlayerController>().transform;

        // Assign the playerTransform to target1 or target2 based on their current values
        // and ensure that the playerTransform is not already assigned to the other target
        if (target1      == null && target2 != playerTransform) target1 = playerTransform;
        else if (target2 == null && target1 != playerTransform) target2 = playerTransform;
    }

    void LateUpdate() => Follow();

    /// <summary>
    /// The Follow method is responsible for adjusting the position of the camera based on the positions of two targets (players).
    /// </summary>
    void Follow()
    {
        // If either target is null, the method returns immediately without executing the rest of the code.
        if (target1 == null || target2 == null) return;

        // Calculate the distance between the two targets (players).
        float distance = Vector3.Distance(target1.position, target2.position);

        // Calculate the desired z-position of the camera based on the distance between the two targets.
        // The desired z-position is a linear interpolation between the maximum and minimum z-positions,
        // with the interpolation parameter being the inverse lerp of the distance and the minimum and maximum distances.
        float desiredZ = Mathf.Lerp(maxZPosition, minZPosition, Mathf.InverseLerp(minDistance, maxDistance, distance));

        // Get the current position of the camera.
        Vector3 currentPosition = vCam.transform.position;

        // Calculate the midpoint between the two targets.
        const float half     = 2;
        Vector3     midpoint = (target1.position + target2.position) / half;

        // Calculate the new z-position of the camera. This is a linear interpolation between the current z-position and the desired z-position,
        // with the interpolation parameter being the product of the time delta and the zoom speed.
        float newZ = Mathf.Lerp(currentPosition.z, desiredZ, Time.deltaTime * zoomSpeed);

        // Set the new position of the camera. The x-position is the x-coordinate of the midpoint, the y-position is the current y-position,
        // and the z-position is the newly calculated z-position.
        vCam.transform.position = new (midpoint.x, currentPosition.y, newZ);
    }
}