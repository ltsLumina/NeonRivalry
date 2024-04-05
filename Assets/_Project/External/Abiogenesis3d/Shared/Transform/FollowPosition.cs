using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class FollowPosition : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;

        void LateUpdate()
        {
            if (!target) return;
            transform.position = target.position + offset;
        }
    }
}
