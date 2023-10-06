#region
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Lumina.Essentials.Editor.UI;
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
                    if (Button("Intro", Height(30))) SceneManagerExtended.LoadScene(0);
                    if (Button("Main Menu", Height(30))) SceneManagerExtended.LoadScene(1);
                    if (Button("Character Selection", Height(30))) SceneManagerExtended.LoadScene(2);
                    if (Button("Game", Height(30))) SceneManagerExtended.LoadScene(3);
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

                const string introScenePath = "Assets/_Project/Runtime/Scenes/Development/Intro.unity";
                const string mainMenuPath   = "Assets/_Project/Runtime/Scenes/Development/MainMenu.unity";
                const string charSelectPath = "Assets/_Project/Runtime/Scenes/Development/CharacterSelect.unity";
                const string gameScenePath  = "Assets/_Project/Runtime/Scenes/Development/Game.unity";
                
                if (!Application.isPlaying)
                {
                    if (Button("Intro", Height(30))) OpenScene(introScenePath);
                    if (Button("Main Menu", Height(30))) OpenScene(mainMenuPath);
                    if (Button("Character Selection", Height(30))) OpenScene(charSelectPath);
                    if (Button("Game", Height(30))) OpenScene(gameScenePath);
                }
                else { Label("Cannot open scenes in play mode.", EditorStyles.centeredGreyMiniLabel); }
            }
            // -- End --
        }

        Space(10);
        
        using (new VerticalScope("box"))
        {
            // -- Open Other Debugging Windows --
            windowsFoldout = EditorGUILayout.Foldout(windowsFoldout, "Debugging Windows", true, EditorStyles.foldoutHeader);

            if (windowsFoldout)
            {
                if (Button("State Debugger")) FGDebuggerWindow.Open();
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

    static void OpenScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath);
        Debug.LogWarning("Loaded a scene using the debug menu! \nThe scene might not behave as expected.");
    }
    #endregion
}
}
