using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    // NOTE: needs to be executed before UPixelator
    [DefaultExecutionOrder(int.MaxValue - 101)]
    public class FollowTransform : MonoBehaviour
    {
        public Transform target;
        public bool followPosition = true;
        public bool followRotation = true;

        private void LateUpdate()
        {
            if (!target) return;

            if (followPosition && followRotation)
                transform.SetPositionAndRotation(target.position, target.rotation);
            else
            {
                if (followPosition) transform.position = target.position;
                if (followRotation) transform.rotation = target.rotation;
            }
        }
    }
}
