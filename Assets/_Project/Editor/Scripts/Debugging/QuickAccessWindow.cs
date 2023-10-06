#region
using System;
using System.Collections.Generic;
using Lumina.Essentials.Editor.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GUILayout;
#endregion

namespace Lumina.Debugging
{
public class QuickAccessWindow : EditorWindow
{
    static Action activeMenu;
    static bool windowsFoldout = true;
    static bool optionsFoldout = true;
    static bool addedScenesFoldout;
    static bool manageScenesFoldout = true;
    
    readonly static List<string> addedScenes = new ();

    [MenuItem("Tools/Debugging/Quick Access")]
    static void ShowWindow()
    {
        var window = GetWindow<QuickAccessWindow>();
        window.titleContent = new ("Quick Access");
        window.minSize      = new (350, 200);
        window.maxSize      = window.minSize;
        window.Show();
    }

    void OnGUI()
    {
        activeMenu();
    }

    void OnEnable()
    {
        Initialize();
        EditorApplication.playModeStateChanged += PlayModeState;

        return;
        void Initialize() => activeMenu = DefaultMenu;
    }

    void OnDisable()
    {
        Terminate();

        return;
        void Terminate()
        {
            // Clear the added scenes list.
            addedScenes.Clear();
            
            // Remove the play mode state changed event.
            EditorApplication.playModeStateChanged -= PlayModeState;
        }
    }

    void PlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            Repaint(); // Refresh the window
        }
    }

    #region GUI
    static void DefaultMenu()
    {
        DrawTopBanner();

        using (new VerticalScope("box"))
        {
            manageScenesFoldout = EditorGUILayout.Foldout(manageScenesFoldout, "Manage Scenes", true, EditorStyles.foldoutHeader);
            if (manageScenesFoldout)
            {
                DrawSceneButtons();
            }
        }
        
        DrawDebuggingWindowMenu();
        DrawDebugOptionsMenu();
    }

    static void DrawTopBanner()
    {
        using (new HorizontalScope())
        {
            FlexibleSpace();
            FlexibleSpace();

            bool isPlaying = Application.isPlaying;
            
            Label(isPlaying ? "Load Scene" : "Open Scene", EditorStyles.largeLabel);
            DrawBackButton();
        }
    }

    static void DrawSceneButtons()
    {
        bool isPlaying = Application.isPlaying;
        
        Label(isPlaying ? "Runtime" : "Editor", EditorStyles.boldLabel);
        using (new VerticalScope("box"))
        {
            if (isPlaying) DrawSceneLoadButtons(LoadScene);
            else DrawSceneLoadButtons(OpenScene);
        }
        
        Space(10);
        
        using (new VerticalScope("box"))
        {
            Label("Custom Scenes", EditorStyles.boldLabel);
            
            // Button to add a custom scene
            if (Button("Add Scene", Height(25)))
            {
                // Open Windows Explorer to select a scene
                string path = EditorUtility.OpenFilePanel("Select a scene", Application.dataPath, "unity");

                // If the path is not empty, add a new button to the menu
                if (!string.IsNullOrEmpty(path))
                {
                    // Add the button
                    addedScenes.Add(path);
                    addedScenesFoldout = true;
                }
            }

            addedScenesFoldout = EditorGUILayout.Foldout(addedScenesFoldout, "Added Scenes", true, EditorStyles.foldoutHeader);

            if (addedScenesFoldout && addedScenes.Count == 0)
            {
                // Warning that there are no added scenes.
                EditorGUILayout.HelpBox("No scenes have been added.", MessageType.Warning, true);
            }

            // Add a button for each added scene
            foreach (string scenePath in addedScenes)
            {
                // derive sceneName from path similarly to how you do it above
                string sceneName = scenePath[(scenePath.LastIndexOf('/') + 1)..];
                sceneName = sceneName[..^6];

                if (addedScenesFoldout)
                {
                    if (Button(sceneName, Height(30))) EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                }
            }
        }
    }

    static void DrawSceneLoadButtons(Action<int> sceneAction)
    {
        if (Button("Intro", Height(30))) sceneAction(0);
        if (Button("Main Menu", Height(30))) sceneAction(1);
        if (Button("Character Select", Height(30))) sceneAction(2);
        if (Button("Game", Height(30))) sceneAction(3);
    }

    static void DrawDebuggingWindowMenu()
    {
        Space(10);

        using (new VerticalScope("box"))
        {
            windowsFoldout = EditorGUILayout.Foldout(windowsFoldout, "Editor Windows", true, EditorStyles.foldoutHeader);

            if (windowsFoldout)
            {
                CreateButtonWithAction("State Debugger", FGDebuggerWindow.Open);
                CreateButtonWithAction("Moveset Creator", CharacterMovesetCreator.Open);
                CreateButtonWithAction("Lumina's Essentials", UtilityPanel.OpenUtilityPanel);
            }
        }

        Space(10);
    }

    static void DrawDebugOptionsMenu()
    {
        using (new VerticalScope("box"))
        {
            optionsFoldout = EditorGUILayout.Foldout(optionsFoldout, "Debug Options", true, EditorStyles.foldoutHeader);

            if (optionsFoldout) EditorSettings.enterPlayModeOptionsEnabled = EditorGUILayout.Toggle("Enter Playmode Options", EditorSettings.enterPlayModeOptionsEnabled);
        }
    }
    #endregion

    #region Utility
    static void DrawBackButton()
    {
        using (new HorizontalScope())
        {
            FlexibleSpace();

            if (Button("Back"))
            {
                Debug.LogWarning("Back button is not yet implemented.");
                
                // -- End --
                activeMenu = DefaultMenu;
            }
        }
    }

    static void CreateButtonWithAction(string buttonText, Action action)
    {
        if (Button(buttonText, Height(25))) action();
    }

    static void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        Debug.LogWarning("Loaded a scene using the debug menu! \nThe scene might not behave as expected.");
    }

    static void OpenScene(int sceneIndex)
    {
        // Get the scene path by the build index.
        string path = SceneUtility.GetScenePathByBuildIndex(sceneIndex);

        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        Debug.LogWarning("Loaded a scene using the debug menu! \nThe scene might not behave as expected.");
    }
    #endregion
}
}
