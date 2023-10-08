using System;
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
            SetSliderValues(playerOne, playerTwo, 100);
            SetSliderValues(playerOne, playerTwo, 0);
        }
    }

    #region Utility
    static Healthbar GetHealthbar(HealthbarManager manager, int index) =>
        manager.Healthbars.Count > index ? manager.Healthbars[index] : null;

    static void SetSliderValues(Healthbar playerOne, Healthbar playerTwo, float value)
    {
        using (new EditorGUILayout.HorizontalScope())
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