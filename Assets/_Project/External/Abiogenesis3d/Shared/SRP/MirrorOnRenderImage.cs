using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class MirrorOnRenderImage : MonoBehaviour
    {
        public delegate void RenderImageCallback(RenderTexture src, RenderTexture dest);
        public RenderImageCallback renderImageCallback;

        public string target;
        public GameObject targetGO;

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (renderImageCallback?.GetInvocationList().Length > 0)
                renderImageCallback.Invoke(src, dest);
            else
            {
                Graphics.SetRenderTarget(dest);
                Graphics.Blit(src, dest);
            }
        }
    }
}
