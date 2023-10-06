#region
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using Lumina.Essentials.Editor.UI;
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
    
    [MenuItem("Tools/Debugging/Quick Access")]
    static void ShowWindow()
    {
        var window = GetWindow<QuickAccessWindow>();
        window.titleContent = new ("Quick Access");
        window.minSize = new (350, 200);
        window.maxSize = window.minSize;
        window.Show();
    }

    void OnGUI()
    {
        activeMenu();
    }

    void OnEnable()
    {
        Initialize();
        
        return;
        void Initialize() => activeMenu = DefaultMenu;
    }

    void OnDisable()
    {
        // return;
        // void Terminate()
        // {
        //     
        // }
    }

    #region GUI
    static void DefaultMenu()
    {
        using (new HorizontalScope())
        {
            FlexibleSpace();
            FlexibleSpace();
            
            Label("Open/Load Scene", EditorStyles.largeLabel);
        
            DrawBackButton();
        }
        
        // Draw the scene loading options.
        using (new HorizontalScope("box"))
        {
            Space(10);

            using (new VerticalScope("box"))
            {
                Label("Runtime", EditorStyles.boldLabel);

                if (Application.isPlaying)
                {
                    if (Button("Intro", Height(30))) LoadScene(0);
                    if (Button("Main Menu", Height(30))) LoadScene(1);
                    if (Button("Character Select", Height(30))) LoadScene(2);
                    if (Button("Game", Height(30))) LoadScene(3);
                }
                else
                {
                    Label("Cannot load scenes in edit mode.", EditorStyles.centeredGreyMiniLabel);
                }
            }
            
            Space(10);

            // -- Open/Load Scenes
            using (new VerticalScope("box"))
            {
                Label("Editor", EditorStyles.boldLabel);
                
                if (!Application.isPlaying)
                {
                    if (Button("Intro", Height(30))) OpenScene(0);
                    if (Button("Main Menu", Height(30))) OpenScene(1);
                    if (Button("Character Selection", Height(30))) OpenScene(2);
                    if (Button("Game", Height(30))) OpenScene(3);
                }
                else { Label("Cannot open scenes in play mode.", EditorStyles.centeredGreyMiniLabel); }
            }
            // -- End --
        }

        Space(10);
        
        using (new VerticalScope("box"))
        {
            // -- Open Other Debugging Windows --
            windowsFoldout = EditorGUILayout.Foldout(windowsFoldout, "Editor Windows", true, EditorStyles.foldoutHeader);

            if (windowsFoldout)
            {
                if (Button("State Debugger")) FGDebuggerWindow.Open();
                if (Button("Moveset Creator")) CharacterMovesetCreator.Open();
                if (Button("Lumina's Essentials")) UtilityPanel.OpenUtilityPanel();
            }
        }
        
        Space(10);

        using (new VerticalScope("box"))
        {
            // Foldout the options.
            optionsFoldout = EditorGUILayout.Foldout(optionsFoldout, "Debug Options", true, EditorStyles.foldoutHeader);

            if (optionsFoldout)
            {
                EditorSettings.enterPlayModeOptionsEnabled = EditorGUILayout.Toggle("Enter Playmode Options", EditorSettings.enterPlayModeOptionsEnabled);
            }
        }
    }
    
    #endregion

    #region Utility
    public static void DrawBackButton()
    {
        using (new HorizontalScope())
        {
            FlexibleSpace();

            if (Button("Back"))
            {
                // Reset back to the default values

                // -- End --
                activeMenu = DefaultMenu;
            }
        }
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
