#region
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;
using Logger = Lumina.Debugging.Logger;
#endregion

public static class MoveCreator
{
    #region Move Data
    
    // -- Fields --
    
    static MoveData currentMove;
    internal static string moveName;

    static bool showAttributes;
    static bool showProperties;

    // -- Attributes --

    static MoveData.Type type;
    static MoveData.Direction direction;
    static MoveData.Guard guard;

    static string name;
    static string description;
    static int damage;

    // -- Resources --

    // -- Move Properties --
    static bool isOverhead;
    static bool isGuardBreak;
    #endregion

    #region GUI
    public static void ManageMoveMenu()
    {
        showAttributes = true;
        showProperties = true;
        
        DrawMenuHeader();
        currentMove = GetMoveToEdit();

        if (currentMove == null)
        {
            PromptCreateNewMove();
            return;
        }

        DisplayMoveEditor();
    }
    
    static void DrawMenuHeader()
    {
        UtilityWindow.DrawBackButton();
        EditorGUILayout.LabelField("Creating Move", EditorStyles.boldLabel);
    }

    static MoveData GetMoveToEdit() => (MoveData) EditorGUILayout.ObjectField("Move to Edit", currentMove, typeof(MoveData), false);

    static void PromptCreateNewMove()
    {
        EditorGUILayout.HelpBox("Select a move or create a new one.", MessageType.Warning);
        Space(10);
        
        moveName = GetMoveName();
        var label = new GUIContent($"Create {moveName}", "Creates the move. \nThe name of the move will be the name of the ScriptableObject.");

        bool isNullOrEmpty = string.IsNullOrEmpty(moveName);
        if (isNullOrEmpty) label.text = "Please choose a name.";

        Space(5);
        
        using (new EditorGUI.DisabledScope(isNullOrEmpty))
        {
            if (Button(label, Height(35)))
            {
                UtilityWindow.window.titleContent = new ("Creating New Move...");
                SwitchToMoveCreatorMenu();
            }
        }
    }

    internal static string GetMoveName() => EditorGUILayout.TextField("Move Name", moveName);

    internal static void SwitchToMoveCreatorMenu()
    {
        UtilityWindow.activeMenu = DrawCreatingMoveMenu;

        // Set the name of the ScriptableObject to the name of the move so that the user doesn't have to write it out again.
        name = moveName;
    }

    static void DisplayMoveEditor()
    {
        Space(10);
        var inspector = Editor.CreateEditor(currentMove);
        inspector.OnInspectorGUI();
    }

    static void DrawCreatingMoveMenu()
    {
        if (UtilityWindow.DrawBackButton()) UtilityWindow.window.titleContent = new ("Utility Window");

        Label("Creating Move", EditorStyles.boldLabel);

        DrawTypesGUI();

        Space(10);

        DrawAttributesGUI();

        Space(10);

        DrawPropertiesGUI();

        // Button to create the move
        CreateMove();
    }

    static void DrawTypesGUI()
    {
        using (new HorizontalScope("box"))
        {
            Label(typeContent, EditorStyles.boldLabel);

            // Dropdown (enum)
            type = (MoveData.Type) EditorGUILayout.EnumPopup(type);

            Label(directionContent, EditorStyles.boldLabel);

            // Dropdown (enum)
            direction = (MoveData.Direction) EditorGUILayout.EnumPopup(direction);

            Label(guardContent, EditorStyles.boldLabel);

            // Dropdown (enum)
            guard = (MoveData.Guard) EditorGUILayout.EnumPopup(guard);
        }
    }
    
    static void DrawAttributesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showAttributes ? "Attributes (click to hide)" : "Attributes (click to show)";

            showAttributes = EditorGUILayout.Foldout(showAttributes, label, true, EditorStyles.boldLabel);
            
            if (showAttributes)
            {
                // Initialize the name of the move with the name of the ScriptableObject
                if (string.IsNullOrEmpty(moveName)) name = "New Move";

                name        = EditorGUILayout.TextField(nameContent, name);
                description = EditorGUILayout.TextField(descriptionContent, description);
                damage      = EditorGUILayout.IntField(damageContent, damage);
            }
        }
    }
    
    static void DrawPropertiesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showProperties ? "Properties (click to hide)" : "Properties (click to show)";

            showProperties = EditorGUILayout.Foldout(showProperties, label, true, EditorStyles.boldLabel);

            if (showProperties)
            {
                isOverhead   = EditorGUILayout.Toggle(isOverheadContent, isOverhead);
                isGuardBreak = EditorGUILayout.Toggle(isGuardBreakContent, isGuardBreak);
            }
        }
    }
    #endregion

    #region Utility
    static void CreateMove()
    {
        var  label                = new GUIContent($"Create \"{name}\"", "Creates the move. \nThe name of the move will be the name of the ScriptableObject.");
        bool emptyName            = string.IsNullOrEmpty(name);
        if (emptyName) label.text = "Please choose a name.";

        using (new EditorGUI.DisabledScope(emptyName))
        {
            if (Button(label))
            {
                FlexibleSpace();

                currentMove = ScriptableObject.CreateInstance<MoveData>();

                string path      = UtilityWindow.GetFilePathByWindowsExplorer("Moves");
                string assetName = name;

                if (string.IsNullOrEmpty(path)) return;

                try
                {
                    AssetDatabase.CreateAsset(currentMove, $"{path}/{assetName}.asset");
                } 
                catch (UnityException e)
                {
                    Debug.LogError($"{Logger.ErrorMessagePrefix} Failed to create asset. The path in the script is probably invalid.\n{e.Message}");
                    throw;
                }
                finally
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                // Assign the currentMove's values
                currentMove.type      = type;
                currentMove.direction = direction;
                currentMove.guard     = guard;

                currentMove.name        = name;
                currentMove.description = description;
                currentMove.damage      = damage;
                
                currentMove.isOverhead   = isOverhead;
                currentMove.isGuardBreak = isGuardBreak;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                UtilityWindow.activeMenu = UtilityWindow.DefaultMenu;

                Debug.Log($"Created new move: \"{currentMove.name}\".");
                Selection.activeObject = currentMove;
                EditorGUIUtility.PingObject(currentMove);

                currentMove                       = null;
                UtilityWindow.createdSuccessfully = true;
            
                ResetMoveCreator();
            }
        }
    }

    public static void ResetMoveCreator()
    {
        currentMove = null;
        moveName    = string.Empty;

        // Reset all fields
        type      = MoveData.Type.Punch;
        direction = MoveData.Direction.Neutral;
        guard     = MoveData.Guard.High;

        name        = string.Empty;
        description = string.Empty;
        damage      = 1;
        
        isOverhead   = false;
        isGuardBreak = false;
    }
    #endregion
}
