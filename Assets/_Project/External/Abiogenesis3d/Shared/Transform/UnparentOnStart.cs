using UnityEngine;

namespace Abiogenesis3d
{
    // Use this for objects that you want to add to prefab but that shouldn't be nested
    public class UnparentOnStart : MonoBehaviour
    {
        void Start()
        {
            transform.parent = null;
        }
    }
}
