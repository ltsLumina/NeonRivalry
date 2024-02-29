using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TransitionsPlus {

    [CustomEditor(typeof(TransitionAnimator))]
    public class TransitionAnimatorEditor : Editor {

        SerializedProperty progress;
        SerializedProperty autoPlay, playDelay, useUnscaledTime, autoDestroy, destroyDelay, destroyAllTransitions, onTransitionEnd;
        SerializedProperty profile;
        SerializedProperty loadSceneAtEnd, sceneNameToLoad, sceneLoadMode;
        SerializedProperty fadeToCamera, mainCamera, secondCamera, switchActiveCamera;
        SerializedProperty renderMode, customScreen, sortingOrder;
        SerializedProperty autoFollow, followTarget, followPositionOffset;

        TransitionProfile cachedProfile;
        Editor cachedProfileEditor;
        static GUIStyle boxStyle;
        const string MASTER_FOLDER_NAME = "Transition Presets";
        int selectedPresetIndex;

        TransitionProfile[] presets;
        string[] presetNames;

        bool previewInProgress, previewPendingSound;
        float previewStartTime;
        TransitionAnimator animator;

        void OnEnable() {
            progress = serializedObject.FindProperty("progress");

            autoPlay = serializedObject.FindProperty("autoPlay");
            playDelay = serializedObject.FindProperty("playDelay");
            useUnscaledTime = serializedObject.FindProperty("useUnscaledTime");
            autoDestroy = serializedObject.FindProperty("autoDestroy");
            destroyDelay = serializedObject.FindProperty("destroyDelay");
            destroyAllTransitions = serializedObject.FindProperty("destroyAllTransitions");
            onTransitionEnd = serializedObject.FindProperty("onTransitionEnd");

            autoFollow = serializedObject.FindProperty("autoFollow");
            followTarget = serializedObject.FindProperty("followTarget");
            followPositionOffset = serializedObject.FindProperty("followPositionOffset");

            loadSceneAtEnd = serializedObject.FindProperty("loadSceneAtEnd");
            sceneNameToLoad = serializedObject.FindProperty("sceneNameToLoad");
            sceneLoadMode = serializedObject.FindProperty("sceneLoadMode");

            fadeToCamera = serializedObject.FindProperty("fadeToCamera");
            mainCamera = serializedObject.FindProperty("mainCamera");
            secondCamera = serializedObject.FindProperty("secondCamera");
            switchActiveCamera = serializedObject.FindProperty("switchActiveCamera");

            renderMode = serializedObject.FindProperty("renderMode");
            customScreen = serializedObject.FindProperty("customScreen");
            sortingOrder = serializedObject.FindProperty("sortingOrder");

            animator = (TransitionAnimator)target;

            profile = serializedObject.FindProperty("profile");
            TransitionProfile tp = (TransitionProfile)profile.objectReferenceValue;
            if (tp != null) tp.displayingAnimator = animator;

            LoadPresets();
        }

        public override void OnInspectorGUI() {

            if (boxStyle == null) {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.padding = new RectOffset(15, 10, 5, 5);
            }

            serializedObject.Update();


            if (profile.objectReferenceValue != null) {
                EditorGUILayout.PropertyField(progress);
                if (GUILayout.Button("Preview")) {
                    StartPreview();
                }

                EditorGUILayout.PropertyField(loadSceneAtEnd, new GUIContent("Load Scene"));
                if (loadSceneAtEnd.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(sceneNameToLoad, new GUIContent("Scene To Load"));
                    EditorGUILayout.PropertyField(sceneLoadMode, new GUIContent("Scene Load Mode"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(fadeToCamera);
                if (fadeToCamera.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(mainCamera);
                    EditorGUILayout.PropertyField(secondCamera);
                    EditorGUILayout.PropertyField(switchActiveCamera);
                    if (animator.mainCameraCapture != null) {
                        EditorGUILayout.BeginHorizontal();
                        DrawPreviewImage(animator.mainCameraCapture);
                        DrawPreviewImage(animator.secondCameraCapture);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                } else {
                    if (animator.profile != null && animator.profile.type.RequiresFirstCameraCapture()) {
                        EditorGUILayout.PropertyField(mainCamera);
                    }
                }

                EditorGUILayout.PropertyField(autoPlay);
                if (autoPlay.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(playDelay);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(autoDestroy);
                    if (autoDestroy.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(destroyDelay);
                        EditorGUILayout.PropertyField(destroyAllTransitions);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(onTransitionEnd);
                }
                EditorGUILayout.PropertyField(useUnscaledTime);

                EditorGUILayout.PropertyField(autoFollow);
                if (autoFollow.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(followTarget);
                    EditorGUILayout.PropertyField(followPositionOffset);
                    EditorGUILayout.PropertyField(mainCamera);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(renderMode);
                switch ((RenderMode)renderMode.intValue) {
                    case RenderMode.VR:
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(mainCamera);
                        EditorGUI.indentLevel--;
                        break;
                    case RenderMode.InsideUI:
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(customScreen, new GUIContent("Raw Image Container", "Optionally render the effect in a Raw Image of your UI."));
                        EditorGUI.indentLevel--;
                        break;
                }
                EditorGUILayout.PropertyField(sortingOrder);

            }

            EditorGUI.BeginChangeCheck();
            selectedPresetIndex = EditorGUILayout.Popup(new GUIContent("Preset", "Select one of the sample presets found in the Transition Presets folder"), selectedPresetIndex, presetNames);
            if (EditorGUI.EndChangeCheck()) {
                AssignSelectedPreset();
            }

            EditorGUILayout.PropertyField(profile);

            if (profile.objectReferenceValue != null) {
                if (cachedProfile != profile.objectReferenceValue) {
                    cachedProfile = null;
                }
                if (cachedProfile == null) {
                    cachedProfile = (TransitionProfile)profile.objectReferenceValue;
                    cachedProfileEditor = CreateEditor(profile.objectReferenceValue);
                }

                // Drawing the profile editor
                EditorGUILayout.BeginVertical(boxStyle);
                cachedProfileEditor.OnInspectorGUI();

                if (GUILayout.Button("Save As New Profile")) {
                    ExportTransitionProfile();
                }
                EditorGUILayout.EndVertical();
            } else {
                EditorGUILayout.HelpBox("Create or assign a transition profile.", MessageType.Info);
                if (GUILayout.Button("New Transition Profile")) {
                    CreateTransitionProfile();
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (previewInProgress) {
                PerformPreview();
                Repaint();
                EditorUtility.SetDirty(target);
            }
        }

        void DrawPreviewImage(Texture texture) {
            if (texture == null) return;
            float w = Mathf.Min(EditorGUIUtility.currentViewWidth - 24, texture.width);
            w = Mathf.Max(8, w / 2f);
            float aspect = (float)texture.height / texture.width;
            float h = w * aspect;
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            GUILayout.Label(texture, boxStyle, GUILayout.Width(w), GUILayout.Height(h));
        }

        void CreateTransitionProfile() {

            var fp = CreateInstance<TransitionProfile>();
            fp.name = "New Transition Plus Profile";

            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path)) {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }

            string fullPath = path + "/" + fp.name + ".asset";
            fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(fp, fullPath);
            AssetDatabase.SaveAssets();
            profile.objectReferenceValue = fp;
            EditorGUIUtility.PingObject(fp);
        }

        void ExportTransitionProfile() {
            var fp = (TransitionProfile)profile.objectReferenceValue;
            var newProfile = Instantiate(fp);

            string path = AssetDatabase.GetAssetPath(fp);
            string fullPath = path;
            if (string.IsNullOrEmpty(path)) {
                path = "Assets";
                foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                    path = AssetDatabase.GetAssetPath(obj);
                    if (File.Exists(path)) {
                        path = Path.GetDirectoryName(path);
                    }
                    break;
                }
                fullPath = path + "/" + fp.name + ".asset";
            }
            fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
            AssetDatabase.CreateAsset(newProfile, fullPath);
            AssetDatabase.SaveAssets();
            profile.objectReferenceValue = newProfile;
            EditorGUIUtility.PingObject(fp);

        }

        void LoadPresets() {

            string[] res = Directory.GetDirectories(Application.dataPath, "*" + MASTER_FOLDER_NAME + "*", SearchOption.AllDirectories);
            string path = null;
            for (int k = 0; k < res.Length; k++) {
                if (res[k].Contains(MASTER_FOLDER_NAME)) {
                    path = res[k];
                    break;
                }
            }
            if (path == null) {
                return;
            }

            List<TransitionProfile> validPresets = new List<TransitionProfile>
            {
                null
            };
            List<string> validPresetsNames = new List<string>
            {
                string.Empty
            };

            string[] profiles = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            if (profiles != null) {
                for (int l = 0; l < profiles.Length; l++) {
                    string profilePath = profiles[l];
                    int i = profilePath.IndexOf("/Assets");
                    if (i < 0) continue;
                    profilePath = profilePath.Substring(i + 1);
                    TransitionProfile profile = AssetDatabase.LoadAssetAtPath<TransitionProfile>(profilePath);
                    if (profile != null) {
                        validPresets.Add(profile);
                        validPresetsNames.Add(ObjectNames.NicifyVariableName(profile.name));
                    }
                }

                presets = validPresets.ToArray();
                presetNames = validPresetsNames.ToArray();
            }
        }

        void AssignSelectedPreset() {
            TransitionProfile selectedPreset = presets[selectedPresetIndex];
            if (selectedPreset != null) {
                TransitionProfile animatorProfile = Instantiate(selectedPreset);
                animatorProfile.name = selectedPreset.name + " (Customized)";
                profile.objectReferenceValue = animatorProfile;
                StartPreview();
            }
        }

        void StartPreview() {
            previewInProgress = true;
            previewStartTime = animator.GetTime();
            previewPendingSound = true;
        }

        void PerformPreview() {
            TransitionProfile profile = animator.profile;
            if (profile == null) return;

            float elapsedTime = animator.GetTime() - previewStartTime;
            float t = elapsedTime / profile.duration;

            if (t >= 1.25f) { // give a bit of pause at the end
                t = 0;
                previewInProgress = false;
            }

            if (previewPendingSound && t >= profile.soundDelay && profile.sound != null) {
                previewPendingSound = false;
                PlayClip(profile.sound);
            }

            animator.SetProgress(t);
        }

        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false) {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );
            method.Invoke(
                null,
                new object[] { clip, startSample, loop }
            );
        }


    }

}