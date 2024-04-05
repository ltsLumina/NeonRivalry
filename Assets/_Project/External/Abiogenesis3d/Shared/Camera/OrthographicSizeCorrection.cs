using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    // NOTE: needs to run after your scripts and before UPixelator
    [DefaultExecutionOrder(int.MaxValue -101)]
    public class OrthographicSizeCorrection : MonoBehaviour
    {
        public Camera cam;

        [Range(420, 1440)]
        public float screenHeight = 1080;

        [Header("Enable this only if your custom script is setting orthographicSize every frame.")]
        [Header("If false, standardOrthoSize will be used.")]
        public bool myScriptSetsOrthoSize;
        [Header("Enable this only if your custom script is has the attribute [ExecuteInEditMode].")]
        public bool myScriptSetsOrthoSizeInEditMode;
        [Header("This will be used if myScriptSetsOrthoSize is disabled.")]
        public float standardOrthoSize = 1;

        void EnsureCam()
        {
            if (!cam)
            {
                cam = GetComponent<Camera>();
                standardOrthoSize = cam.orthographicSize;
            }
        }

        void OnDisable()
        {
            if (!cam) return;
            cam.orthographicSize = standardOrthoSize;
        }

        void LateUpdate()
        {
            EnsureCam();
            if (!cam) return;

           // NOTE: either your custom script sets it or this script sets it, otherwise the multiplication will keep stacking
            if (!myScriptSetsOrthoSize) cam.orthographicSize = standardOrthoSize;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (myScriptSetsOrthoSize && !myScriptSetsOrthoSizeInEditMode) return;
            }
#endif
            // this is what makes the scene equal size regardless of screen resolution
            cam.orthographicSize *= Screen.height / screenHeight;
        }
    }
}
