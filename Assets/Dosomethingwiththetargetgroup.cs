using UnityEngine;
using Cinemachine;

public class AdjustTargetGroupPosition : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;
    public float zOffsetMultiplier = 1f; // Multiplier for z-offset based on x-distance

    void Update()
    {
        if (targetGroup != null)
        {
            // Calculate the average x-position of all targets
            Vector3 averagePosition = Vector3.zero;
            foreach (var target in targetGroup.m_Targets)
            {
                averagePosition += target.target.position;
            }
            averagePosition /= targetGroup.m_Targets.Length;

            // Set the z-position of the CinemachineTargetGroup based on the average x-position
            Vector3 newPosition = targetGroup.transform.position;
            newPosition.z = averagePosition.x * zOffsetMultiplier;
            transform.position = newPosition;
        }
    }
}