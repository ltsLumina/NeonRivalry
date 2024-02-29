using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TransitionsPlus {

    public delegate void TransitionEvent();

    public enum RenderMode {
        FullScreen = 0,
        InsideUI = 1,
        VR = 2
    }

    [ExecuteAlways]
    [HelpURL("https://kronnect.com/guides-category/transitions-plus/")]
    public class TransitionAnimator : MonoBehaviour {

        // public

        [Range(0, 1)]
        public float progress;

        public bool completed => progress >= 1f;

        [Tooltip("Starts the transition effect when game starts or gameobject becomes activated. Use SetProgress method to set the current animation progress")]
        public bool autoPlay = true;

        [Tooltip("Delay to start the transition")]
        public float playDelay;

        [Tooltip("Use unscaled time (not affected by Time.timeScale value")]
        public bool useUnscaledTime;

        [Tooltip("Automatically removes Transitions Plus from the scene when the transition ends playing")]
        public bool autoDestroy;

        [Tooltip("Delay to remove the transition after it's finished")]
        public float destroyDelay;

        [Tooltip("Destroy all transitions, not only this one")]
        public bool destroyAllTransitions;

        public RenderMode renderMode = RenderMode.FullScreen;
        public RawImage customScreen;

        [Tooltip("Used when launching several animations at the same time")]
        public int sortingOrder = 100;

        public TransitionProfile profile;

        [Tooltip("Event triggered when transition ends in autoPlay mode")]
        public UnityEvent onTransitionEnd;

        [Tooltip("If a scene should be loaded when transition effect ends (optional)")]
        public bool loadSceneAtEnd;

        [Tooltip("Name of the scene to load when transition effect ends")]
        public string sceneNameToLoad;

        public LoadSceneMode sceneLoadMode = LoadSceneMode.Single;

        public bool fadeToCamera;

        public Camera mainCamera;
        public Camera secondCamera;
        [Tooltip("When this parameter is enabled, Transition Plus will manage main and second camera activation states so while transition is being executed, the second camera remains disabled and when transition ends, the main camera is disabled and the second camera is enabled automatically.")]
        public bool switchActiveCamera = true;

        [Tooltip("Makes the center of the animation follow a target gameobject")]
        public bool autoFollow;
        public Transform followTarget;
        [Tooltip("An optional offset that can be added to the position of the followed gameobject. Useful to make the transition focus on certain part of the object.")]
        public Vector3 followPositionOffset;

        // private
        RawImage screen, defaultScreen;
        Canvas canvas;
        Material screenMat;
        float startTime;
        bool playing;
        const string TRANSITION_ROOT_NAME = "Transition Plus";
        Texture2D gradientTex;
        Color[] gradientColors;
        RenderTexture rtFirst, rtSecond;

        const int MAX_CENTERS = 32;
        readonly Vector4[] centers = new Vector4[MAX_CENTERS];
        float captureProgress;

        static class ShaderParams {
            public static readonly int T = Shader.PropertyToID("_T");
            public static readonly int NoiseIntensity = Shader.PropertyToID("_NoiseIntensity");
            public static readonly int VignetteIntensity = Shader.PropertyToID("_VignetteIntensity");
            public static readonly int ToonIntensity = Shader.PropertyToID("_ToonIntensity");
            public static readonly int ToonDotIntensity = Shader.PropertyToID("_ToonDotIntensity");
            public static readonly int ToonDotRadius = Shader.PropertyToID("_ToonDotRadius");
            public static readonly int ToonDotCount = Shader.PropertyToID("_ToonDotCount");
            public static readonly int MainTex = Shader.PropertyToID("_MainTex");
            public static readonly int NoiseTex = Shader.PropertyToID("_NoiseTex");
            public static readonly int SplashTex = Shader.PropertyToID("_SplashTex");
            public static readonly int RotationMultiplier = Shader.PropertyToID("_RotationMultiplier");
            public static readonly int Rotation = Shader.PropertyToID("_Rotation");
            public static readonly int Distortion = Shader.PropertyToID("_Distortion");
            public static readonly int GradientTex = Shader.PropertyToID("_GradientTex");
            public static readonly int Splits = Shader.PropertyToID("_Splits");
            public static readonly int AspectRatio = Shader.PropertyToID("_AspectRatio");
            public static readonly int Seed = Shader.PropertyToID("_Seed");
            public static readonly int Centers = Shader.PropertyToID("_Centers");
            public static readonly int CentersCount = Shader.PropertyToID("_CentersCount");
            public static readonly int Contrast = Shader.PropertyToID("_Contrast");
            public static readonly int CellDivisions = Shader.PropertyToID("_CellDivisions");
            public static readonly int Spread = Shader.PropertyToID("_Spread");
            public static readonly int Center = Shader.PropertyToID("_Center");
            public static readonly int TimeMultiplier = Shader.PropertyToID("_TimeMultiplier");
            public static readonly int MaskTex = Shader.PropertyToID("_MaskTex");
            public static readonly int FirstCameraTex = Shader.PropertyToID("_FirstCameraTex");

            public const string SKW_GRADIENT_TIME = "GRADIENT_TIME";
            public const string SKW_GRADIENT_OPACITY = "GRADIENT_OPACITY";
            public const string SKW_GRADIENT_SPATIAL_RADIAL = "GRADIENT_SPATIAL_RADIAL";
            public const string SKW_GRADIENT_SPATIAL_HORIZONTAL = "GRADIENT_SPATIAL_HORIZONTAL";
            public const string SKW_GRADIENT_SPATIAL_VERTICAL = "GRADIENT_SPATIAL_VERTICAL";
            public const string SKW_TEXTURE = "TEXTURE";
            public const string SKW_TOON = "TOON";

        }

        public RenderTexture mainCameraCapture => rtFirst;
        public RenderTexture secondCameraCapture => rtSecond;


        public void SetProfile(TransitionProfile profile) {
            this.profile = profile;
            UpdateMaterialProperties();
        }

        private void OnEnable() {
            if (playing && progress < 1f && profile != null && profile.duration > 0) {
                startTime = GetTime() - progress * profile.duration;
            }
        }

        private void OnDisable() {
            if (profile != null) {
                profile.onSettingsChanged -= UpdateMaterialProperties;
            }
        }

        private void OnDestroy() {
            ReleaseCameraTextures();
        }

        void OnValidate() {
            UpdateMaterialProperties();
        }

        void Start() {
            UpdateMaterialProperties();
#if UNITY_EDITOR
// inside Editor, the capure/toggle canvas from UpdateMaterialProperties() is delayed on frame to avoid OnValidate errors so we force the refresh here, to ensure camera image is present on frame 0
            CaptureCameras();
            ToggleCanvas();
#endif
            if (Application.isPlaying) {
                if (autoPlay) {
                    Invoke(nameof(StartTransition), playDelay);
                }
            }
        }

        void StartTransition() {
            playing = true;
            startTime = GetTime();
            if (profile != null && profile.sound != null) {
                Invoke(nameof(PlayAudio), profile.soundDelay);
            }
        }

        void Update() {

            if (screenMat == null) return;

            if (autoFollow && followTarget != null && mainCamera != null) {
                Vector2 center = mainCamera.WorldToViewportPoint(followTarget.position + followPositionOffset);
                center.x -= 0.5f;
                center.y -= 0.5f;
                screenMat.SetVector(ShaderParams.Center, center);
            }

            if (!playing || profile == null || !Application.isPlaying) return;

            float t = profile.duration > 0 ? Mathf.Clamp01((GetTime() - startTime) / profile.duration) : 1;
            SetProgress(t);

            if (t >= 1) {
                enabled = false;
                onTransitionEnd?.Invoke();
                if (autoDestroy) {
                    if (destroyAllTransitions) {
                        TransitionAnimator[] animators = Misc.FindObjectsOfType<TransitionAnimator>();
                        foreach (TransitionAnimator anim in animators) {
                            Destroy(anim.gameObject, destroyDelay);
                        }
                    } else {
                        Destroy(gameObject, destroyDelay);
                    }
                }
                if (loadSceneAtEnd && !string.IsNullOrEmpty(sceneNameToLoad)) {
                    SceneManager.LoadScene(sceneNameToLoad, sceneLoadMode);
                }
            }
        }

        public void UpdateMaterialProperties() {

            if (gameObject == null) return;

            if (canvas == null) {
                canvas = GetComponentInChildren<Canvas>();
                if (canvas == null) return;
            }
            canvas.sortingOrder = sortingOrder;

            if (defaultScreen == null) {
                defaultScreen = canvas.GetComponentInChildren<RawImage>();
            }

            if (renderMode == RenderMode.InsideUI) {
                screen = customScreen;
                if (screen == null) {
                    screen = defaultScreen;
                }
            } else {
                screen = defaultScreen;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () => {
                    try
                    {
                        UnityEditor.EditorApplication.delayCall += () => {
                            if (canvas != null) {
                                UpdateCanvasProperties();
                            }
                        };
                    }
                    catch { }
                };
#else
                UpdateCanvasProperties();
#endif
            }

            if (screen == null) return;

            if (mainCamera == null) mainCamera = Camera.main;

            if (profile == null) {
                canvas.enabled = false;
                return;
            }

            if (fadeToCamera) profile.colorMode = ColorMode.Texture;

            profile.ValidateSettings();

            // Subscribe to profile changes
            profile.onSettingsChanged -= UpdateMaterialProperties;
            profile.onSettingsChanged += UpdateMaterialProperties;

            screenMat = screen.material;
            string transitionTypeName = profile.type.ToString();
            if (screenMat == null || screenMat.name != transitionTypeName) {
                Shader shader = Shader.Find("TransitionsPlus/" + transitionTypeName);
                if (shader == null) {
                    Debug.LogError("Shader for transition " + transitionTypeName + " can't be loaded.");
                    return;
                }
                screenMat = new Material(shader);
                screenMat.name = transitionTypeName;
            }

            screen.material = null;
            screen.material = screenMat;

            screenMat.SetFloat(ShaderParams.NoiseIntensity, profile.type.SupportsNoise() ? profile.noiseIntensity : 0);
            screenMat.SetFloat(ShaderParams.VignetteIntensity, profile.type.SupportsVignette() ? profile.vignetteIntensity : 0);
            screenMat.SetTexture(ShaderParams.SplashTex, profile.splashTexture);
            screenMat.SetTextureScale(ShaderParams.NoiseTex, profile.noiseScale);
            screenMat.SetInt(ShaderParams.ToonIntensity, profile.type.SupportsToon() ? profile.toonGradientIntensity : 1);
            screenMat.SetFloat(ShaderParams.ToonDotIntensity, profile.type.SupportsToon() ? profile.toonDotIntensity : 0);
            screenMat.SetFloat(ShaderParams.ToonDotRadius, profile.toonDotRadius);
            screenMat.SetInt(ShaderParams.ToonDotCount, profile.type.SupportsToon() ? profile.toonDotCount : 1);
            if (profile.type.SupportsRotation()) {
                screenMat.SetFloat(ShaderParams.Rotation, profile.rotation * Mathf.Deg2Rad);
                screenMat.SetFloat(ShaderParams.RotationMultiplier, profile.rotationMultiplier);
            } else {
                screenMat.SetFloat(ShaderParams.Rotation, 0);
                screenMat.SetFloat(ShaderParams.RotationMultiplier, 0);
            }
            screenMat.SetFloat(ShaderParams.Distortion, profile.distortion);
            screenMat.SetInt(ShaderParams.Splits, profile.splits);
            screenMat.SetFloat(ShaderParams.AspectRatio, profile.type.SupportsAspectRatio() && profile.keepAspectRatio ? 1 : 0);
            screenMat.SetInt(ShaderParams.Seed, profile.seed);
            screenMat.SetInt(ShaderParams.CentersCount, profile.centersCount);
            screenMat.SetFloat(ShaderParams.Contrast, profile.type.SupportsContrast() ? profile.contrast : 1f);
            screenMat.SetTexture(ShaderParams.NoiseTex, profile.noiseTexture);
            screenMat.SetInt(ShaderParams.CellDivisions, profile.cellDivisions);
            screenMat.SetFloat(ShaderParams.Spread, profile.spread);
            screenMat.SetVector(ShaderParams.Center, profile.type.SupportsCenter() ? profile.center : Vector2.zero);
            screenMat.SetFloat(ShaderParams.TimeMultiplier, profile.timeMultiplier);
            screenMat.SetTexture(ShaderParams.MaskTex, profile.shapeTexture);

            screenMat.shaderKeywords = null;
            screenMat.enabledKeywords = null;
            if (profile.colorMode == ColorMode.Texture || fadeToCamera) {
                screenMat.EnableKeyword(ShaderParams.SKW_TEXTURE);
            } else if (profile.colorMode == ColorMode.Gradient) {
                switch (profile.gradientMode) {
                    case GradientMode.Opacity:
                        screenMat.EnableKeyword(ShaderParams.SKW_GRADIENT_OPACITY);
                        break;
                    case GradientMode.Time:
                        screenMat.EnableKeyword(ShaderParams.SKW_GRADIENT_TIME);
                        break;
                    case GradientMode.SpatialRadial:
                        screenMat.EnableKeyword(ShaderParams.SKW_GRADIENT_SPATIAL_RADIAL);
                        break;
                    case GradientMode.SpatialHorizontal:
                        screenMat.EnableKeyword(ShaderParams.SKW_GRADIENT_SPATIAL_HORIZONTAL);
                        break;
                    case GradientMode.SpatialVertical:
                        screenMat.EnableKeyword(ShaderParams.SKW_GRADIENT_SPATIAL_VERTICAL);
                        break;
                }
            }
            if ((profile.toonDotIntensity > 0 || profile.toonGradientIntensity > 1f) && profile.type.SupportsToon()) {
                screenMat.EnableKeyword(ShaderParams.SKW_TOON);
            }

            // compute cells centers
            if (profile.type.SupportsCentersCount()) {
                Random.State state = Random.state;
                Random.InitState(profile.seed);
                for (int k = 0; k < profile.centersCount; k++) {
                    Vector2 position;
                    position.x = Random.value;
                    position.y = Random.value;
                    float radiusMultiplier = Random.value;
                    radiusMultiplier = radiusMultiplier * 0.25f + 0.75f;
                    float timeShift = Random.value;
                    centers[k].x = position.x;
                    centers[k].y = position.y;
                    centers[k].z = radiusMultiplier;
                    centers[k].w = timeShift;
                }
                Random.state = state;
                screenMat.SetVectorArray(ShaderParams.Centers, centers);
            }


            if (gradientTex == null) {
                gradientTex = new Texture2D(256, 1);
                gradientTex.wrapMode = TextureWrapMode.Clamp;
            }
            if (gradientColors == null || gradientColors.Length != 256) {
                gradientColors = new Color[256];
            }
            if (profile.colorMode == ColorMode.SingleColor) {
                Color whiteColor = Color.white;
                for (int k = 0; k < 256; k++) {
                    float f = k / 255f;
                    gradientColors[k] = whiteColor;
                }
            } else {
                for (int k = 0; k < 256; k++) {
                    float f = k / 255f;
                    gradientColors[k] = profile.gradient.Evaluate(f);
                }
            }
            gradientTex.SetPixels(gradientColors);
            gradientTex.Apply();

            screenMat.SetTexture(ShaderParams.GradientTex, gradientTex);

            SetProgress(progress);

            AssignFirstCameraTexture();
            AssignSecondCameraTextureAndColor();
        }

        void UpdateCanvasProperties() {
            if (renderMode == RenderMode.VR && mainCamera != null) {
                canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceCamera;
                canvas.planeDistance = 1;
                canvas.worldCamera = mainCamera;
            } else {
                canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
            }
        }

        void AssignSecondCameraTextureAndColor() {
            if (profile == null || screenMat == null || screen == null) return;
            if (fadeToCamera && rtSecond != null) {
                screen.texture = rtSecond;
                screenMat.SetTexture(ShaderParams.MainTex, rtSecond);
            } else if (profile.colorMode == ColorMode.Texture) {
                screen.texture = profile.texture;
                screenMat.SetTexture(ShaderParams.MainTex, profile.texture);
            } else {
                screen.texture = Texture2D.whiteTexture;
                screenMat.SetTexture(ShaderParams.MainTex, Texture2D.whiteTexture);
            }

            if (profile.colorMode == ColorMode.SingleColor) {
                screenMat.color = profile.color;
            } else {
                screenMat.color = Color.white;
            }
        }


        void AssignFirstCameraTexture() {
            if (profile == null || screenMat == null) return;
            screenMat.SetTexture(ShaderParams.FirstCameraTex, rtFirst);
        }

        void ToggleCanvas() {
            if (canvas == null) return;
            if (renderMode == RenderMode.InsideUI && screen != defaultScreen) {
                canvas.enabled = false;
                return;
            }

            if (profile != null) {
                canvas.enabled = profile.invert ? progress < 1f : progress > 0;
            }
        }

        void CaptureCameras() {
            if (captureProgress == 0 || rtFirst == null) {
                CaptureFirstCameraTexture();
            }
            if (captureProgress == 0 || rtSecond == null) {
                CaptureSecondCameraTexture();
            }
            captureProgress = progress;
        }

        void CaptureFirstCameraTexture() {
            ReleaseFirstCameraTexture();
            if (profile == null || (!fadeToCamera && !profile.type.RequiresFirstCameraCapture()) || mainCamera == null) return;

            RenderTexture prevTarget = mainCamera.targetTexture;
            rtFirst = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 24);
            mainCamera.targetTexture = rtFirst;
            mainCamera.Render();
            mainCamera.targetTexture = prevTarget;
            AssignFirstCameraTexture();
        }

        void CaptureSecondCameraTexture() {
            ReleaseSecondCameraTexture();
            if (!fadeToCamera || secondCamera == null) return;

            RenderTexture prevTarget = secondCamera.targetTexture;
            rtSecond = new RenderTexture(secondCamera.pixelWidth, secondCamera.pixelHeight, 24);
            secondCamera.targetTexture = rtSecond;
            secondCamera.Render();
            secondCamera.targetTexture = prevTarget;
            AssignSecondCameraTextureAndColor();
        }

        void ReleaseCameraTextures() {
            ReleaseFirstCameraTexture();
            ReleaseSecondCameraTexture();
        }

        void ReleaseFirstCameraTexture() {
            if (rtFirst != null) {
                rtFirst.Release();
                DestroyImmediate(rtFirst);
            }
        }

        void ReleaseSecondCameraTexture() {
            if (rtSecond != null) {
                rtSecond.Release();
                DestroyImmediate(rtSecond);
            }
        }

        #region API

        public float GetTime() {
            return useUnscaledTime ? Time.unscaledTime : Time.time;
        }

        public void SetProgress(float t) {
            t = Mathf.Clamp01(t);
            progress = t;

            if (profile != null) {
                if (profile.type.SupportsTimeMultiplier()) {
                    t *= profile.timeMultiplier;
                }
                screenMat.SetFloat(ShaderParams.T, profile.invert ? 1f - t : t);
            }

            if (fadeToCamera && switchActiveCamera) {
                if (secondCamera != null) {
                    secondCamera.gameObject.SetActive(true);
                    secondCamera.enabled = progress >= 1f;
                    if (mainCamera != null) {
                        mainCamera.gameObject.SetActive(true);
                        mainCamera.enabled = progress < 1f;
                    }
                }
            }


#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => {
                try {
                    UnityEditor.EditorApplication.delayCall += () => {
                        CaptureCameras();
                        ToggleCanvas();
                    };
                }
                catch { }
            };
#else
            CaptureCameras();
            ToggleCanvas();
#endif
        }

        public void PlayAudio() {
            if (profile == null || profile.sound == null) return;
            AudioListener listener = Misc.FindObjectOfType<AudioListener>();
            if (listener != null) {
                AudioSource.PlayClipAtPoint(profile.sound, listener.transform.position);
            }
        }


        public static GameObject CreateTransition() {
            GameObject o = Resources.Load<GameObject>("Prefabs/Transitions Plus");
            if (o == null) {
                Debug.LogError("Couldn't find Transitions Root prefab.");
                return null;
            }

            o = Instantiate(o);
            o.name = TRANSITION_ROOT_NAME;

            return o;
        }

        static TransitionAnimator CreateTransitionAnimator() {
            GameObject o = CreateTransition();
            return o.GetComponentInChildren<TransitionAnimator>();
        }


        /// <summary>
        /// Starts a new transition from code.
        /// </summary>
        /// <param name="customScreen">The RawImage where transition should be inserted when renderMode is InsideUI</param>
        /// <returns></returns>
        public static TransitionAnimator Start(TransitionProfile profile, bool autoDestroy = true, float destroyDelay = 1f, string sceneNameToLoad = null, LoadSceneMode sceneLoadMode = LoadSceneMode.Single, bool fadeToCamera = false, Camera mainCamera = null, Camera secondCamera = null, bool switchActiveCamera = true, bool autoFollow = false, Transform followTarget = null, int sortingOrder = int.MinValue, float playDelay = 0, Vector3 followPositionOffset = default, RenderMode renderMode = RenderMode.FullScreen, RawImage customScreen = null) {

            TransitionAnimator animator = CreateTransitionAnimator();
            animator.autoDestroy = autoDestroy;
            animator.destroyDelay = destroyDelay;
            animator.autoFollow = autoFollow;
            animator.followTarget = followTarget;
            animator.followPositionOffset = followPositionOffset;
            animator.renderMode = renderMode;
            animator.customScreen = customScreen;
            if (sortingOrder > int.MinValue) {
                animator.sortingOrder = sortingOrder;
            }
            animator.playDelay = playDelay;
            if (!string.IsNullOrEmpty(sceneNameToLoad)) {
                animator.loadSceneAtEnd = true;
                animator.sceneNameToLoad = sceneNameToLoad;
                animator.sceneLoadMode = sceneLoadMode;
            } else if (fadeToCamera && secondCamera) {
                animator.fadeToCamera = true;
                animator.mainCamera = mainCamera;
                animator.secondCamera = secondCamera;
                animator.switchActiveCamera = switchActiveCamera;
            }
            animator.SetProfile(profile);
            return animator;

        }

        public static TransitionAnimator Start(TransitionType type, float duration = 2f, Color color = default, Gradient gradient = null, Texture2D texture = null, Vector2 center = default, bool keepAspectRatio = false, float rotation = 0, float rotationMultiplier = 0, float toonDotIntensity = 0, int toonGradientIntensity = 1, float noiseIntensity = 0.5f, Texture2D noiseTex = null, Vector2 noiseScale = default, bool invert = false, bool autoDestroy = true, float vignetteIntensity = 0.5f, float contrast = 1f, int splits = 5, int centersCount = 8, int seed = 0, int cellsDivisions = 64, float spread = 16f, float destroyDelay = 1, string sceneNameToLoad = null, LoadSceneMode sceneLoadMode = LoadSceneMode.Single, AudioClip sound = null, bool fadeToCamera = false, Camera mainCamera = null, Camera secondCamera = null, bool switchActiveCamera = true, float timeMultiplier = 1f, bool autoFollow = false, Transform followTarget = null, Texture2D shapeTexture = null, int sortingOrder = int.MinValue, float playDelay = 0, Vector3 followPositionOffset = default, RenderMode renderMode = RenderMode.FullScreen, RawImage customScreen = null) {

            TransitionProfile profile = ScriptableObject.CreateInstance<TransitionProfile>();
            profile.type = type;
            profile.duration = duration;
            profile.invert = invert;
            if (gradient != null) {
                profile.colorMode = ColorMode.Gradient;
                profile.gradient = gradient;
            } else if (texture != null) {
                profile.colorMode = ColorMode.Texture;
                profile.texture = texture;
            } else {
                profile.colorMode = ColorMode.SingleColor;
                profile.color = color != default ? color : Color.black;
            }
            profile.rotation = rotation;
            profile.rotationMultiplier = rotationMultiplier;
            profile.toonDotIntensity = toonDotIntensity;
            profile.toonGradientIntensity = toonGradientIntensity;
            profile.noiseIntensity = noiseIntensity;
            profile.noiseTexture = noiseTex;
            profile.noiseScale = noiseScale != default ? noiseScale : Vector2.one;
            profile.vignetteIntensity = vignetteIntensity;
            profile.splits = splits;
            profile.centersCount = centersCount;
            profile.seed = seed;
            profile.contrast = contrast;
            profile.cellDivisions = cellsDivisions;
            profile.spread = spread;
            profile.sound = sound;
            profile.center = center;
            profile.keepAspectRatio = keepAspectRatio;
            profile.timeMultiplier = timeMultiplier;
            profile.shapeTexture = shapeTexture;

            return Start(profile, autoDestroy, destroyDelay, sceneNameToLoad, sceneLoadMode, fadeToCamera, mainCamera, secondCamera, switchActiveCamera, autoFollow, followTarget, sortingOrder, playDelay, followPositionOffset, renderMode, customScreen);
        }

        #endregion
    }


}