using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthbarManager))]
public class HealthbarManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var manager = (HealthbarManager) target;

        DisplaySectionTitle("Healthbar Values");
        
        if (manager != null) AdjustSliders(manager);

        DisplaySectionTitle("Debugging Tools");
        
        SetHealth(manager);
    }

    static void DisplaySectionTitle(string title) 
    {
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField(title, EditorStyles.largeLabel, GUILayout.Height(20));
        EditorGUILayout.Space();
    }

    static void SetHealth(HealthbarManager manager)
    {
        Healthbar playerOne = GetHealthbar(manager, 0);
        Healthbar playerTwo = GetHealthbar(manager, 1);

        if (playerOne != null || playerTwo != null)
        {
            SetSliderValues(playerOne, playerTwo, "Set to 100", 100);
            SetSliderValues(playerOne, playerTwo, "Set to 0", 0);
        }
    }

    #region Utility
    static Healthbar GetHealthbar(HealthbarManager manager, int index) =>
        manager.Healthbars.Count > index ? manager.Healthbars[index] : null;

    static void SetSliderValues(Healthbar playerOne, Healthbar playerTwo, string label, float value)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (playerOne != null && GUILayout.Button($"Set Left {label}")) playerOne.Slider.value  = value;
            if (playerTwo != null && GUILayout.Button($"Set Right {label}")) playerTwo.Slider.value = value;
        }
    }
    #endregion

    static void AdjustSliders(HealthbarManager manager)
    {
        if (manager.Healthbars.Count >= 1) AdjustHealthbarSlider(manager.Healthbars[0], "Left Healthbar");
        if (manager.Healthbars.Count >= 2) AdjustHealthbarSlider(manager.Healthbars[1], "Right Healthbar");
    }

    static void AdjustHealthbarSlider(Healthbar player, string label)
    {
        if (player != null)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            player.Slider.value = EditorGUILayout.IntSlider((int) player.Slider.value, 0, 100);
        }
    }
}