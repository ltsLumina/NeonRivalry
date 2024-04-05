using UnityEngine;

namespace Abiogenesis3d
{
    public partial class Utils
    {
        public static float GetCamHeightRatio(Camera cam, Transform distanceTarget)
        {
            float camHeight = cam.orthographicSize * 2;
            if (!cam.orthographic)
            {
                // TODO: test perspective
                float camDistance = Vector3.Distance(cam.transform.position, distanceTarget.position);
                camHeight = 2 * camDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            }
            return camHeight / Screen.height;
        }
    }
}
