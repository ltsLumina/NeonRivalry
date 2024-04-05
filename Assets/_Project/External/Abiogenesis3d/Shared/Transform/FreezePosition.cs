using UnityEngine;

namespace Abiogenesis3d
{
    [DefaultExecutionOrder(int.MinValue)]
    public class FreezePosition : MonoBehaviour
    {
        public Vector3 position;

        void OnEnable()
        {
            position = transform.position;
        }

        void Update()
        {
            position = transform.position;
        }

        void LateUpdate()
        {
            transform.position = position;
        }
    }
}
