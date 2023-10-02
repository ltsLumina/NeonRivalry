using System.Collections.Generic;
using Lumina.Debugging;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;

public static class MovesetCreator
{
    // -- Fields --

    static Moveset currentMoveset;
    public static string movesetName;

    // Dictionary of known moves by type
    readonly static Dictionary<string, List<MoveData>> existingMoves = new ();

    // The scroll position of the scroll view
    static Vector2 scrollViewPos;

    // -- End --

    public static void CreatingMovesetMenu()
    {
        DrawMenuHeader();
        currentMoveset = GetMovesetToEdit();

        if (currentMoveset == null)
        {
            PromptCreateNewMoveset();
            return;
        }

        DisplayExistingMovesetOptions();
    }

    #region GUI
    static void DrawMenuHeader()
    {
        CharacterMovesetCreator.DrawBackButton();
        EditorGUILayout.LabelField("Moveset Creator", EditorStyles.boldLabel);
    }

    static Moveset GetMovesetToEdit() => (Moveset) EditorGUILayout.ObjectField("Moveset to Edit", currentMoveset, typeof(Moveset), false);

    static void PromptCreateNewMoveset()
    {
        EditorGUILayout.HelpBox("Select a Moveset or create a new one.", MessageType.Warning);

        movesetName = GetMovesetName();

        string assetName = GenerateAssetName(movesetName);

        if (Button($"Create {assetName}")) CreateMovesetWithConfirmation(assetName);
    }

    static string GetMovesetName() => EditorGUILayout.TextField("Moveset Name", movesetName);

    static string GenerateAssetName(string assetName) => string.IsNullOrEmpty(assetName) ? "New Moveset" : assetName;

    static void CreateMovesetWithConfirmation(string assetName)
    {
        if (string.IsNullOrEmpty(assetName) || assetName == "New Moveset") // Checking if new Moveset name should be confirmed
        {
            const string warning = "Warning";
            string       message = WarningMessage(assetName);

            if (EditorUtility.DisplayDialog(warning, message, "Proceed", "Cancel")) CreateMoveset();
        }
        else { CreateMoveset(); } // Create new Moveset without confirmation
    }

    static void DisplayExistingMovesetOptions()
    {
        scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos); // Begin ScrollView
        
        Space(10);
        
        DisplayMovesetInspector();
        
        Space(10);

        EditorGUILayout.LabelField(existingMovesContent, EditorStyles.boldLabel);
        RemoveDeletedMoveData();
        DisplayExistingMoves();
        
        // End ScrollView
        EndScrollView();
    }

    static void DisplayMovesetInspector()
    {
        var inspector = Editor.CreateEditor(currentMoveset);
        inspector.OnInspectorGUI();
    }

    static void RemoveDeletedMoveData()
    {
        List<string> keys = new (existingMoves.Keys);

        foreach (string key in keys)
        {
            // Remove null MoveData:
            existingMoves[key].RemoveAll(MoveData => MoveData == null);

            // If no MoveData remaining for a key, remove the key:
            if (existingMoves[key].Count == 0) existingMoves.Remove(key);
        }
    }

    static void DisplayExistingMoves()
    {
        using (new VerticalScope("box"))
        {
            foreach (KeyValuePair<string, List<MoveData>> entry in existingMoves)
            {
                Space(5);
                DisplayMoveCategory(entry);
            }
        }
    }

    static void DisplayMoveCategory(KeyValuePair<string, List<MoveData>> entry)
    {
        using (new HorizontalScope("box"))
        {
            EditorGUILayout.LabelField(entry.Key, Width(170));
            DisplayMovesInCategory(entry);
        }
    }

    static void DisplayMovesInCategory(KeyValuePair<string, List<MoveData>> entry)
    {
        using (new VerticalScope("box"))
        {
            foreach (MoveData moveData in entry.Value)
            {
                Space(2.5f);
                if (Button(moveData.name, Height(30), MaxWidth(300))) AddMoveToMoveset(moveData);
            }
        }
    }

    static void AddMoveToMoveset(MoveData moveData)
    {
        switch (moveData.type)
        {
            case MoveData.Type.Punch:
                currentMoveset.punchMoves.Add(moveData);
                break;

            case MoveData.Type.Kick:
                currentMoveset.kickMoves.Add(moveData);
                break;

            case MoveData.Type.Slash:
                currentMoveset.slashMoves.Add(moveData);
                break;

            case MoveData.Type.Unique:
                currentMoveset.uniqueMoves.Add(moveData);
                break;
        }
    }
    #endregion

    #region Utility
    public static void InitializeExistingMoves()
    {
        existingMoves["Punch"]  = new ();
        existingMoves["Kick"]   = new ();
        existingMoves["Slash"]  = new ();
        existingMoves["Unique"] = new ();
    }

    public static void ResetMovesetCreator()
    {
        currentMoveset = null;
        movesetName    = string.Empty;
    }

    /// <summary>
    /// Returns a warning message if the asset name is empty or the default name.
    /// </summary>
    /// <param name="assetName"> The name of the asset. </param>
    /// <param name="isMoveset"> Prints a message formatted for a moveset if true, and a move if false. </param>
    /// <returns></returns>
    static string WarningMessage(string assetName, bool isMoveset = true)
    {
        string message     = null;
        string assetType   = isMoveset ? "moveset" : "move";
        string defaultName = isMoveset ? "New Moveset" : "New Move";

        if (string.IsNullOrEmpty(assetName))
        {
            message = $"The name is empty, and a new {assetType} called \"{defaultName}\" will be created.\n" +
                      $"If there already exists a {assetType} called \"{defaultName}\", then the old one will be overwritten.";
        }
        else if (assetName == defaultName)
        {
            message = $"The name is the default name, and a new {assetType} called \"{defaultName}\" will be created.\n" +
                      $"If there already exists a {assetType} called \"{defaultName}\", then the old one will be overwritten.";
        }

        return message;
    }

    static void CreateMoveset()
    {
        currentMoveset = ScriptableObject.CreateInstance<Moveset>();
        const string path        = "Assets/_Project/Runtime/Scripts/Player/Combat/Scriptable Objects/Movesets";
        const string defaultName = "New Moveset";
        string       assetName   = string.IsNullOrEmpty(movesetName) ? defaultName : movesetName;
        
        try
        {
            AssetDatabase.CreateAsset(currentMoveset, $"{path}/{assetName}.asset");
        }
        catch (UnityException e)
        {
            Debug.LogError($"{FGDebugger.errorMessagePrefix} Failed to create asset. The path in the script is probably invalid.\n{e}");
            throw;
        }
        finally
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        Debug.Log($"Created moveset \"{currentMoveset.name}\".");
        Selection.activeObject = currentMoveset;
        EditorGUIUtility.PingObject(currentMoveset);

        CharacterMovesetCreator.createdSuccessfully = true;
    }

    public static void LoadExistingMoves()
    {
        // We'll use this to only consider .asset files in the Assets/MoveData directory
        string[] guids = AssetDatabase.FindAssets
        ("t:MoveData", new[]
         { "Assets/_Project/Runtime/Scripts/Player/Combat/Scriptable Objects/Moves" });

        foreach (string guid in guids)
        {
            string   path     = AssetDatabase.GUIDToAssetPath(guid);
            MoveData moveData = AssetDatabase.LoadAssetAtPath<MoveData>(path);

            if (moveData != null) existingMoves[moveData.type.ToString()].Add(moveData);
        }
    }
    #endregion
}