using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class MirrorCamera : MonoBehaviour
    {
        public Camera cam;
        public Camera target;

        void Update()
        {
            if (!cam || !target) return;

            cam.orthographic = target.orthographic;
            cam.orthographicSize = target.orthographicSize;
            cam.fieldOfView = target.fieldOfView;
            cam.nearClipPlane = target.nearClipPlane;
            cam.farClipPlane = target.farClipPlane;
            cam.rect = target.rect;
        }
    }
}
