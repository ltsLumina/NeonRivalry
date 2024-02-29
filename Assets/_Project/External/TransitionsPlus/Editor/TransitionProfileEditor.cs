using UnityEditor;
using UnityEngine;

namespace TransitionsPlus {

    [CustomEditor(typeof(TransitionProfile))]
    public class TransitionProfileEditor : Editor {
        SerializedProperty type, invert, timeMultiplier;
        SerializedProperty colorMode, color, gradient, gradientMode, shapeTexture;
        SerializedProperty duration;
        SerializedProperty noiseIntensity, vignetteIntensity, texture;
        SerializedProperty noiseScale, noiseTexture;
        SerializedProperty contrast;
        SerializedProperty toonGradientIntensity, toonDotIntensity, toonDotRadius, toonDotCount;
        SerializedProperty distortion, rotation, rotationMultiplier, splits, center, keepAspectRatio;
        SerializedProperty seed, centersCount;
        SerializedProperty cellDivisions, spread;
        SerializedProperty splashTexture;
        SerializedProperty sound, soundDelay;

        TransitionAnimator animator;
        TransitionProfile profile;

        void OnEnable() {

            try {

                type = serializedObject.FindProperty("type");
                invert = serializedObject.FindProperty("invert");
                timeMultiplier = serializedObject.FindProperty("timeMultiplier");

                colorMode = serializedObject.FindProperty("colorMode");
                color = serializedObject.FindProperty("color");
                gradient = serializedObject.FindProperty("gradient");
                gradientMode = serializedObject.FindProperty("gradientMode");
                shapeTexture = serializedObject.FindProperty("shapeTexture");

                duration = serializedObject.FindProperty("duration");

                keepAspectRatio = serializedObject.FindProperty("keepAspectRatio");
                center = serializedObject.FindProperty("center");

                contrast = serializedObject.FindProperty("contrast");

                toonGradientIntensity = serializedObject.FindProperty("toonGradientIntensity");
                toonDotIntensity = serializedObject.FindProperty("toonDotIntensity");
                toonDotRadius = serializedObject.FindProperty("toonDotRadius");
                toonDotCount = serializedObject.FindProperty("toonDotCount");

                noiseIntensity = serializedObject.FindProperty("noiseIntensity");
                noiseScale = serializedObject.FindProperty("noiseScale");
                vignetteIntensity = serializedObject.FindProperty("vignetteIntensity");

                texture = serializedObject.FindProperty("texture");
                noiseTexture = serializedObject.FindProperty("noiseTexture");
                splashTexture = serializedObject.FindProperty("splashTexture");

                distortion = serializedObject.FindProperty("distortion");
                rotation = serializedObject.FindProperty("rotation");
                rotationMultiplier = serializedObject.FindProperty("rotationMultiplier");
                splits = serializedObject.FindProperty("splits");

                seed = serializedObject.FindProperty("seed");
                centersCount = serializedObject.FindProperty("centersCount");

                cellDivisions = serializedObject.FindProperty("cellDivisions");
                spread = serializedObject.FindProperty("spread");

                sound = serializedObject.FindProperty("sound");
                soundDelay = serializedObject.FindProperty("soundDelay");

                profile = (TransitionProfile)target;

            } catch { }
        }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            TransitionType transitionType = (TransitionType)type.intValue;

            EditorGUILayout.PropertyField(type);
            EditorGUILayout.PropertyField(duration);
            EditorGUILayout.PropertyField(invert);
            if (transitionType.SupportsTimeMultiplier()) {
                EditorGUILayout.PropertyField(timeMultiplier);
            }

            if (profile.displayingAnimator == null || !profile.displayingAnimator.fadeToCamera) {
                EditorGUILayout.PropertyField(colorMode);
                switch ((ColorMode)colorMode.intValue) {
                    case ColorMode.SingleColor:
                        EditorGUILayout.PropertyField(color);
                        break;
                    case ColorMode.Gradient:
                        EditorGUILayout.PropertyField(gradient);
                        EditorGUILayout.PropertyField(gradientMode);
                        break;
                    default:
                        EditorGUILayout.PropertyField(texture);
                        break;
                }
            }

            if (transitionType.SupportsShapeTexture()) {
                EditorGUILayout.PropertyField(shapeTexture);
            }

            if (transitionType.SupportsCenter()) {
                EditorGUILayout.PropertyField(center);
            }

            if (transitionType.SupportsAspectRatio()) {
                EditorGUILayout.PropertyField(keepAspectRatio);
            }

            if (transitionType.SupportsSplits()) {
                EditorGUILayout.PropertyField(splits);
            }

            if (transitionType.SupportsCellsDivisions()) {
                EditorGUILayout.PropertyField(cellDivisions);
            }

            if (transitionType.SupportsSpread()) {
                EditorGUILayout.PropertyField(spread);
            }

            if (transitionType.SupportsSeed()) {
                EditorGUILayout.PropertyField(seed);

            }

            if (transitionType.SupportsCentersCount()) {
                EditorGUILayout.PropertyField(centersCount);
            }

            if (transitionType.SupportsRotation()) {
                EditorGUILayout.PropertyField(rotation);
                EditorGUILayout.PropertyField(rotationMultiplier);
                if (rotation.floatValue != 0 || rotationMultiplier.floatValue != 0) {
                    EditorGUILayout.PropertyField(distortion, new GUIContent("Twist"));
                }
            }

            if (transitionType.SupportsVignette()) {
                EditorGUILayout.PropertyField(vignetteIntensity);
            }

            if (transitionType.SupportsContrast()) {
                EditorGUILayout.PropertyField(contrast);
            }

            if (transitionType.SupportsNoise()) {
                EditorGUILayout.PropertyField(noiseIntensity);
                if (noiseIntensity.floatValue > 0) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(noiseScale, new GUIContent("Scale"));
                    EditorGUILayout.PropertyField(noiseTexture, new GUIContent("Texture"));
                    EditorGUI.indentLevel--;
                }
            }

            if (transitionType.SupportsSplashTexture()) {
                EditorGUILayout.PropertyField(splashTexture, new GUIContent("Splash Texture"));
            }

            if (transitionType.SupportsToon()) {
                EditorGUILayout.PropertyField(toonGradientIntensity, new GUIContent("Toon Gradient"));
                EditorGUILayout.PropertyField(toonDotIntensity, new GUIContent("Toon Dot"));
                if (toonDotIntensity.floatValue > 0) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(toonDotRadius, new GUIContent("Dot Radius"));
                    EditorGUILayout.PropertyField(toonDotCount, new GUIContent("Dot Count"));
                    EditorGUI.indentLevel--;
                }

            }

            EditorGUILayout.PropertyField(sound);
            if (sound.objectReferenceValue != null) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(soundDelay, new GUIContent("Delay"));
                EditorGUI.indentLevel--;
            }

            if (serializedObject.ApplyModifiedProperties()) {
                TransitionAnimator[] animators = Misc.FindObjectsOfType<TransitionAnimator>();
                TransitionProfile thisProfile = (TransitionProfile)target;
                foreach(TransitionAnimator animator in animators) {
                    if (animator.profile == thisProfile) {
                        animator.UpdateMaterialProperties();
                    }
                }
            }
        }

    }

}