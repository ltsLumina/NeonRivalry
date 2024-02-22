using System;
using UnityEngine;

namespace TransitionsPlus {

#if UNITY_2022_1_OR_NEWER
    [InspectorOrder(InspectorSort.ByName, InspectorSortDirection.Ascending)]
#endif
    public enum TransitionType {
        Fade = 0,
        Mosaic = 1,
        Dissolve = 2,
        Tiles = 3,
        Shape = 4,
        Wipe = 20,
        DoubleWipe = 21,
        CrossWipe = 22,
        CircularWipe = 23,
        Burn = 30,
        BurnSquare = 31,
        Splash = 33,
        SeaWaves = 40,
        Screen = 50,
        Smear = 100,
        Slide = 110,
        DoubleSlide = 111,
        Cube = 112,
        Melt = 120
    }

    public enum ColorMode {
        SingleColor,
        Gradient,
        Texture
    }

    public enum GradientMode {
        Opacity,
        Time = 10,
        SpatialRadial = 20,
        SpatialHorizontal = 21,
        SpatialVertical = 22
    }

    public delegate void OnSettingsChanged();

    [CreateAssetMenu(fileName = "TransitionProfile", menuName = "Transition Profile", order = 250)]
    [HelpURL("https://kronnect.com/guides-category/transitions-plus/")]
    public class TransitionProfile : ScriptableObject {

        [NonSerialized]
        public TransitionAnimator displayingAnimator;

        public TransitionType type = TransitionType.Fade;

        public bool invert;
        [Tooltip("Multiplies progress value by a custom value to adjust animation timing")]
        public float timeMultiplier = 1f;
        public ColorMode colorMode = ColorMode.SingleColor;
        public Color color = Color.black;
        public Gradient gradient;
        [Tooltip("Specifies if gradient should be based on the transition pixel opacity or the current transition time")]
        public GradientMode gradientMode = GradientMode.Opacity;
        [Tooltip("Optional transition SDF (Single Distance Field) shape texture that produces variety of effects. Use a SDF generator to convert a texture into a SDF texture (example: https://jobtalle.com/SDFMaker/)")]
        public Texture2D shapeTexture;
        public float duration = 2;
        [Range(0, 1)]
        public float noiseIntensity = 0.5f;
        [Range(0, 1)]
        public float vignetteIntensity = 1f;
        [Range(1, 256)]
        public int toonGradientIntensity = 64;
        [Range(0, 1)]
        public float toonDotIntensity = 0.5f;
        [Range(0.01f, 0.8f)]
        public float toonDotRadius = 0.333f;
        public int toonDotCount = 32;
        public float rotationMultiplier;
        public float rotation;
        [Range(0f, 5f)]
        public float distortion;
        public int splits = 2;
        public Vector2 center;

        [Tooltip("Compensates difference between screen width/height so the effect doesn't stretch.")]
        public bool keepAspectRatio;
        public int seed;
        [Range(1, 32)]
        public int centersCount = 16;
        public Texture2D texture;
        public int cellDivisions = 32;
        public float spread = 64;

        public Vector2 noiseScale = Vector2.one;
        public Texture2D noiseTexture;

        public float contrast = 1f;

        public Texture2D splashTexture;

        public AudioClip sound;
        [Tooltip("Optional delay to play the sound when transition starts")]
        public float soundDelay;

        public event OnSettingsChanged onSettingsChanged;

        private void OnEnable() {
            ValidateSettings();
        }

        private void OnValidate() {
            ValidateSettings();
            if (onSettingsChanged != null) {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () => {
                    try {
                        onSettingsChanged();
                        UnityEditor.EditorApplication.delayCall += () => UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }
                    catch { }
                };
#else
                onSettingsChanged();
#endif
            }
        }

        public void ValidateSettings() {
            duration = Mathf.Max(0, duration);
            seed = Mathf.Max(0, seed);
            toonDotCount = Mathf.Max(8, toonDotCount);
            contrast = Mathf.Max(1f, contrast);
            spread = Mathf.Max(1f, spread);
            cellDivisions = Mathf.Max(1, cellDivisions);
            soundDelay = Mathf.Max(0, soundDelay);
            splits = Mathf.Max(1, splits);
            center.x = Mathf.Clamp(center.x, -1, 2);
            center.y = Mathf.Clamp(center.y, -1, 2);

            if (gradient == null) {
                gradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0] = new GradientColorKey(Color.white, 0);
                colorKeys[1] = new GradientColorKey(Color.white, 1);
                gradient.colorKeys = colorKeys;
            }
            if (noiseTexture == null) {
                noiseTexture = Resources.Load<Texture2D>("Textures/DefaultNoiseTex");
            }
            if (splashTexture == null) {
                splashTexture = Resources.Load<Texture2D>("Textures/Splash1");
            }
            if (shapeTexture == null) {
                shapeTexture = Resources.Load<Texture2D>("Textures/StarSDF");
            }
        }

    }


    public static class TransitionTypeExtensions {

        public static bool SupportsNoise(this TransitionType type) {
            return type == TransitionType.Wipe || type == TransitionType.Fade || type == TransitionType.DoubleWipe || type == TransitionType.CrossWipe || type == TransitionType.Burn || type == TransitionType.BurnSquare || type == TransitionType.Tiles || type == TransitionType.CircularWipe || type == TransitionType.Screen || type == TransitionType.DoubleSlide || type == TransitionType.Melt;
        }

        public static bool SupportsToon(this TransitionType type) {
            return type == TransitionType.Wipe || type == TransitionType.Fade || type == TransitionType.DoubleWipe || type == TransitionType.CrossWipe || type == TransitionType.Burn || type == TransitionType.BurnSquare || type == TransitionType.CircularWipe || type == TransitionType.Screen;
        }

        public static bool SupportsRotation(this TransitionType type) {
            return type == TransitionType.Wipe || type == TransitionType.DoubleWipe || type == TransitionType.CrossWipe || type == TransitionType.BurnSquare || type == TransitionType.Mosaic || type == TransitionType.Dissolve || type == TransitionType.Tiles || type == TransitionType.Splash || type == TransitionType.SeaWaves || type == TransitionType.CircularWipe || type == TransitionType.Screen || type == TransitionType.Shape || type == TransitionType.Slide || type == TransitionType.DoubleSlide;
        }

        public static bool SupportsSplits(this TransitionType type) {
            return type == TransitionType.CrossWipe || type == TransitionType.SeaWaves || type == TransitionType.Screen || type == TransitionType.Shape;
        }

        public static bool SupportsAspectRatio(this TransitionType type) {
            return type == TransitionType.Fade || type == TransitionType.Wipe || type == TransitionType.DoubleWipe || type == TransitionType.CrossWipe || type == TransitionType.Splash || type == TransitionType.Screen || type == TransitionType.Tiles || type == TransitionType.Shape;
        }

        public static bool SupportsCenter(this TransitionType type) {
            return type == TransitionType.Fade || type == TransitionType.CircularWipe || type == TransitionType.Shape || type == TransitionType.Wipe || type == TransitionType.Smear;
        }

        public static bool SupportsSeed(this TransitionType type) {
            return type == TransitionType.Burn || type == TransitionType.BurnSquare || type == TransitionType.Dissolve || type == TransitionType.Splash;
        }

        public static bool SupportsShapeTexture(this TransitionType type) {
            return type == TransitionType.Shape;
        }

        public static bool SupportsCentersCount(this TransitionType type) {
            return type == TransitionType.Burn || type == TransitionType.BurnSquare || type == TransitionType.Splash;
        }

        public static bool SupportsSpread(this TransitionType type) {
            return type == TransitionType.Mosaic;
        }

        public static bool SupportsCellsDivisions(this TransitionType type) {
            return type == TransitionType.Mosaic || type == TransitionType.Dissolve || type == TransitionType.Tiles;
        }

        public static bool SupportsSplashTexture(this TransitionType type) {
            return type == TransitionType.Splash;
        }

        public static bool SupportsVignette(this TransitionType type) {
            return type == TransitionType.Fade;
        }

        public static bool SupportsContrast(this TransitionType type) {
            return type != TransitionType.Shape && type != TransitionType.Smear && type != TransitionType.Slide && type != TransitionType.Melt && type != TransitionType.Cube;
        }

        public static bool SupportsTimeMultiplier(this TransitionType type) {
            return type != TransitionType.Cube;
        }

        public static bool RequiresFirstCameraCapture(this TransitionType type) {
            return type == TransitionType.Smear || type == TransitionType.Slide || type == TransitionType.DoubleSlide || type == TransitionType.Melt || type == TransitionType.Cube;
        }

    }



}