using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthbarManager))]
public class HealthbarManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var manager = (HealthbarManager) target;

        EditorGUILayout.Space(15);
        
        // Display the values of the healthbars through sliders.
        EditorGUILayout.LabelField("Healthbar Values", EditorStyles.largeLabel, GUILayout.Height(20));
        
        EditorGUILayout.Space();

        if (manager == null) return; 
        
        AdjustSliders(manager);

        EditorGUILayout.Space();
        
        // Display the debugging tools.
        EditorGUILayout.LabelField("Debugging Tools", EditorStyles.boldLabel);
        
        SetHealth(manager);
    }
    
    static void SetHealth(HealthbarManager manager)
    { // Buttons to set the healthbars value only if players exist.
        using (new EditorGUILayout.HorizontalScope())
        {
            if (manager.PlayerOne != null && GUILayout.Button("Set Left to 100")) manager.PlayerOne.Slider.value  = 100;
            if (manager.PlayerTwo != null && GUILayout.Button("Set Right to 100")) manager.PlayerTwo.Slider.value = 100;
        }

        // Set the healthbars to 0 only if players exist.
        using (new EditorGUILayout.HorizontalScope())
        {
            if (manager.PlayerOne != null && GUILayout.Button("Set Left to 0")) manager.PlayerOne.Slider.value  = 0;
            if (manager.PlayerTwo != null && GUILayout.Button("Set Right to 0")) manager.PlayerTwo.Slider.value = 0;
        }
    }

    static void AdjustSliders(HealthbarManager manager)
    { // Adjust the Left Healthbar
        if (manager.PlayerOne != null)
        {
            EditorGUILayout.LabelField("Left Healthbar", EditorStyles.boldLabel);
            manager.PlayerOne.Slider.value = EditorGUILayout.IntSlider((int) manager.PlayerOne.Slider.value, 0, 100);
        }

        // Adjust the Right Healthbar
        if (manager.PlayerTwo != null)
        {
            EditorGUILayout.LabelField("Right Healthbar", EditorStyles.boldLabel);
            manager.PlayerTwo.Slider.value = EditorGUILayout.IntSlider((int) manager.PlayerTwo.Slider.value, 0, 100);
        }
    }
}