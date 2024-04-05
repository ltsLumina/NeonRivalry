using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(MultiCameraEventsIgnore))]
    public class ParallaxFollowMultiplier : MonoBehaviour
    {
        public Transform target;

        [Header("Position")]
        public bool followPosition = true;
        public Vector3 positionOffset;
        [Range(0f, 1f)] public float positionMultiplier = 1f;

        [Header("Rotation")]
        public bool followRotation = true;
        public Vector3 rotationOffset;
        // public Vector3 rotationMultiplier = Vector3.one;

        [Header("Offsets")]

        Vector3 initialTargetPosition;
        Quaternion initialTargetRotation;

        public bool reInitialize;

        UPixelator uPixelator;

        void Start()
        {
            Init();
        }

        void Init()
        {
            initialTargetPosition = target.position;
            initialTargetRotation = target.rotation;
        }

        void LateUpdate()
        {
            if (!target) return;

            if (!uPixelator) uPixelator = FindObjectOfType<UPixelator>();

            if (reInitialize)
            {
                reInitialize = false;
                Init();
            }

            if (followPosition)
            {
                var newPosition = positionOffset + initialTargetPosition + (target.position - initialTargetPosition) * positionMultiplier;
                var dist = Vector3.Distance(transform.position, newPosition);
                transform.position = newPosition;
            }

            if (followRotation)
            {
                // Quaternion resultRotation = Quaternion.identity;

                var diff = target.rotation * Quaternion.Inverse(initialTargetRotation);

                // all
                // resultRotation = Quaternion.Lerp(initialTargetRotation, target.rotation, rotationMultiplier.x);

                // Y
                // Quaternion yRotation = Quaternion.Euler(0f, target.rotation.eulerAngles.y, 0f);
                // resultRotation *= Quaternion.Lerp(initialTargetRotation, yRotation, rotationMultiplier.y);
                // // X
                // Quaternion xRotation = Quaternion.Euler(target.rotation.eulerAngles.x, 0f, 0f);
                // resultRotation *= Quaternion.Lerp(initialTargetRotation, xRotation, rotationMultiplier.x);
                // // Z
                // Quaternion zRotation = Quaternion.Euler(0f, 0f, target.rotation.eulerAngles.z);
                // resultRotation *= Quaternion.Lerp(initialTargetRotation, zRotation, rotationMultiplier.z);

                // var newRotation = Quaternion.Lerp(initialTargetRotation, diff, rotationMultiplier.x) * initialTargetRotation;
                var newRotation = diff * initialTargetRotation;
                transform.rotation = Quaternion.Euler(rotationOffset + newRotation.eulerAngles);
            }

        }
    }
}
