using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;

public partial class MoveCreator
{
    // -- Fields --
    
    Moveset currentMoveset;
    string movesetName;

    // Dictionary of known moves by type
    readonly Dictionary<string, List<MoveData>> existingMoves = new ();

    // The scroll position of the scroll view
    Vector2 scrollViewPos;

    // -- End --

    void CreatingMovesetMenu()
    {
        DrawBackButton();
        
        EditorGUILayout.LabelField("Moveset Creator", EditorStyles.boldLabel);

        currentMoveset = (Moveset) EditorGUILayout.ObjectField("Moveset to Edit", currentMoveset, typeof(Moveset), false);
        
        if (currentMoveset == null)
        {
            EditorGUILayout.HelpBox("Select a Moveset or create a new one.", MessageType.Warning);
            
            // Set the moveset name
            movesetName = EditorGUILayout.TextField("Moveset Name", movesetName);
            
            string assetName = string.IsNullOrEmpty(movesetName) ? "New Moveset" : movesetName;
            
            if (Button($"Create {assetName}"))
            {
                if (string.IsNullOrEmpty(assetName) || assetName == "New Moveset")
                {

                    // If the asset name is empty, a new moveset called "New Moveset" will be created.
                    // If there already exists a moveset called "New Moveset", then the old one will be overwritten.
                    const string warning = "Warning";
                    string message = WarningMessage(assetName);

                    if (EditorUtility.DisplayDialog(warning, message, "Proceed", "Cancel")) 
                        CreateMoveset();
                }
                else { CreateMoveset(); }
            }
            return;
        }

        scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos); // Begin ScrollView
        
        Space(10);
        
        // Show the inspector for the moveset
        Editor inspector = Editor.CreateEditor(currentMoveset);
        inspector.OnInspectorGUI();

        Space(10);

        // Now, let's list the pre-made moves we can add
        EditorGUILayout.LabelField(existingMovesContent, EditorStyles.boldLabel);

        // Remove deleted MoveData:
        List<string> keys = new (existingMoves.Keys);

        foreach (string key in keys)
        {
            // Remove null MoveData:
            existingMoves[key].RemoveAll(MoveData => MoveData == null);

            // If no MoveData remaining for a key, remove the key:
            if (existingMoves[key].Count == 0) existingMoves.Remove(key);
        }
        
        using (new VerticalScope("box"))
        {
            foreach (KeyValuePair<string, List<MoveData>> entry in existingMoves)
            {
                Space(5);
                
                using (new HorizontalScope("box"))
                {
                    EditorGUILayout.LabelField(entry.Key, Width(170));

                    using (new VerticalScope("box"))
                    {
                        foreach (MoveData moveData in entry.Value)
                        {
                            Space(2.5f);
                            
                            if (Button(moveData.name, Height(30), MaxWidth(300)))
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
                        }
                    }
                }
            }
        }

        EditorGUILayout.EndScrollView(); // End ScrollView
    }

    #region Utility
    void ResetMovesetCreator()
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
    
    void CreateMoveset()
    {
        currentMoveset = CreateInstance<Moveset>();
        const string path        = "Assets/_Project/Runtime/Scripts/Player/Attacking/Scriptable Objects/Movesets";
        const string defaultName = "New Moveset";
        string       assetName   = string.IsNullOrEmpty(movesetName) ? defaultName : movesetName;
        
        AssetDatabase.CreateAsset(currentMoveset, $"{path}/{assetName}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created moveset \"{currentMoveset.name}\".");
        Selection.activeObject = currentMoveset;
        EditorGUIUtility.PingObject(currentMoveset);

        createdSuccessfully = true;
    }

    void LoadExistingMoves()
    {
        // We'll use this to only consider .asset files in the Assets/MoveData directory
        string[] guids = AssetDatabase.FindAssets
        ("t:MoveData", new[]
         { "Assets/_Project/Runtime/Scripts/Player/Attacking/Scriptable Objects/Moves" });

        foreach (string guid in guids)
        {
            string   path     = AssetDatabase.GUIDToAssetPath(guid);
            MoveData moveData = AssetDatabase.LoadAssetAtPath<MoveData>(path);

            if (moveData != null) existingMoves[moveData.type.ToString()].Add(moveData);
        }
    }
    #endregion
}