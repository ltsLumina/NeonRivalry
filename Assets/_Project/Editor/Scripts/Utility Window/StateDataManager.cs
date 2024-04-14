#region
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GUILayout;
using Object = UnityEngine.Object;
#endregion

/// <summary>
/// Allows the user to edit the values of all state data assets in the project from once convenient location.
/// </summary>
public static class StateDataManager // NOTE: I haven't put as much effort into this as I did the other utility windows, so it's not as clean in terms of code.
{
    static bool stateDataReferencesFoldout;
    static Vector2 scrollPos;
    static bool foldout;

    readonly static List<StateData> StateDataList = new ()
    { new ("Move State", typeof(MoveStateData)),
      new ("Jump State", typeof(JumpStateData)),
      new ("Fall State", typeof(FallStateData)),
      new ("Attack State", typeof(AttackStateData)),
      new ("Airborne Attack State", typeof(AirborneAttackStateData)),

      // add more states here once they exist.
    };

    public static void ManageStateDataMenu() // Note: This method has not yet been refactored. Be aware that it is a mess.
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawMenuHeader();

        using (new VerticalScope("box"))
        {
            stateDataReferencesFoldout = EditorGUILayout.Foldout(stateDataReferencesFoldout, "State Data References", true);
        }

        // Display the state data references 
        using (new VerticalScope("box"))
        {
            // Display the state data references while the foldout is open
            foreach (StateData stateData in StateDataList.Where(_ => stateDataReferencesFoldout))
            {
                stateData.data = EditorGUILayout.ObjectField(stateData.name, stateData.data, stateData.type, false);
            }
        }

        // Display the state data inspectors
        foreach (StateData stateData in StateDataList)
        {
            Space(10);

            GUIContent foldoutContent = new (stateData.name, "Inspect the state data for each state. \nYou can click on this to open/close the foldout.");

            using (new EditorGUILayout.VerticalScope("box"))
            {
                stateData.foldout = EditorGUILayout.Foldout(stateData.foldout, foldoutContent, true, EditorStyles.boldLabel);

                if (!stateData.foldout) continue;
                
                var inspector = Editor.CreateEditor(stateData.data);
                if (inspector != null)
                {
                    inspector.OnInspectorGUI();

                    switch (stateData.type)
                    {
                        // Customize the inspector of the attack state data to show the attack moves from the moveset.
                        case not null when stateData.type == typeof(AttackStateData):
                        {
                            var attackStateData = (AttackStateData) stateData.data;

                            Space(10);
                            
                            var label = new GUIContent("Moves", "Displays the moves from the moveset.");
                            foldout = EditorGUILayout.Foldout(foldout, label, true);
                            
                            if (foldout)
                            {
                                using (new EditorGUI.DisabledScope(true))
                                {
                                    foreach (List<MoveData> attackMoves in attackStateData.Moveset.Moves)
                                    {
                                        foreach (MoveData move in attackMoves)
                                        {
                                            var moveName = new GUIContent(move.name, "You can change which move is selected from the \"Manage Movesets\" window.");
                                            EditorGUILayout.ObjectField(moveName, move, typeof(MoveData), false);
                                        }
                                    }
                                }
                            }

                            break;
                        }
                        
                        // Customize the inspector of the airborne attack state data to show the airborne attack move from the moveset.
                        case not null when stateData.type == typeof(AirborneAttackStateData):
                        {
                            var airborneAttackStateData = (AirborneAttackStateData) stateData.data;

                            GUIContent label = new
                            ("Selected Airborne Move",
                             "This variable is shown through an editor script. It is not a part of the AirborneAttackStateData class. \nYou can change which move is selected from the \"Manage Movesets\" window.");
                            break;
                        }
                    }
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    static void DrawMenuHeader()
    {
        UtilityWindow.DrawBackButton();
        EditorGUILayout.LabelField("Manage State Data", EditorStyles.boldLabel);
    }
}

internal class StateData
{
    public readonly string name;
    public readonly Type type;
    public Object data;
    
    // Initialize as true to open the foldout by default
    public bool foldout = true;
    
    public StateData(string name, Type type)
    {
        this.name = name;
        this.type = type;

        string   filter = $"t:{type.Name}";
        string[] guids  = AssetDatabase.FindAssets(filter);

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            data = AssetDatabase.LoadAssetAtPath(path, type);
        }
    }
}
