using System.Collections.Generic;
using UnityEngine;
#if UNITY_PIPELINE_URP
using System;
using UnityEngine.Rendering;
#endif

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class UPixelatorRenderHandler : MonoBehaviour
    {
        [HideInInspector] public UPixelator uPixelator;

        public UPixelatorCameraInfo camInfo;
        UPixelatorSnappable camSnappable;

        [Tooltip("UPixelatorSnappables that are visible to this camera")]
        public List<UPixelatorSnappable> snappables = new List<UPixelatorSnappable>();

        [HideInInspector] public Quaternion storedCamRotation;
        [HideInInspector] public int storedPixelMultiplier;
        [HideInInspector] public float origOrthoSize;

        internal void DoLateUpdate()
        {
            // TODO: move to upixelator dirty logic
            EnsureCamSnappable();

            // NOTE: this ensures OnEndCameraRendering is called after other callbacks
            // otherwise the camera snapped position might be reset back too early

#if UNITY_PIPELINE_URP
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
#else
            Camera.onPostRender -= PostRender;
            Camera.onPostRender += PostRender;
#endif
            // NOTE: setting targetTexture must be done here instead of the render callbacks
            //  otherwise it will not be set the same frame
            HandleTargetTexture();
        }

        void HandleTargetTexture()
        {
            if (camInfo.cam.targetTexture != camInfo.renderTexture)
                camInfo.cam.targetTexture = camInfo.renderTexture;

            // WaitForEndOfFrame is unreliable but for targetTexture seems to not be a problem to call
            Utils.RunAtEndOfFrameOrdered(() => {
                // NOTE: this is needed or else the Screen size is set to renderTexture size
                //  and events like Input.mousePosition that return pixel values are wrong
                // NOTE: this also prevents other cameras to affect the texture
                camInfo.cam.targetTexture = null;
            }, 0, this);
        }

        float GetSnapSize()
        {
            var mult = (camInfo.cam == uPixelator.uPixelatorCam) ? 1 : uPixelator.pixelMultiplier;
            return mult * (2 * camInfo.cam.orthographicSize) / uPixelator.screenSize.y;
        }

        void EnsureCamSnappable()
        {
            if (camSnappable) return;

            camSnappable = camInfo.cam.GetComponent<UPixelatorSnappable>();
            if (!camSnappable) camSnappable = camInfo.cam.gameObject.AddComponent<UPixelatorSnappable>();

            camSnappable.isCamera = true;
            // camSnappable.snapRotation = false;
            // camSnappable.snapLocalScale = false;
            // camSnappable.stabilizeDiagonal = false;

            // #if UNITY_PIPELINE_URP
            //     var camData = camInfo.cam?.GetComponent<UniversalAdditionalCameraData>();
            //     if (camData?.renderType == CameraRenderType.Overlay)
            //         camSnappable.isOverlayCamera = true;
            // #endif
        }

        internal void HandleSnap()
        {
            if (!ShouldSnap()) return;

            var isInitialPositionDirty = false;
            if (camSnappable != null)
            {
                if (storedPixelMultiplier != uPixelator.pixelMultiplier)
                {
                    isInitialPositionDirty = true;
                    storedPixelMultiplier = uPixelator.pixelMultiplier;
                }

                if (camInfo.cam.transform.rotation != storedCamRotation)
                {
                    isInitialPositionDirty = true;
                    storedCamRotation = camInfo.cam.transform.rotation;
                }
            }

            foreach (UPixelatorSnappable snappable in snappables)
            {
                if (snappable == null) continue;
                if (!snappable.isActiveAndEnabled) continue;

                if (snappable == camSnappable)
                {
                    if (snappable.snapPosition)
                    {
                        var camDiff = HandleCamSnap();
                        if (camInfo.cam == uPixelator.uPixelatorCam) continue;
                        UpdateRenderQuadPosition(camDiff);
                    }
                    continue;
                }

                if (isInitialPositionDirty)
                    snappable.initialPosition = snappable.transform.position;

                Vector3 initialPos = default;
                if (camInfo.cam != uPixelator.uPixelatorCam) initialPos = snappable.initialPosition;

                // NOTE: snap rotation and scale first, then position
                if (snappable.snapRotation)
                {
                    if (snappable.isLocalRotation) snappable.SnapLocalRotation();
                    else snappable.SnapRotation(camInfo.cam.transform.rotation);
                }
                if (snappable.snapLocalScale) snappable.SnapLocalScale(snappable.snapScaleValue);

                if (snappable.snapPosition)
                {
                    snappable.SnapPosition(camInfo.cam.transform.rotation, GetSnapSize(), initialPos);
                }
            }
        }

        public bool ShouldSnap()
        {
            return camInfo.snap && camInfo.cam && camInfo.cam.orthographic;
        }

        // public void UpdateRenderQuadPosition()
        // {
        //     if (!ShouldSnap()) return;
        //     if (camSnappable == null) return;
        //     if (!camSnappable.isActiveAndEnabled) return;
        //     if (!camSnappable.snapPosition) return;

        //     camSnappable.StorePosition();
        //     UpdateRenderQuadPosition(HandleCamSnap());
        //     camSnappable.RestorePosition();
        // }

        Vector3 HandleCamSnap()
        {
            var repeatSize = camInfo.stabilize ? uPixelator.ditherRepeatSize : 1;
            float camSnapSize = repeatSize * GetSnapSize();
            return camSnappable.SnapPosition(camInfo.cam.transform.rotation, camSnapSize, default);
        }

        void UpdateRenderQuadPosition(Vector3 camSnapDiff)
        {
            if (!camInfo.stabilize) return;

            // if (camSnapDiff == default) return;
            Vector3 localPosition = -camSnapDiff / camInfo.cam.orthographicSize;
            // NOTE: keep z, it is handled by the UPixelator based on depth
            localPosition.z = camInfo.renderQuad.localPosition.z;
            camInfo.renderQuad.localPosition = localPosition;

            // if (camInfo.stabilize && camInfo.cam.orthographic) {}
            // else renderQuad.localPosition = Vector3.zero;
        }

        internal void HandleUnsnap()
        {
            if (!ShouldSnap()) return;

            foreach (UPixelatorSnappable snappable in snappables)
            {
                if (snappable == null) continue;
                if (!snappable.isActiveAndEnabled) continue;

                // NOTE: restore in reverse order from snapping
                if (snappable.snapPosition) snappable.RestorePosition();
                if (snappable.snapLocalScale) snappable.RestoreLocalScale();
                if (snappable.snapRotation) snappable.RestoreRotation();
            }
        }

        void OnEnable()
        {
            // NOTE: needed here to immediately resume rendering to texture instead of waiting next LateUpdate
            // TODO: test is this is still needed, like a frame missing one one render making rendertexture blank
            // if (camInfo?.cam != null) camInfo.cam.targetTexture = camInfo.renderTexture;

#if UNITY_PIPELINE_URP
            Utils.AddCallbackToStart<Action<ScriptableRenderContext, Camera>>(typeof(RenderPipelineManager), "beginCameraRendering", new Action<ScriptableRenderContext, Camera>(OnBeginCameraRendering));
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
#else
            Utils.AddCallbackToStart<Camera.CameraCallback>(typeof(Camera), "onPreRender", new Camera.CameraCallback(PreRender));
            Camera.onPostRender -= PostRender;
            Camera.onPostRender += PostRender;
#endif
        }

        void OnDisable()
        {
            if (camInfo != null && camInfo.cam != null)
            {
                camInfo.cam.rect = new Rect(0, 0, 1, 1);

                if (camInfo.cam.targetTexture != null)
                {
                    camInfo.cam.targetTexture.Release();
                    camInfo.cam.targetTexture = null;
                }
            }

            camSnappable = null;

#if UNITY_PIPELINE_URP
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
#else
            Camera.onPreRender -= PreRender;
            Camera.onPostRender -= PostRender;
#endif
        }

#if UNITY_PIPELINE_URP
        void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            PreRender(camera);
        }

        void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            PostRender(camera);
        }
#endif

        // void OnGUI()
        // {
        //     if (camInfo.cam == uPixelator.uPixelatorCam) return;
        //     GUI.Label(new Rect(Screen.width/2 - 50, Screen.height/2 +50, 100, 20),"" + a);
        // }

        public UPixelatorSnappable GetCamSnappable()
        {
            if (!camSnappable) EnsureCamSnappable();
            return camSnappable;
        }

        internal void HandleUpscaling()
        {
            camInfo.cam.orthographicSize *= (float)camInfo.renderTexture.height * uPixelator.pixelMultiplier / uPixelator.screenSize.y;
        }

        internal void HandleFreeAspect()
        {
            float rectX = 0;
            float rectY = 0;

            if (uPixelator.renderTextureSize.x % 2 == 1)
                rectX = -1f / uPixelator.renderTextureSize.x;

            if (uPixelator.renderTextureSize.y % 2 == 1)
            {
                rectY = -1f / uPixelator.renderTextureSize.y;
                // zoom out by half pixel size
                camInfo.cam.orthographicSize += camInfo.cam.orthographicSize * rectY;
            }
            camInfo.cam.rect = new Rect(rectX, rectY, 1, 1);
        }

        // NOTE: store the Screen size values in LateUpdate like uPixelator.screenSize, otherwise
        // it will differ in PreRender when resizing the screen in the editor
        void PreRender(Camera camera)
        {
            if (camera != camInfo.cam) return;

            // NOTE: rectTransform snapping stops working when called every PreRender
            //  and rectTransform jitter happens when not called every PreRender
            if (!Application.isPlaying) uPixelator.HandleSnapping();
        }

        void PostRender(Camera camera)
        {
            if (camera != camInfo.cam) return;

            if (camera == uPixelator.uPixelatorCam)
            {
                // NOTE: unsnapping here will make urp overlay cameras have wrong base cam position however
                // overlay cameras will be pixelated as well but ui needs to be non pixelated so doesn't matter
                uPixelator.HandleUnsnapping();
                return;
            }

            // see comment in PreRender
            if (!Application.isPlaying) uPixelator.HandleUnsnapping();
        }
    }
}
