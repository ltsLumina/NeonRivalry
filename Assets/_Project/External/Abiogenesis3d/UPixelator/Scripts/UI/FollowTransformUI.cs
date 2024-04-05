using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    // NOTE: needs to be called after UPixelator for synced camera position
    [DefaultExecutionOrder(int.MaxValue - 99)]
    public class FollowTransformUI : MonoBehaviour
    {
        // NOTE: hidden until fixed
        [HideInInspector]
        public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;

        UPixelator uPixelator;

        public Camera ownerCam;

        FitCanvasToScreen fitCanvasToScreen;

        public Canvas parentCanvas;
        public Transform parentTransform;
        public RectTransform rectTransform;

        void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void HandleOwnerCam()
        {
            if (uPixelator)
            {
                ownerCam = uPixelator.isActiveAndEnabled ? uPixelator.uPixelatorCam : uPixelator.mirroredCamera;
            }

            if (ownerCam == null || !ownerCam.isActiveAndEnabled)
            {
                ownerCam = Camera.main;
            }
        }

        void HandleParent()
        {
            // NOTE: do this every time because parent can change
            parentCanvas = transform.parent.GetComponent<Canvas>();
            if (!parentCanvas)
            {
                GameObject parentCanvasGO = new GameObject("ParentCanvas");
                parentCanvasGO.transform.SetParent(transform.parent);
                parentCanvas = parentCanvasGO.AddComponent<Canvas>();
                transform.SetParent(parentCanvasGO.transform);
            }
            // NOTE: do this every time because parent can change
            // TODO: refactor this to track the parent and clean up previous parent components
            fitCanvasToScreen = parentCanvas.GetComponent<FitCanvasToScreen>();
            if (!fitCanvasToScreen)
                fitCanvasToScreen = parentCanvas.gameObject.AddComponent<FitCanvasToScreen>();

            if (renderMode == RenderMode.WorldSpace)
            {
                Debug.LogWarning("WorldSpace works without this script, remove it first. Switching to Overlay.");
                renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else if (renderMode == RenderMode.ScreenSpaceCamera)
            {
                Debug.LogWarning("ScreenSpaceCamera is currently not supported. Switching to Overlay.");
                renderMode = RenderMode.ScreenSpaceOverlay;
            }

            parentCanvas.renderMode = renderMode;
            // parentCanvas.worldCamera = ownerCam;
            parentCanvas.planeDistance = -0.1f;
            parentCanvas.pixelPerfect = true;

            string renderModeStr = "";
            if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) renderModeStr = "Overlay";
            else if (parentCanvas.renderMode == RenderMode.WorldSpace) renderModeStr = "World";
            else if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera) renderModeStr = "Camera";

            fitCanvasToScreen.gameObject.layer = transform.gameObject.layer;
            fitCanvasToScreen.name = name + " - Parent: " + renderModeStr;
            fitCanvasToScreen.cam = ownerCam;

            fitCanvasToScreen.Init();

            // get first non canvas parent to follow
            parentTransform = parentCanvas.transform.parent;
            while (parentTransform && parentTransform.GetComponent<Canvas>() != null)
                parentTransform = parentTransform.parent;
        }

        void LateUpdate()
        {
            if (!uPixelator) uPixelator = GameObject.FindObjectOfType<UPixelator>();
            HandleOwnerCam();
            if (!uPixelator) return;

            HandleParent();

            if (ownerCam == null) return;

            fitCanvasToScreen.DoUpdate();

            Vector3 viewportPoint = ownerCam.WorldToViewportPoint(parentTransform.position);

            if (float.IsNaN(viewportPoint.x) ||
                float.IsNaN(viewportPoint.y) ||
                float.IsNaN(viewportPoint.z))
            {
                // TODO: this happens when scripts are recompiled or undo is pressed..
                // TODO: why, probably camera is destroyed?
                // Debug.Log("viewportPoint: " + viewportPoint + ", parentTransform.position: " + parentTransform.position);
            }
            else {
                rectTransform.anchoredPosition = default;
                // rectTransform.SetPositionAndRotation(viewportPoint, Quaternion.Euler( 0, 0, 0));
                rectTransform.anchorMin = viewportPoint;
                rectTransform.anchorMax = viewportPoint;
            }
        }
    }
}
