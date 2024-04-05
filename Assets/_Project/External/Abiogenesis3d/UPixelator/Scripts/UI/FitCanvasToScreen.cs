using UnityEngine;
using UnityEngine.UI;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasScaler))]
    public class FitCanvasToScreen : MonoBehaviour
    {
        public Camera cam;
        Canvas canvas;
        RectTransform canvasRect;
        CanvasScaler canvasScaler;

        public void Init()
        {
            canvas = GetComponent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();
            canvasScaler = canvas.GetComponent<CanvasScaler>();
        }

        void OnEnable()
        {
            Init();
        }

        public void DoUpdate()
        {
            // TODO: in free aspect when screen changes, this should not return
            if (!cam) return;

            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                canvasScaler.dynamicPixelsPerUnit = 1; // match world space
                canvasRect.sizeDelta = new Vector2(Screen.width, Screen.height);

                float camHeightRatio = Utils.GetCamHeightRatio(cam, canvas.transform);
                canvas.transform.localScale = Vector3.one * camHeightRatio * canvas.scaleFactor;
            }
        }
    }
}
