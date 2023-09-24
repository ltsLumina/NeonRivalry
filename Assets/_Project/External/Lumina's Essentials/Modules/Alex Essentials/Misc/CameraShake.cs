
// Uncomment the "#define" statement below if you are using cinemachine in your project.
//#define USING_CINEMACHINE

#if USING_CINEMACHINE
#region
using Cinemachine;
using UnityEngine;
#endregion

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShake : MonoBehaviour
{
    float shakeTimer;
    
    public static CameraShake Instance { get; private set; }

    CinemachineVirtualCamera cinemachineVCam;

    void Awake()
    { // Create a singleton instance
        Instance = this;

        // Cache the virtual camera from the scene
        cinemachineVCam = GetComponent<CinemachineVirtualCamera>();
    }

    /// <summary>
    /// Camera Shake for Cinemachine.
    /// <remarks>Syntax: CameraShake.instance.ShakeCamera(1.5f, 0.2f);</remarks>
    /// </summary>
    /// <param name="intensity"> The amount the screen shakes.</param>
    /// <param name="time"> Duration the shake occurs for.</param>
    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachinePerlin =
            cinemachineVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachinePerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    void Update()
    {
        if (!(shakeTimer > 0)) return;

        shakeTimer -= Time.deltaTime;
        if (shakeTimer <= 0f) StopCameraShake();
    }

    void StopCameraShake()
    {
        CinemachineBasicMultiChannelPerlin cinemachinePerlin =
            cinemachineVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachinePerlin.m_AmplitudeGain = 0f;
    }
}
#endif