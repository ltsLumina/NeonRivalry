using System.Collections.Generic;
using UnityEditor;
using static UnityEngine.GUILayout;

public partial class MoveCreator
{
    // -- Menus --
    
    // Define list to hold moves for currently created moveset
    Moveset currentMoveset;

    // Define for specifying moveset name
    string movesetName;
    
    // List of all already created moves
    List<MoveData> existingMoves = new ();

    // Dictionary of known moves by type
    Dictionary<string, List<MoveData>> existingMovesByType = new ();

    // -- End --

    void CreatingMovesetMenu()
    {
        DrawBackButton();

        Label("Creating Moveset", EditorStyles.boldLabel);

        // Load
        EditorGUILayout.BeginHorizontal();
        currentMoveset = (Moveset) EditorGUILayout.ObjectField(currentMoveset, typeof(Moveset), false);

        if (Button("New Moveset"))
        {
            currentMoveset = CreateInstance<Moveset>();

            // Save the scriptable object.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path)) { path = "Assets"; }

            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/New Moveset.asset");
            AssetDatabase.CreateAsset(currentMoveset, assetPathAndName);
            AssetDatabase.SaveAssets();

            Selection.activeObject = currentMoveset;
        }

        EditorGUILayout.EndHorizontal();

        if (currentMoveset == null)
        {
            EditorGUILayout.HelpBox("Please select a Moveset to edit, or create a new Moveset.", MessageType.Info);
            return;
        }

        // Edit
        EditorGUI.BeginChangeCheck();
        Editor.CreateEditor(currentMoveset).OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(currentMoveset);
            AssetDatabase.SaveAssets();
        }

        // Known Moves 
        EditorGUILayout.LabelField("Known Moves:", EditorStyles.boldLabel);

        foreach (var move in existingMoves)
        {
            if (Button($"Add {move}"))
            {
                // add move to the corresponding category list in the moveset
                switch (move.type)
                {
                    case MoveData.Type.Punch:
                        currentMoveset.punchMoves.Add(move);
                        break;

                    case MoveData.Type.Kick:
                        currentMoveset.kickMoves.Add(move);
                        break;

                    case MoveData.Type.Slash:
                        currentMoveset.slashMoves.Add(move);
                        break;

                    case MoveData.Type.Unique:
                        currentMoveset.uniqueMoves.Add(move);
                        break;
                }

                EditorUtility.SetDirty(currentMoveset);
                AssetDatabase.SaveAssets();
            }
        }

        // Button to create the moveset
        //CreateMoveset();
    }
    
    /*void CreateMoveset()
    {
        if (Button(new GUIContent("Create Moveset", "Creates the moveset.")))
        {
            FlexibleSpace();

            var moveset = ScriptableObject.CreateInstance<Moveset>();

            const string path        = "Assets/Movesets";
            const string defaultName = "New Unnamed Moveset";
            string       assetName   = string.IsNullOrEmpty(movesetName) ? defaultName : movesetName;

            AssetDatabase.CreateAsset(moveset, $"{path}/{assetName}.asset");

            foreach (MoveData move in currentMoveset)
            {
                switch (move.type)
                {
                    case MoveData.Type.Punch:
                        moveset.punchMoves.Add(move);
                        break;

                    case MoveData.Type.Kick:
                        moveset.kickMoves.Add(move);
                        break;

                    case MoveData.Type.Slash:
                        moveset.slashMoves.Add(move);
                        break;

                    case MoveData.Type.Unique:
                        moveset.uniqueMoves.Add(move);
                        break;
                }

                AssetDatabase.AddObjectToAsset(move, moveset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            activeMenu = DefaultMenu;

            Debug.Log($"Created moveset {moveset.name}.");
            Selection.activeObject = moveset;
            EditorGUIUtility.PingObject(moveset);

            createdSuccessfully = true;
        }
    }*/
}