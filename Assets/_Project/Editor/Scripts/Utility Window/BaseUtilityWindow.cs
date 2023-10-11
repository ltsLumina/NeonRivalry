#region
using System;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;
#endregion

public class BaseUtilityWindow : EditorWindow
{
    // -- Menus --

    internal static BaseUtilityWindow window;
    readonly static Vector2 winSize = new (475, 615);

    public static Action activeMenu;
    public static bool createdSuccessfully;

    [MenuItem("Tools/Character Creation/Utility Window")]
    public static void Open()
    {
        window              = GetWindow<BaseUtilityWindow>();
        window.titleContent = new ("Utility Window");
        window.minSize      = new (winSize.x, winSize.y);
        window.maxSize      = window.minSize;
        window.Show();
    }

    void OnEnable()
    {
        window = GetWindow<BaseUtilityWindow>();
        
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
            Label("Manage Moves, Movesets, or State Data", EditorStyles.boldLabel);
        }

        using (new HorizontalScope("box"))
        {
            if (Button("Manage Moves", ExpandWidth(true)))
            {
                MoveCreator.showAttributes = true;
                MoveCreator.showResources  = true;
                MoveCreator.showProperties = true;
                
                createdSuccessfully = false;
                window.titleContent = new ("Managing Moves...");
                activeMenu          = MoveCreator.ManageMoveMenu;

            }
            
            if (Button("Manage Movesets", ExpandWidth(true)))
            {
                createdSuccessfully = false;
                activeMenu          = MovesetCreator.ManageMovesetMenu;
                window.titleContent = new ("Managing Movesets...");
            }
            
            if (Button("Manage State Data", ExpandWidth(true)))
            {
                createdSuccessfully = false;
                activeMenu          = StateDataManager.ManageStateDataMenu;
                window.titleContent = new ("Managing State Data...");
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
                Label("1. Click \"Manage Movesets\" or \"Mange Moves\".");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("2. Either select an existing Move or Moveset to edit, or create a new one.");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("3. To create a new move or moveset, click \"Create Moveset\" or \"Create Move\"");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("4. Fill in the fields.");
                FlexibleSpace();
            }
                
            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("5. Done! The ScriptableObject will be created.");
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
