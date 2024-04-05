using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Abiogenesis3d
{
    public enum GraphicsFormatSubset
    {
        Automatic_Experimental = GraphicsFormat.None,
        RGBA8_UNorm = GraphicsFormat.R8G8B8A8_UNorm,
        RGBA8_SRGB = GraphicsFormat.R8G8B8A8_SRGB,
        RGBA8_SNorm = GraphicsFormat.R8G8B8A8_SNorm,
        RGBA16_SFloat = GraphicsFormat.R16G16B16A16_SFloat,
    }

    [Serializable]
    public class UPixelatorCameraInfo
    {
        public Camera cam;
        [Header("Subpixel")]
        [Tooltip("This will remove pixel creep on camera movement and only works for orthographic cameras.")]
        public bool snap = true;
        [Tooltip("Disable this for stationary cameras, it can lead to incorrect snapping on pixelMultiplier change.")]
        public bool stabilize = true;

        // [Header("Parallax")]
        // public float positionSpeed;
        // public float rotationSpeed;

        [HideInInspector] public Transform parent;
        [HideInInspector] public Renderer renderQuadRenderer;
        [HideInInspector] public RenderTexture renderTexture;
        [HideInInspector] public Transform renderQuad;
        [HideInInspector] public UPixelatorRenderHandler renderHandler;

        [Header("Orthographic Size Correction")]
        [Tooltip(@"
            Since orthographicSize is proportional to Screen.height when set in Free Aspect the game will appear to zoom in/out.
            Enable this to correct the orthographicSize so that the scene has the same size on screen regardless of the resolution.
        ")]
        public bool orthographicSizeCorrectionEnabled = true;
        [Tooltip(@"
            Enable this if you have a custom script for camera zoom by changing cam.orthographicSize.
            Your script must set cam.orthographicSize every frame otherwise this will not work.
        ")]
        public bool myScriptSetsOrthoSize;
        [Tooltip("Enable this if your script that sets cam.orthographicSize every frame is also running in edit mode.")]
        public bool myScriptSetsOrthoSizeInEditMode;

        [HideInInspector] public OrthographicSizeCorrection orthographicSizeCorrection;
    }

    [RequireComponent(typeof(MultiCameraEvents))]
    [RequireComponent(typeof(MultiCameraEventsIgnore))]
    [RequireComponent(typeof(PixelArtEdgeHighlightsIgnore))]
    [ExecuteInEditMode]
    // NOTE: execute last, after all other game logic is done with the transforms
    //  leave some room for other scripts like FollowTransformUI that needs to be
    //  called after UPixelator syncs camera positions
    [DefaultExecutionOrder(int.MaxValue - 100)]
    public class UPixelator : MonoBehaviour
    {
        [Tooltip("Camera that UPixelator camera will mirror (transform, clip planes, ortho size). Disable with `dontMirrorCamera`.")]
        public Camera mirroredCamera;
        [Tooltip("Layer mask for World Space UI, you can add multiple layer and they will be removed from the mirroredCamera.")]
        public LayerMask layerMaskUI = 1 << 5; // UI

        [Tooltip("Chose a supported RenderTexture type for your target platform. (NOTE: Automatic currently only works for URP")]
        public GraphicsFormatSubset graphicsFormat = GraphicsFormatSubset.RGBA8_UNorm;

        [Tooltip("How many times should each pixel be repeated on each axis.")]
        [Range(2, 5)] public int pixelMultiplier = 3;
        [Tooltip("Checkerboard screenspace repeat size, after how many pixels the pattern overlaps itself.")]
        [Range(2, 8)] public int ditherRepeatSize = 8;

        [HideInInspector] public Vector2Int lastScreenSize;
        [HideInInspector] public Vector2Int screenSize;
        [HideInInspector] public Vector2Int renderTextureSize;
        // public Vector2Int extraPadding;

        [Header("Orthographic Size Correction")]
        [Range(420, 1440)] public float screenHeight = 1080f;

        // TODO: if layer not named add a name for it through script?
        // maybe use this for excluding from post process on upixelator cam with urp render objects?
        // public LayerMask layerMaskQuads = 1 << 30;

        const float camSliceGap = 0.05f;

        // NOTE: this is wip, need to multiply ui positions with this
        // [Range(0.1f, 1)] public float uPixelatorZoom = 1;
        // TODO: UPixelatorZoomOffset to zoom at mouse position

        [Tooltip("Camera that renders all UPixelator textures and world space UI on `layerMaskUI` if `mirroredCamera` is set.")]
        [HideInInspector] public Camera uPixelatorCam;

        [Header("To ignore a camera add UPixelatorCameraIgnore component to it.")]
        public bool autoDetectCameras = false;
        public List<UPixelatorCameraInfo> cameraInfos = new List<UPixelatorCameraInfo>();

        public Material renderQuadTransparentMat;
        public Material renderQuadOpaqueMat;

        float lastHandleInits;
        float handleInitsEvery = 1;

        [NonSerialized] public bool isSnappablesDirty = true;
        [NonSerialized] public bool isCamerasDirty = true;
        [NonSerialized] public bool isScreenSizeDirty = true;

        [Tooltip("For some reason when entering play mode LateUpdate is called twice so skip first snap and let it be called from OnPostRender.")]
        bool skippedFirstSnapInLateUpdate;

        enum SnapState {Unsnapped, Snapped};
        SnapState snapState = SnapState.Unsnapped;

        [Header("Experimental")]
        [Tooltip("When set does not render UI and is kept far above the scene.")]
        public bool dontMirrorCamera;

        void OnValidate()
        {
            if (ditherRepeatSize % 2 != 0) ditherRepeatSize += 1;

            Refresh();
        }

        void EnforceUniqueInstance()
        {
            var existingInstances = FindObjectsOfType<UPixelator>();
            if (existingInstances.Length > 1)
            {
                Debug.Log($"UPixelator: There should only be one active instance in the scene. Deactivating: {name}");
                enabled = false;
                return;
            }
        }

        public void Refresh()
        {
            lastHandleInits = 0;
            HandleInits();
        }

        void OnEnable()
        {
            Refresh();

            EnforceUniqueInstance();

            if (uPixelatorCam) uPixelatorCam.enabled = true;
        }

        void OnDisable()
        {
            if (uPixelatorCam) uPixelatorCam.enabled = false;
            foreach (var camInfo in cameraInfos)
                if (camInfo.renderHandler) camInfo.renderHandler.enabled = false;
        }

        void HandleScreenResize()
        {
            // NOTE: never use Screen class directly, use stored screenSize as it can change between PreRender/PostRender calls
            screenSize = new Vector2Int(Screen.width, Screen.height);
            renderTextureSize = GetRenderTextureSize();
        }

        // void OnGUI()
        // {
        //     GUI.Label(new Rect(Screen.width/2 - 50, Screen.height/2 -50, 100, 20), Screen.width + "x" + Screen.height);
        // }

        // System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();


        void Update()
        {
            foreach (var camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;
                if (camInfo.cam == uPixelatorCam) continue;

                if (!camInfo.orthographicSizeCorrection)
                {
                    camInfo.orthographicSizeCorrection = camInfo.cam.GetComponent<OrthographicSizeCorrection>();
                    if (camInfo.orthographicSizeCorrection)
                    {
                        // init values to ones from the found script
                        camInfo.myScriptSetsOrthoSize = camInfo.orthographicSizeCorrection.myScriptSetsOrthoSize;
                        camInfo.myScriptSetsOrthoSizeInEditMode = camInfo.orthographicSizeCorrection.myScriptSetsOrthoSizeInEditMode;
                    }
                    else
                    {
                        camInfo.orthographicSizeCorrection = camInfo.cam.gameObject.AddComponent<OrthographicSizeCorrection>();
                    }
                }
                // TODO: improve by using a config file that is referenced from both places
                camInfo.orthographicSizeCorrection.enabled = camInfo.orthographicSizeCorrectionEnabled;
                camInfo.orthographicSizeCorrection.myScriptSetsOrthoSize = camInfo.myScriptSetsOrthoSize;
                camInfo.orthographicSizeCorrection.myScriptSetsOrthoSizeInEditMode = camInfo.myScriptSetsOrthoSizeInEditMode;
                camInfo.orthographicSizeCorrection.screenHeight = screenHeight;
            }
        }

        void LateUpdate()
        {
            // stopwatch.Start();
#if UNITY_EDITOR
            // don't run instances that are part of the current opened prefab
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage)
            {
                var openedPrefabRoot = prefabStage.prefabContentsRoot;
                var uPixelators = openedPrefabRoot.GetComponentsInChildren<UPixelator>();
                if (uPixelators.FirstOrDefault(u => u == this)) return;
            }
#endif
            HandleInits();

            // these should run every frame
            if (isScreenSizeDirty)
            {
                isScreenSizeDirty = false;
                HandleScreenResize();
            }

            if (isCamerasDirty)
            {
                isCamerasDirty = false;

                EnsureUPixelatorCam();
                HandleGetCameras();
                HandleUPixelatorCam();
                HandleCamInfos();
            }

            if (!dontMirrorCamera && mirroredCamera)
                uPixelatorCam.orthographicSize = mirroredCamera.orthographicSize;

            HandleCamInfosSync();
            HandleMirrorCameraSync();

            if (isSnappablesDirty)
            {
                isSnappablesDirty = false;
                HandleSnappables();
            }

            foreach (var camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;
                if (!camInfo.renderHandler) continue;
                if (!camInfo.renderHandler.isActiveAndEnabled) continue;

                // NOTE: call LateUpdate from here to enforce a consistent order
                camInfo.renderHandler.DoLateUpdate();
            }

            // TODO: for some reason late update is called twice when entering the playmode
            if (skippedFirstSnapInLateUpdate) HandleSnapping();
            else skippedFirstSnapInLateUpdate = true;

            // stopwatch.Stop();
            // var duration = stopwatch.Elapsed.TotalMilliseconds;
            // if (duration> 0.2d) Debug.Log($"Duration: {stopwatch.Elapsed.TotalMilliseconds} ms");
            // stopwatch.Reset();
        }

        void HandleMirrorCameraSync()
        {
            if (dontMirrorCamera || !mirroredCamera) return;

            uPixelatorCam.transform.SetPositionAndRotation(
                mirroredCamera.transform.position, mirroredCamera.transform.rotation);
        }

        void HandleCamInfosSync()
        {
            foreach (var camInfo in cameraInfos)
            {
                if (!camInfo.renderHandler) continue;
                // TODO: this can be used to conditionally render
                if (!camInfo.cam)
                {
                    camInfo.renderHandler.gameObject.SetActive(false);
                    continue;
                }
                if (camInfo.cam == uPixelatorCam) continue;

                camInfo.renderHandler.gameObject.SetActive(camInfo.cam.gameObject.activeInHierarchy);

                HandleResizeTexture(camInfo);

                camInfo.renderQuad.localScale = new Vector2(renderTextureSize.x, renderTextureSize.y) / screenSize.y * 2 * pixelMultiplier;

                if (camInfo.renderHandler)
                {
                    // sort parent by depth
                    var handlerLocalPos = camInfo.renderHandler.transform.localPosition;

                    const float zOffset = -1; // push it into visible farClipPlane
                    var farClipPlane = mirroredCamera ? mirroredCamera.farClipPlane + zOffset : 0;
                    handlerLocalPos.z = farClipPlane - cameraInfos.Where(x => x.cam).ToList().IndexOf(camInfo) * camSliceGap;
                    camInfo.renderHandler.transform.localPosition = handlerLocalPos;

                    camInfo.renderHandler.transform.localScale = Vector3.one * uPixelatorCam.orthographicSize;
                }
            }
        }

        void HandleCamInfos()
        {
            foreach (var camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;

#if UNITY_EDITOR
                // NOTE: if orthographicSize is 0 some divisions will break
                if (camInfo.cam.orthographicSize == 0)
                {
                    Debug.LogWarning("Cameras should not have orthographicSize 0, setting to 1.", camInfo.cam.gameObject);
                    camInfo.cam.orthographicSize = 1;
                }
#endif
                if (camInfo.cam == uPixelatorCam)
                    camInfo.stabilize = false;

                // TODO: why having this true fixes camera perspective projection geometry intersection artifacts when they happen??
                // TODO: why having this true fixes tearing on _CameraDepthNormalsTexture in postprocess??
                //   cannot reproduce this as a fix... builtin has tearing regardless, has to do with setting cam.rect
                // camInfo.cam.allowHDR = true;

                camInfo.cam.allowMSAA = false;

                UpdateCamInfo(camInfo);

#if UNITY_EDITOR
                // cleanup any handlers
                var handlers = gameObject.GetComponentsInChildren<UPixelatorRenderHandler>(true);
                foreach (var handler in handlers)
                {
                    // need to remove handler gameobjects that are not in cameraInfos
                    if (cameraInfos.All(c => c.renderHandler != handler))
                        DestroyImmediate(handler.gameObject);
            }

                for (int i = 0; i < cameraInfos.Count; i++)
                {
                    var renderHandler = cameraInfos[i].renderHandler;
                    if (renderHandler) renderHandler.transform.SetSiblingIndex(i);
                }
#endif
            }
        }

        public void HandleSnapping()
        {
            // NOTE: animator `Culling Mode: Cull Update Transforms` mutates positions on start
            // if (Time.realtimeSinceStartup < 7) return;

            if (snapState == SnapState.Snapped) return;
            snapState = SnapState.Snapped;

            // Debug.Log("<color=orange>Store positions</color>");
            // NOTE: first store all snappables on all cameras, othwerise the positions will be mutated
            foreach (var camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;
                if (!camInfo.snap) continue;

                // NOTE: first store all or transform is changed by afterwards snapped parent
                foreach (UPixelatorSnappable snappable in camInfo.renderHandler.snappables)
                {
                    if (!snappable) continue;
                    if (!snappable.isActiveAndEnabled) continue;

                    if (snappable.snapPosition) snappable.StorePosition();
                    if (snappable.snapRotation) snappable.StoreRotation();
                    if (snappable.snapLocalScale) snappable.StoreLocalScale();
                }
            }

            foreach (var camInfo in cameraInfos)
            {
                camInfo.renderHandler.HandleSnap();
            }

            // TODO: separation of concern
            // NOTE: this has to be done from LateUpdate since by the time PreRender is called objects are already culled
            foreach (UPixelatorCameraInfo camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;
                if (camInfo.cam == uPixelatorCam) continue;

                camInfo.renderHandler.origOrthoSize = camInfo.cam.orthographicSize;

                camInfo.renderHandler.HandleUpscaling();
                camInfo.renderHandler.HandleFreeAspect();
            }
        }

        // NOTE: called from handlers PostRender
        internal void HandleUnsnapping()
        {
            if (snapState == SnapState.Unsnapped) return;
            snapState = SnapState.Unsnapped;

            foreach (var camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;
                if (!camInfo.snap) continue;

                // Debug.Log("<color=orange>Validate positions</color> " + camInfo.cam.name);
                foreach (UPixelatorSnappable snappable in camInfo.renderHandler.snappables)
                {
                    if (!snappable) continue;
                    if (!snappable.isActiveAndEnabled) continue;

                    if (snappable.snapPosition) snappable.ValidatePositionBeforeRestore();
                }
            }

            foreach (var camInfo in cameraInfos)
            {
                camInfo.renderHandler.HandleUnsnap();
            }

            // TODO: separation of concern
            foreach (UPixelatorCameraInfo camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;
                if (camInfo.cam == uPixelatorCam) continue;

                camInfo.cam.orthographicSize = camInfo.renderHandler.origOrthoSize;
            }
        }

        public Camera GetFirstCamera()
        {
            return cameraInfos
                .Where(c => c.cam)
                .Where(c => c.cam != uPixelatorCam)
                .Where(c => c.cam.gameObject.activeInHierarchy)
                .FirstOrDefault()?.cam;
        }

        Type GetIgnoredType()
        {
            return typeof(UPixelatorCameraIgnore);
        }

        void HandleGetCameras()
        {
            List<Camera> cameras;
            if (autoDetectCameras) cameras = FindObjectsOfType<Camera>().ToList();
            else
            {
                cameras = new List<Camera> {};
                if (mirroredCamera) cameras.Add(mirroredCamera);
                cameras.Add(uPixelatorCam);
                cameras.AddRange(cameraInfos.Select(c => c.cam));
            }

            var camerasToRemove = cameras.Where(c => c && c.GetComponent(GetIgnoredType())).ToArray();
            foreach(var cam in camerasToRemove)
            {
                var match = cameraInfos.FirstOrDefault(c => c.cam == cam);
                if (match != default) cameraInfos.Remove(match);
            }
            cameras.RemoveAll(c => camerasToRemove.Contains(c));

            foreach (var camInfo in cameraInfos)
            {
                if (!cameras.Contains(camInfo.cam))
                {
                    if (camInfo.renderHandler)
                        camInfo.renderHandler.gameObject?.SetActive(false);
                }
            }

            foreach (var cam in cameras)
            {
                var camInfo = cameraInfos.FirstOrDefault(c => c.cam == cam);
                if (camInfo != null)
                {
                    if (camInfo.renderHandler)
                        camInfo.renderHandler.gameObject.SetActive(true);
                    continue;
                }
                else
                {
                    camInfo = new UPixelatorCameraInfo {cam = cam};
                    cameraInfos.Add(camInfo);
                }
            }

            cameraInfos = cameraInfos.OrderBy(c => GetCamDepthOr0(c.cam)).ToList();

            var depthsNotUnique = cameraInfos
                .Where(c => c.cam)
                .Where(c => c.cam != uPixelatorCam)
                .Where(c => c.cam.gameObject.activeInHierarchy)
                .GroupBy(c => GetCamDepthOr0(c.cam))
                .Where(g => g.Count() > 1)
                .Select(g => g.Key).ToArray();

            if (depthsNotUnique.Length > 0)
            {
                Debug.LogWarning("UPixelator: Please make cameras have unique depth.");
                return;
            }
        }

        Vector2Int GetRenderTextureSize()
        {
            // TODO: move to handler and allow each camera to have a separate texture size, rect, pixelMultiplier etc.
            return screenSize / pixelMultiplier + GetRenderTexturePadding();

// #if UNITY_EDITOR
//             // NOTE: prevent Unity from irreversibly breaking the camera component...
//             size.x = Mathf.Clamp(size.x, 1, screenSize.x + ditherRepeatSize);
//             size.y = Mathf.Clamp(size.y, 1, screenSize.y + ditherRepeatSize);
// #endif
            // return size;
        }

        // NOTE: must always have even numbers, which dither is setup to have
        // NOTE: lowest padding should always be 2 to hide subpixel offset correction on screen edges
        public Vector2Int GetRenderTexturePadding()
        {
            // NOTE: ditherRepeatSize is always even, being set in OnValidate
            return Vector2Int.one * ditherRepeatSize * 2; //  + extraPadding * 2;
        }

        // TODO: cleanup unreferenced quads
        void EnsureRenderQuad(UPixelatorCameraInfo camInfo)
        {
            if (camInfo.renderQuad) return;
            camInfo.renderQuad = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            DestroyImmediate(camInfo.renderQuad.GetComponent<MeshCollider>());
            camInfo.renderQuad.SetParent(camInfo.parent, false);
        }

        void EnsureRenderTexture(UPixelatorCameraInfo camInfo)
        {
            if (camInfo.renderTexture)
            {
                camInfo.renderTexture.name = "RenderTexture - " + camInfo.cam.name;
                return;
            }

            // NOTE: this is initial resolution, do changes from HandleResizeTexture
            camInfo.renderTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 8, GetGraphicsFormat(camInfo.cam), 0);
            camInfo.renderTexture.filterMode = FilterMode.Point;

            camInfo.renderTexture.Create();
        }

        void EnsureRenderQuadRenderer(UPixelatorCameraInfo camInfo)
        {
            if (camInfo.renderQuadRenderer) return;
            camInfo.renderQuadRenderer = camInfo.renderQuad.GetComponent<Renderer>();
        }

        void HandleRenderQuadMaterial(UPixelatorCameraInfo camInfo)
        {
            EnsureRenderQuadRenderer(camInfo);

            if (!renderQuadTransparentMat)
            {
                Debug.LogWarning("renderQuadTransparentMat needs to be assigned");
                return;
            }
            if (!renderQuadOpaqueMat)
            {
                Debug.LogWarning("renderQuadOpaqueMat needs to be assigned");
                return;
            }

            var index = 0;
            foreach (UPixelatorCameraInfo c in cameraInfos)
            {
                if (!c.cam) continue;
                if (!c.cam.gameObject.activeInHierarchy) continue;
                if (c == camInfo) break;
                index++;
            }
            var mat = index == 0 ? renderQuadOpaqueMat : renderQuadTransparentMat;

            // TODO: write a custom urp converted since Unity team doesn't want to fix this bug in their converter
#if UNITY_EDITOR && UNITY_2021_1_OR_NEWER && UNITY_PIPELINE_URP
            if (mat.shader.name == "Unlit/Texture")
            {
                Debug.Log("Converting to URP shader");
                mat.shader = Shader.Find("Universal Render Pipeline/Unlit");
                // r.sharedMaterial.renderQueue = 3000;
            }
#endif
            var r = camInfo.renderQuadRenderer;
            if (r.sharedMaterial.shader == mat.shader) return;

            r.sharedMaterial = Instantiate(mat);
            r.sharedMaterial.mainTexture = camInfo.renderTexture;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
            r.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            r.allowOcclusionWhenDynamic = false;
        }

        GraphicsFormat GetGraphicsFormat(Camera cam)
        {
#if UNITY_PIPELINE_URP
            // auto detect
            if (graphicsFormat == GraphicsFormatSubset.Automatic_Experimental)
                return SystemInfo.GetGraphicsFormat(IsHDR(cam) ? DefaultFormat.HDR : DefaultFormat.LDR);
#else
            // builtin fallback
            if (graphicsFormat == GraphicsFormatSubset.Automatic_Experimental)
            {
                Debug.Log("Automatic_Experimental is not supported in Builtin, using RGBA8_UNorm.");
                graphicsFormat = GraphicsFormatSubset.RGBA8_UNorm;
            }
#endif
            return (GraphicsFormat) graphicsFormat;
        }

        bool IsHDR(Camera cam)
        {
            var isHDR = cam.allowHDR;
#if UNITY_PIPELINE_URP
            isHDR = isHDR && PipelineAssets.ShouldUseHDR();
#endif
            return isHDR;
        }

        void HandleResizeTexture(UPixelatorCameraInfo camInfo)
        {
            EnsureRenderTexture(camInfo);

            var graphicsFormat = GetGraphicsFormat(camInfo.cam);

            if (renderTextureSize.x != camInfo.renderTexture.width ||
                renderTextureSize.y != camInfo.renderTexture.height ||
                camInfo.renderTexture.graphicsFormat != graphicsFormat)
            {
                camInfo.cam.targetTexture = null;
                camInfo.renderTexture.Release();
                if (camInfo.renderTexture.graphicsFormat != graphicsFormat)
                {
                    // Debug.Log("Graphics format changed: " + graphicsFormat.ToString(), camInfo.cam.gameObject);
                    camInfo.renderTexture.graphicsFormat = graphicsFormat;
                }
                camInfo.renderTexture.width = renderTextureSize.x;
                camInfo.renderTexture.height = renderTextureSize.y;
                camInfo.cam.targetTexture = camInfo.renderTexture;
            }
        }

        float GetCamDepthOr0(Camera cam)
        {
            if (cam) return cam.depth;
            return 0;
        }

        void HandleRenderQuad(UPixelatorCameraInfo camInfo)
        {
            EnsureRenderQuad(camInfo);

            // camInfo.renderQuad.gameObject.layer = (int) Math.Log(layerMaskQuads, 2);
            // camInfo.parent.gameObject.layer = (int) Math.Log(layerMaskQuads, 2);
            camInfo.renderQuad.gameObject.layer = (int) Math.Log(layerMaskUI, 2);
            camInfo.parent.gameObject.layer = (int) Math.Log(layerMaskUI, 2);
            camInfo.renderQuad.name = "RenderQuad - " + camInfo.cam.name;
        }

        void EnsureUPixelatorRenderHandler(UPixelatorCameraInfo camInfo)
        {
            if (camInfo.renderHandler) return;

            camInfo.renderHandler = camInfo.parent.GetComponent<UPixelatorRenderHandler>();
            if (!camInfo.renderHandler) camInfo.renderHandler = camInfo.parent.gameObject.AddComponent<UPixelatorRenderHandler>();

            camInfo.renderHandler.name = "UPixelator - " + camInfo.cam.name;
        }

        void HandleUPixelatorRenderHandler(UPixelatorCameraInfo camInfo)
        {
            EnsureUPixelatorRenderHandler(camInfo);

            camInfo.renderHandler.uPixelator = this;
            camInfo.renderHandler.camInfo = camInfo;
            camInfo.renderHandler.enabled = camInfo.cam.enabled;
        }

        void EnsureParent(UPixelatorCameraInfo camInfo)
        {
            if (camInfo.parent) return;
            camInfo.parent = new GameObject().transform;
            camInfo.parent.transform.SetParent(transform, false);
        }

        void UpdateCamInfo(UPixelatorCameraInfo camInfo)
        {
            // CalculateParallax(camInfo);

            EnsureParent(camInfo);

            if (camInfo.cam == uPixelatorCam)
                camInfo.stabilize = false;
            else
            {
                HandleRenderQuad(camInfo);
                HandleResizeTexture(camInfo);
                HandleRenderQuadMaterial(camInfo);
            }
            HandleUPixelatorRenderHandler(camInfo);
        }

        void EnsureUPixelatorCam()
        {
            if (uPixelatorCam) return;
            uPixelatorCam = GetComponent<Camera>();
            if (!uPixelatorCam) uPixelatorCam = gameObject.AddComponent<Camera>();
            uPixelatorCam.enabled = true;
        }

        void HandleUPixelatorCam()
        {
            // TODO: why extra distance is needed?
            const float farClipPlaneExtraDistance = 1;

            if (mirroredCamera)
            {
                mirroredCamera.cullingMask &= ~layerMaskUI;

                uPixelatorCam.farClipPlane = mirroredCamera.farClipPlane;
                uPixelatorCam.nearClipPlane = mirroredCamera.nearClipPlane;
            }
            else
            {
                uPixelatorCam.transform.SetPositionAndRotation(Vector3.one * 1000, Quaternion.identity);
                uPixelatorCam.farClipPlane = cameraInfos.Count * camSliceGap + farClipPlaneExtraDistance;
                // TODO: -0.2 needed for TMPro Text to be shown
                // TODO: but at least -0.17 is needed for pixelMultiplier==1 to not clip?
                uPixelatorCam.nearClipPlane = - 0.2f;
            }

            uPixelatorCam.clearFlags = CameraClearFlags.SolidColor;
            uPixelatorCam.backgroundColor = Color.black;

            uPixelatorCam.orthographic = true;
            // uPixelatorCam.orthographicSize = uPixelatorZoom;

            // uPixelatorCam.allowHDR = true;
            uPixelatorCam.allowMSAA = false;

            uPixelatorCam.cullingMask = 0;
            uPixelatorCam.cullingMask |= layerMaskUI;
            // uPixelatorCam.cullingMask |= layerMaskQuads;

            // NOTE: this needs to be after all previous uPixelator textures to render properly
            if (cameraInfos.Where(c => c.cam != uPixelatorCam).Count() > 0)
                uPixelatorCam.depth = cameraInfos
                    .Where(c => c.cam != uPixelatorCam)
                    .Max(c => GetCamDepthOr0(c.cam))
                    + 1;
        }

        public bool IsGameObjectOnCameraLayer(GameObject g, Camera c)
        {
            int l = 1 << g.layer;
            return (c.cullingMask & l) != 0;
        }

        bool BelongsToCamera(UPixelatorSnappable s, Camera c)
        {
            // NOTE: since upixelatorCam mirrors mirroredCamera it sees the same gameObjects but should not own them here,
            //  upixelatorCam should only own the rectTransform elements that it sees as it doubles as the ui/overlay camera
            // NOTE: we hide the ui elements from main camera so that they are displayed in full resolution
            //  that is why they're not assigned here as they are not visible
            if (!s.rectTransform && c == uPixelatorCam) return false;
            if (!IsGameObjectOnCameraLayer(s.transform.gameObject, c)) return false;

            // NOTE: worldToScreenPoint is incorrectly calculated so use WorldToViewportPoint and pixels to get offset percent
            var offsetInPixels = 200;
            float offsetX = offsetInPixels / (float)Screen.width;
            float offsetY = offsetInPixels / (float)Screen.height;
            Vector3 viewportPoint = c.WorldToViewportPoint(s.transform.position);
            Rect extendedViewport = new Rect(-offsetX, -offsetY, 1 + 2 * offsetX, 1 + 2 * offsetY);

            return (
                extendedViewport.Contains(viewportPoint) && // isInViewport
                viewportPoint.z >= 0 && viewportPoint.z <= c.farClipPlane // isWithinDepth
            );
        }

        public void HandleInits()
        {
            if (dontMirrorCamera) mirroredCamera = null;
            else if (!mirroredCamera) mirroredCamera = Camera.main;

            if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
            {
                lastScreenSize = new Vector2Int(Screen.width, Screen.height);
                lastHandleInits = 0;
            }

            if (Time.time - lastHandleInits > handleInitsEvery)
            {
                lastHandleInits = Time.time;
                isSnappablesDirty = true;
                isCamerasDirty = true;
                isScreenSizeDirty = true;

#if UNITY_PIPELINE_URP
#if UNITY_EDITOR
                var urpAssets = PipelineAssets.GetUrpAssets();
                foreach (var urpAsset in urpAssets) SetupRenderFeatures.SetDownsamplingToNone(urpAsset);
#endif
#endif
            }
        }

        void HandleSnappables()
        {
            var snappables = FillSnappablesList();

            foreach (var camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;
                if (!camInfo.renderHandler) continue;

                camInfo.renderHandler.snappables.Clear();
                camInfo.renderHandler.snappables.Add(camInfo.renderHandler.GetCamSnappable());
            }

            foreach (var s in snappables)
            {
                if (s.isCamera) continue;

                var observerCamInfo = cameraInfos
                    .Where(c => c.cam)
                    .Where(c => c.cam.gameObject.activeInHierarchy)
                    .ToList()
                    .Find(c => BelongsToCamera(s, c.cam));

                if (observerCamInfo == null) continue;
                if (!observerCamInfo.renderHandler) continue;

                observerCamInfo.renderHandler.snappables.Add(s);
            }
        }

        List<UPixelatorSnappable> FillSnappablesList()
        {
            var snappableArray = GameObject.FindObjectsOfType<UPixelatorSnappable>();

            foreach (var snappable in snappableArray)
                snappable.nested = new List<UPixelatorSnappable>();

            var roots = new List<UPixelatorSnappable>();
            foreach (var snappable in snappableArray)
            {
                UPixelatorSnappable parent = snappable.transform.parent?.GetComponentInParent<UPixelatorSnappable>();
                if (!parent) roots.Add(snappable);
                else parent.nested.Add(snappable);
            }

            var flattened = new List<UPixelatorSnappable>();
            Action<UPixelatorSnappable> flatten = null;
            flatten = s => {
                flattened.Add(s);
                s.nested.ForEach(c => flatten(c));
            };
            roots.ForEach(flatten);

            return flattened;
        }
    }
}
