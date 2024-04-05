using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class FollowDelayed : MonoBehaviour
    {
        public Transform target;
        public float speed = 10;

        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);

#if UNITY_EDITOR
            if (!Application.isPlaying) transform.position = target.position;
#endif
        }
    }
}
