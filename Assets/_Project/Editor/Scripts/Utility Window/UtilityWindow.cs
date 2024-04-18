#region
using System;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;
#endregion

public class UtilityWindow : EditorWindow
{
    // -- Menus --
    
    internal static UtilityWindow window;
    readonly static Vector2 winSize = new (475, 615);

    public static Action activeMenu;
    public static bool createdSuccessfully;

    [MenuItem("Tools/Character Menu")]
    public static void Open()
    {
        if (window != null)
        {
            window.Focus();
        }
        else
        {
            window              = GetWindow<UtilityWindow>(true);
            window.titleContent = new ("Utility Window");
            window.minSize      = new (winSize.x, winSize.y / 2);
            window.Show();
        }
    }

    void OnEnable()
    {
        window = GetWindow<UtilityWindow>();
        if (Resources.FindObjectsOfTypeAll<UtilityWindow>().Length > 1) Close();

        createdSuccessfully = false;

        // Initialize the dictionary with types
        MovesetCreator.InitializeExistingMoves();

        MovesetCreator.LoadExistingMoves();

        activeMenu ??= DefaultMenu;
    }

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
                createdSuccessfully = false;
                
                activeMenu = MoveCreator.ManageMoveMenu;
                window.titleContent = new ("Managing Moves...");
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
                Label("1. Click \"Manage Moves\" or \"Mange Movesets\".");
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
                Label("3. To create a new move or moveset, click \"Create Move\" or \"Create Moveset\"");
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
    public static bool DrawBackButton()
    {
        bool isButtonPressed = false;

        using (new HorizontalScope())
        {
            FlexibleSpace();

            if (Button("Back"))
            {
                window.titleContent = new ("Utility Window");
                
                // -- Move Creator --
                MoveCreator.ResetMoveCreator();
                MoveCreator.moveName = string.Empty;

                // -- Moveset Creator --
                MovesetCreator.ResetMovesetCreator();

                // -- End --
                activeMenu = DefaultMenu;

                isButtonPressed = true;
            }
        }

        return isButtonPressed;
    }

    public static string GetFilePathByWindowsExplorer(string defaultFolder = "Moves")
    {
        const string folderPath = "Assets/_Project/Runtime/_Scripts/Player/Combat/Scriptable Objects/";
        
        string path = EditorUtility.SaveFolderPanel("Choose a folder to save the move in", folderPath, defaultFolder);
        
        // Replace the path with the relative path.
        path = path.Replace(Application.dataPath, "Assets");
        return path;
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
