using UnityEngine;

namespace Abiogenesis3d
{
    public class MultiCameraRaycastTest : MonoBehaviour
    {
        public MultiCameraEvents multiCameraEvents;

        void Start()
        {
            if (!multiCameraEvents)
                multiCameraEvents = FindObjectOfType<MultiCameraEvents>();
        }

        void Update()
        {
            var hit = multiCameraEvents.raycastHit;
            Debug.Log((hit.collider?.name ?? "null") + ", " + hit.point);
        }
    }
}
