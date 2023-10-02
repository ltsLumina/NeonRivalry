using UnityEditor;
using UnityEngine;

// Used to display the moveset in the inspector.
[CustomEditor(typeof(AttackSystem))]
public class AttackSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var     attackSystem = (AttackSystem) target;
        Moveset moveset      = attackSystem.ActiveMoveset;
        
        // Draw the default inspector of the AttackSystem class.
        base.OnInspectorGUI();
        
        GUILayout.Space(35);

        GUILayout.Label("Active Moveset", EditorStyles.boldLabel);

        // Displays the moveset in the inspector as read-only.
        using (new EditorGUI.DisabledScope(true))
        {
            Editor inspector = CreateEditor(moveset);
            inspector.OnInspectorGUI();
        }
    }
}
