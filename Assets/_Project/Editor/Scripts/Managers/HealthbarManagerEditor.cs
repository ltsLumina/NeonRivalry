using System;
using Lumina.Debugging;
using UnityEditor;
using UnityEngine;

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
            FGDebugger.Debug("HealthbarManager is null.", LogType.Exception);
            return;
        } 

        DisplaySectionTitle("Healthbar Values");

        if (HealthbarManager.Healthbars.Count == 0) return;
        
        AdjustSliders();

        DisplaySectionTitle("Debugging Tools");
        
        SetHealth();
        
        DebugToggles();
    }

    static void DebugToggles()
    {
        GUILayout.Space(15);
        
        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.Label("Debugging Toggles", EditorStyles.boldLabel);
            
            manager.Invincible = EditorGUILayout.Toggle("Invincible", manager.Invincible);
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
        Healthbar playerOne = HealthbarManager.PlayerOne;
        Healthbar playerTwo = HealthbarManager.PlayerTwo;

        if (playerOne != null || playerTwo != null)
        {
            SetSliderValues(playerOne, playerTwo, 100);
            SetSliderValues(playerOne, playerTwo, 0);
        }
    }

    #region Utility
    static void SetSliderValues(Healthbar playerOne, Healthbar playerTwo, float value)
    {
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button($"Set Left to {value}"))
                try { playerOne.Slider.value = value; } 
                catch (NullReferenceException e)
                {
                    Debug.LogError($"Player One Healthbar is null. \n {e}");
                    throw;
                }

            if (GUILayout.Button($"Set Right to {value}"))
                try { playerTwo.Slider.value = value; } 
                catch (NullReferenceException e)
                {
                    Debug.LogError($"Player One Healthbar is null. \n {e}");
                    throw;
                }
        }
    }
    #endregion

    static void AdjustSliders()
    {
        if (HealthbarManager.Healthbars.Count >= 1) AdjustHealthbarSlider(HealthbarManager.Healthbars[0], "Left Healthbar");
        if (HealthbarManager.Healthbars.Count >= 2) AdjustHealthbarSlider(HealthbarManager.Healthbars[1], "Right Healthbar");
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