using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class LockTransform : MonoBehaviour
    {
        void Update()
        {
            if (transform.parent) transform.SetParent(null);
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
        }
    }
}
