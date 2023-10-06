#region
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.GUILayout;
#endregion

namespace Lumina.Debugging
{
public class QuickAccessWindow : EditorWindow
{
    static Action activeMenu;
    static bool foldout = true;
    
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

        return;
        void Terminate()
        {
            throw new NotImplementedException();
        }
    }

    #region GUI
    static void DefaultMenu()
    {
        using (new HorizontalScope())
        {
            FlexibleSpace();
            FlexibleSpace();
            
            Label("Load Scene", EditorStyles.largeLabel);
        
            DrawBackButton();
        }
        
        // Draw the scene loading options.
        using (new HorizontalScope("box"))
        {
            Space(10);

            using (new VerticalScope("box"))
            {
                Label("Runtime Scene Load", EditorStyles.boldLabel);

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
                Label("Editor Scene Load", EditorStyles.boldLabel);

                const string introScenePath = "Assets/_Project/Runtime/Scenes/Development/Intro.unity";
                const string mainMenuPath   = "Assets/_Project/Runtime/Scenes/Development/MainMenu.unity";
                const string charSelectPath = "Assets/_Project/Runtime/Scenes/Development/CharacterSelect.unity";
                const string gameScenePath  = "Assets/_Project/Runtime/Scenes/Development/Game.unity";
                
                if (!Application.isPlaying)
                {
                    //These are all false positives
                    if (Button("Intro", Height(30))) EditorSceneManager.OpenScene(introScenePath);
                    if (Button("Main Menu", Height(30))) EditorSceneManager.OpenScene(mainMenuPath);
                    if (Button("Character Selection", Height(30))) EditorSceneManager.OpenScene(charSelectPath);
                    if (Button("Game", Height(30))) EditorSceneManager.OpenScene(gameScenePath);
                }
                else { Label("Cannot open scenes in play mode.", EditorStyles.centeredGreyMiniLabel); }
            }
            // -- End --
        }

        Space(10);

        using (new VerticalScope("box"))
        {
            // -- Open Other Debugging Windows --
            foldout = EditorGUILayout.Foldout(foldout, "Debugging Windows", true, EditorStyles.foldoutHeader);

            if (foldout)
            {
                if (Button("State Debugger")) FGDebuggerWindow.Open();
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
    #endregion
}
}
