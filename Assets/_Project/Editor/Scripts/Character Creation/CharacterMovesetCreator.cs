#region
using System;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;
#endregion

public class CharacterMovesetCreator : EditorWindow
{
    // -- Menus --

    readonly static Vector2 winSize = new (475, 615);

    public static Action activeMenu;
    public static bool createdSuccessfully;

    [MenuItem("Tools/Character Creation/Moveset Creator")]
    public static void Open()
    {
        var window = GetWindow<CharacterMovesetCreator>();
        window.titleContent = new ("Moveset Creator");
        window.minSize      = new (winSize.x, winSize.y);
        window.maxSize      = window.minSize;
        window.Show();
    }

    void OnEnable()
    {
        createdSuccessfully = false;

        // Initialize the dictionary with types
        MovesetCreator.InitializeExistingMoves();

        MovesetCreator.LoadExistingMoves();

        activeMenu = DefaultMenu;
    }

    void OnDisable() => activeMenu = null;

    void OnGUI() => activeMenu();

    #region GUI
    public static void DefaultMenu()
    {
        DrawHeaderGUI();

        Space(10);

        DrawCreationTextGUI();

        DrawInstructionsGUI();
    }

    static void DrawHeaderGUI()
    {
        using (new HorizontalScope("box"))
        {
            Label("Create Moves / Movesets", EditorStyles.boldLabel);

            if (Button("Manage Movesets"))
            {
                createdSuccessfully = false;
                activeMenu          = MovesetCreator.CreatingMovesetMenu;
            }

            if (Button("Manage Moves"))
            {
                createdSuccessfully = false;

                MoveCreator.showAttributes = true;
                MoveCreator.showResources  = true;
                MoveCreator.showProperties = true;

                activeMenu = MoveCreator.CreatingMoveMenu;
            }
        }
    }

    static void DrawCreationTextGUI()
    {
        using (new HorizontalScope("box"))
        {
            FlexibleSpace();

            if (createdSuccessfully) Label(createdSuccessfullyContent, EditorStyles.boldLabel);

            FlexibleSpace();
        }
    }

    static void DrawInstructionsGUI()
    {
        using (new VerticalScope("box"))
        {
            Space(15);
            
            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("Instructions", EditorStyles.whiteLargeLabel);
                
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("1. Click \"Create Moveset\" or \"Create Move\".");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("2. Fill in the fields.");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("3. Click \"Create Moveset\" or \"Create Move\" again.");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("4. Done! The ScriptableObject will be created.");
                FlexibleSpace();
            }

            Space(10);
            
            // Horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            FlexibleSpace();
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
                // -- Move Creator --
                MoveCreator.ResetMoveCreator();

                // -- Moveset Creator --
                MovesetCreator.ResetMovesetCreator();

                // -- End --
                activeMenu = DefaultMenu;
            }
        }
    }

    /// <summary>
    ///     Returns a warning message if the asset name is empty or the default name.
    /// </summary>
    /// <param name="assetName"> The name of the asset. </param>
    /// <param name="isMoveset"> Prints a message formatted for a moveset if true, and a move if false. </param>
    /// <returns></returns>
    public static string WarningMessage(string assetName, bool isMoveset = true)
    {
        string message     = null;
        string assetType   = isMoveset ? "moveset" : "move";
        string defaultName = isMoveset ? "New Moveset" : "New Move";

        if (string.IsNullOrEmpty(assetName))
            message = $"The name is empty, and a new {assetType} called \"{defaultName}\" will be created.\n" +
                      $"If there already exists a {assetType} called \"{defaultName}\", then the old one will be overwritten.";
        else if (assetName == defaultName)
            message = $"The name is the default name, and a new {assetType} called \"{defaultName}\" will be created.\n" +
                      $"If there already exists a {assetType} called \"{defaultName}\", then the old one will be overwritten.";

        return message;
    }
    #endregion
}
