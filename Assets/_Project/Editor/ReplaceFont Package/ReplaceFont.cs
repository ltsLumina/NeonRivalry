#region
using System;
using System.Collections.Generic;
using System.Linq;
using Editors;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#endregion

namespace Plugins
{
public static class ReplaceFont
{
    [MenuItem("Tools/Project/Replace Font")] static void ReplaceFontMenuItem() => ReplaceFontEditorWindow.ShowWindow();

    public static void ReplaceFontInScene(Font font)
    {
        if (font == null)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Font is null", "Ok");
            return;
        }

        List<Component> components   = null;
        Scene           currentScene = SceneManager.GetActiveScene();

        Text[] textComponents = Resources.FindObjectsOfTypeAll<Text>();

        Undo.SetCurrentGroupName("Replace all legacy text fonts");

        foreach (Text component in textComponents)
        {
            if (component.gameObject.scene != currentScene) continue;

            Undo.RecordObject(component, "");
            component.font = font;
            if (components == null) components = new ();
            components.Add(component);
            Debug.Log($"Replaced: {component.name}", component);
        }

        if (components == null) Debug.LogError("Can't find any text components on current scene");

        Undo.IncrementCurrentGroup();
    }

    public static void ReplaceFontInScene(TMP_FontAsset font)
    {
        if (font == null)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Font is null", "Ok");
            return;
        }

        List<Component> components   = null;
        Scene           currentScene = SceneManager.GetActiveScene();

        TextMeshProUGUI[] textComponents = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        Undo.SetCurrentGroupName("Replace all TMP fonts");

        foreach (TextMeshProUGUI component in textComponents)
        {
            if (component.gameObject.scene != currentScene) continue;

            Undo.RecordObject(component, "");

            component.font =   font;
            components     ??= new ();
            components.Add(component);
            Debug.Log($"Replaced: {component.name}", component);
        }

        if (components == null) Debug.LogError("Can't find any TMP components on current scene");

        Undo.IncrementCurrentGroup();
    }

    public static void ReplaceFontPrefab(Font font)
    {
        if (font == null)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Font is null", "Ok");
            return;
        }

        string[] prefabsPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)).ToArray();

        List<Component> components = null;

        Undo.SetCurrentGroupName("Replace all legacy text fonts");

        foreach (string path in prefabsPaths)
        {
            if (path.Contains("Packages")) continue;

            using (var prefabScope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                GameObject prefab = prefabScope.prefabContentsRoot;

                Text[] prefabTexts = prefab.GetComponentsInChildren<Text>(true);

                foreach (Text component in prefabTexts)
                {
                    Undo.RecordObject(component, "");
                    component.font = font;
                    if (components == null) components = new ();
                    components.Add(component);
                }

                Debug.Log($"Replaced: {prefab.name}", prefab);
            }
        }

        if (components == null) Debug.LogError("Can't find any text components in prefabs");

        Undo.IncrementCurrentGroup();
    }

    public static void ReplaceFontPrefab(TMP_FontAsset font)
    {
        if (font == null)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Font is null", "Ok");
            return;
        }

        string[] prefabsPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)).ToArray();

        List<Component> components = null;
        Undo.SetCurrentGroupName("Replace all TMP fonts");

        foreach (string path in prefabsPaths)
        {
            if (path.Contains("Packages")) continue;

            using (var prefabScope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                GameObject prefab = prefabScope.prefabContentsRoot;

                TextMeshProUGUI[] prefabTexts = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);

                foreach (TextMeshProUGUI component in prefabTexts)
                {
                    Undo.RecordObject(component, "");
                    component.font = font;
                    if (components == null) components = new ();
                    components.Add(component);
                }

                Debug.Log($"Replaced: {prefab.name}", prefab);
            }
        }

        if (components == null) Debug.LogError("Can't find any TMP components in prefabs");

        Undo.IncrementCurrentGroup();
    }

    public static void ReplaceFontSpecified(TMP_FontAsset font, List<TextMeshProUGUI> components)
    {
        if (font == null)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Font is null", "Ok");
            return;
        }

        if (components.Count == 0)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Components List is null", "Ok");
            return;
        }

        Undo.SetCurrentGroupName("Replace specified TMP fonts");

        bool oneComponentIsEmpty = false;

        foreach (TextMeshProUGUI component in components)
        {
            if (component == null)
            {
                oneComponentIsEmpty = true;
                continue;
            }

            Undo.RecordObject(component, "");
            component.font = font;
            Debug.Log($"Replaced: {component.gameObject.name}", component);
        }

        if (oneComponentIsEmpty)
            EditorUtility.DisplayDialog
                ("Replace font Result", "One of the components of the list is empty. Please manually check the list of text objects you have set. All other components have been replaced successfully.", "Ok");

        Undo.IncrementCurrentGroup();
    }

    public static void ReplaceFontSpecified(Font font, List<Text> components)
    {
        if (font == null)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Font is null", "Ok");
            return;
        }

        if (components.Count == 0)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Components List is null", "Ok");
            return;
        }

        Undo.SetCurrentGroupName("Replace specified legacy text fonts");

        bool oneComponentIsEmpty = false;

        foreach (Text component in components)
        {
            if (component == null)
            {
                oneComponentIsEmpty = true;
                continue;
            }

            Undo.RecordObject(component, "");
            component.font = font;
            Debug.Log($"Replaced: {component.gameObject.name}", component);
        }

        if (oneComponentIsEmpty)
            EditorUtility.DisplayDialog
                ("Replace font Result", "One of the components of the list is empty. Please manually check the list of text objects you have set. All other components have been replaced successfully.", "Ok");

        Undo.IncrementCurrentGroup();
    }

    public static void ReplaceFontInProject(Font font)
    {
        if (font == null)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Font is null", "Ok");
            return;
        }

        string[] scenesPaths  = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)).ToArray();
        string[] prefabsPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)).ToArray();

        List<Component> components = null;

        Undo.SetCurrentGroupName("Replace all legacy text fonts");

        foreach (string path in prefabsPaths)
        {
            if (path.Contains("Packages")) continue;

            using (var prefabScope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                GameObject prefab = prefabScope.prefabContentsRoot;

                Text[] prefabTexts = prefab.GetComponentsInChildren<Text>(true);

                foreach (Text component in prefabTexts)
                {
                    Undo.RecordObject(component, "");
                    component.font = font;
                    if (components == null) components = new ();
                    components.Add(component);
                }

                Debug.Log($"Replaced: {prefab.name}", prefab);
            }
        }

        string currentScene = SceneManager.GetActiveScene().path;
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Text[] textComponents = Object.FindObjectsOfType<Text>();

        foreach (string scene in scenesPaths)
        {
            EditorSceneManager.OpenScene(scene);

            foreach (Text component in textComponents)
            {
                Undo.RecordObject(component, "");
                component.font = font;
                if (components == null) components = new ();
                components.Add(component);
                Debug.Log($"Replaced: {component.name}", component);
            }

            EditorSceneManager.SaveOpenScenes();
        }

        EditorSceneManager.OpenScene(currentScene);

        if (components == null) Debug.LogError("Can't find any text components on all scenes");
        Undo.IncrementCurrentGroup();
    }

    public static void ReplaceFontInProject(TMP_FontAsset font)
    {
        if (font == null)
        {
            EditorUtility.DisplayDialog("Replace font Result", "Font is null", "Ok");
            return;
        }

        string[] scenesPaths  = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)).ToArray();
        string[] prefabsPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)).ToArray();

        List<Component> components = null;

        Undo.SetCurrentGroupName("Replace all TMP fonts");

        foreach (string path in prefabsPaths)
        {
            if (path.Contains("Packages")) continue;

            using (var prefabScope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                GameObject prefab = prefabScope.prefabContentsRoot;

                TextMeshProUGUI[] prefabTexts = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);

                foreach (TextMeshProUGUI component in prefabTexts)
                {
                    component.font = font;
                    if (components == null) components = new ();
                    components.Add(component);
                }

                Debug.Log($"Replaced: {prefab.name}", prefab);
            }
        }

        string currentScene = SceneManager.GetActiveScene().path;
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        foreach (string scene in scenesPaths)
        {
            EditorSceneManager.OpenScene(scene);

            TextMeshProUGUI[] textComponents = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

            foreach (TextMeshProUGUI component in textComponents)
            {
                component.font = font;
                if (components == null) components = new ();
                components.Add(component);
                Debug.Log($"Replaced: {component.name}", component);
            }

            EditorSceneManager.SaveOpenScenes();
        }

        EditorSceneManager.OpenScene(currentScene);

        if (components == null) Debug.LogError("Can't find any TMP components on all scenes");
        Undo.IncrementCurrentGroup();
    }
}
}
