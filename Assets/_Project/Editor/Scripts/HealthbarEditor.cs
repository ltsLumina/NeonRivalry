using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Healthbar))]
public class HealthbarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var healthbar = (Healthbar) target;

        EditorGUILayout.Space(15);

        EditorGUILayout.Space();
        
        // Display the values of the healthbars through sliders.
        EditorGUILayout.LabelField("Healthbar Values", EditorStyles.largeLabel, GUILayout.Height(20));
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Left Healthbar", EditorStyles.boldLabel);
        healthbar.LeftHealthbarValue = EditorGUILayout.IntSlider((int) healthbar.LeftHealthbarValue, 0, 100);
        EditorGUILayout.LabelField("Right Healthbar", EditorStyles.boldLabel);
        healthbar.RightHealthbarValue = EditorGUILayout.IntSlider((int) healthbar.RightHealthbarValue, 0, 100);
        
        EditorGUILayout.Space();
        
        // Display the debugging tools.
        EditorGUILayout.LabelField("Debugging Tools", EditorStyles.boldLabel);
        
        // Buttons to set the healthbars to 100.
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Set Left to 100"))
        {
            healthbar.LeftHealthbarValue = 100;
        }
        if (GUILayout.Button("Set Right to 100"))
        {
            healthbar.RightHealthbarValue = 100;
        }
        EditorGUILayout.EndHorizontal();
        
        // Set the healthbars to 0.
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Set Left to 0"))
        {
            healthbar.LeftHealthbarValue = 0;
        }
        if (GUILayout.Button("Set Right to 0"))
        {
            healthbar.RightHealthbarValue = 0;
        }
        EditorGUILayout.EndHorizontal();
    }
}
