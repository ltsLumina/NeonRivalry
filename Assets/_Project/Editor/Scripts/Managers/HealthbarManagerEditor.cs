using UnityEditor;
using UnityEngine;
using Logger = Lumina.Debugging.Logger;

[CustomEditor(typeof(HealthbarManager))]
public class HealthbarManagerEditor : Editor
{
    static HealthbarManager manager;

    void OnEnable() => manager = (HealthbarManager) target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (manager == null)
        {
            Logger.Debug("HealthbarManager is null.", LogType.Exception);
            return;
        } 

        if (HealthbarManager.Healthbars.Count == 0) return;

        DisplaySectionTitle("Healthbar Values");
        
        AdjustSliders();

        DisplaySectionTitle("Debugging Tools");
        
        SetHealth();
        
        DebugToggles();
    }

    static void DebugToggles()
    {
        GUILayout.Space(15);

        GUILayout.Label("Debugging Toggles", EditorStyles.boldLabel);

        for (int index = 0; index < HealthbarManager.Healthbars.Count; index++)
        {
            Healthbar healthbar = HealthbarManager.Healthbars[index];
            GUILayout.Label($"Player {index + 1}", EditorStyles.boldLabel);
            healthbar.Invincible = EditorGUILayout.Toggle("Invincible: ", healthbar.Invincible);
        }
    }

    static void DisplaySectionTitle(string title) 
    {
        if (HealthbarManager.Healthbars.Count == 0) title = "No Healthbars Found";

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField(title, EditorStyles.largeLabel, GUILayout.Height(20));
        EditorGUILayout.Space();
    }

    static void SetHealth()
    {
        GUILayout.Label("Set Health", EditorStyles.boldLabel);

        SetSliderValues(HealthbarManager.PlayerOne, HealthbarManager.PlayerTwo, 100);
        SetSliderValues(HealthbarManager.PlayerOne, HealthbarManager.PlayerTwo, 50);
        SetSliderValues(HealthbarManager.PlayerOne, HealthbarManager.PlayerTwo, 25);
        SetSliderValues(HealthbarManager.PlayerOne, HealthbarManager.PlayerTwo, 0);
    }

    #region Utility
    // ReSharper disable Unity.PerformanceAnalysis
    static void SetSliderValues(Healthbar playerOne, Healthbar playerTwo, float value)
    {
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button($"Set Left to {value}"))
            {
                if (playerOne != null) playerOne.Value = (int) value;
                else Debug.LogError("Player One Healthbar is null.");
            }

            // Return early if there is only one healthbar.
            if (HealthbarManager.Healthbars.Count < 2) return;

            if (GUILayout.Button($"Set Right to {value}"))
            {
                if (playerTwo != null) playerTwo.Value = (int) value;
                else Debug.LogError("Player Two Healthbar is null.");
            }
        }
    }
    #endregion

    static void AdjustSliders()
    {
        if (HealthbarManager.Healthbars.Count >= 1) AdjustHealthbarSlider(HealthbarManager.PlayerOne, "Left Healthbar");
        if (HealthbarManager.Healthbars.Count >= 2) AdjustHealthbarSlider(HealthbarManager.PlayerTwo, "Right Healthbar");
    }

    static void AdjustHealthbarSlider(Healthbar healthbar, string label)
    {
        if (healthbar != null)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            healthbar.Value = EditorGUILayout.IntSlider(healthbar.Value, 0, 100);
        }
    }
}