using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using static UnityEngine.GUILayout;
using Slider = UnityEngine.UI.Slider;

public class NewHealthbar : MonoBehaviour
{
    [Header("Serialized References")]
    [SerializeField] Slider leftHealthbar;
    [SerializeField] Slider rightHealthbar;
    
    [Header("Optional Parameters")]
    [SerializeField] float changeRate;

    // Cached References
    HealthbarManager healthbarManager;
    
    // Events
    public delegate void OnPlayerDeath();
    public static event OnPlayerDeath onPlayerDeath;
    
    void Start()
    {
        healthbarManager = FindObjectOfType<HealthbarManager>();
        
        // Event subscription.
        onPlayerDeath += healthbarManager.ReloadOnDeath;
    }

    // Properties
    public Slider LeftHealthbar => leftHealthbar;
    public Slider RightHealthbar => rightHealthbar;

    public float LeftHealthbarValue
    {
        get => leftHealthbar.value;
        set
        {
            // The health before deducting after a hit. TODO: This is unused, but will be implemented later.
            float rightHealthbarPreviousValue = leftHealthbar.value;

            leftHealthbar.value = value;
            leftHealthbar.value = Mathf.Clamp(LeftHealthbar.value, 0, 100);
            //TODO: Currently, health is clamped between 0 and 100.
            //TODO: If we want more or less, don't forget to edit this.

            bool isPlayerOneDead = LeftHealthbarValue <= 0;
            if (isPlayerOneDead) onPlayerDeath?.Invoke();
        }
    }

    public float RightHealthbarValue
    {
        get => rightHealthbar.value;
        set
        {
            // The health before deducting after a hit. TODO: This is unused, but will be implemented later.
            float leftHealthbarPreviousValue = leftHealthbar.value;

            rightHealthbar.value = value;
            rightHealthbar.value = Mathf.Clamp(RightHealthbar.value, 0, 100);
            //TODO: Currently, health is clamped between 0 and 100.
            //TODO: If we want more or less, don't forget to edit this.

            bool isPlayerTwoDead = RightHealthbarValue <= 0;
            if (isPlayerTwoDead) onPlayerDeath?.Invoke();
        }
    }

    /// <summary>
    /// Adjusts the value of the healthbars.
    /// <remarks> !! This method is not intended to be the one that damages players in the final product, this is simply for debugging purposes. </remarks>
    /// </summary>
    /// <param name="isPlayerOne"> Player ONE is on the LEFT, while Player TWO is on RIGHT. </param>
    /// <param name="newHealth"> The new health to adjust the healthbar to. </param>
    /// <param name="deductionRate"> The amount to reduce the health by each frame. </param>
    public void AdjustHealthbar(bool isPlayerOne, int newHealth, float deductionRate)
    {
        switch (isPlayerOne)
        {
            case true:
                LeftHealthbarValue = Mathf.MoveTowards(LeftHealthbarValue, newHealth, deductionRate * Time.deltaTime);
                break;

            default:
                RightHealthbarValue = Mathf.MoveTowards(RightHealthbarValue, newHealth, deductionRate * Time.deltaTime);
                break;
        }
    }
}

[CustomEditor(typeof(NewHealthbar))]
public class NewHealthbarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var healthbar = (NewHealthbar) target;

        EditorGUILayout.Space(15);

            Space();
        
        // Display the values of the healthbars through sliders.
        LabelField("Healthbar Values", EditorStyles.largeLabel, Height(20));
        
            Space();
        
        LabelField("Left Healthbar", EditorStyles.boldLabel);
        healthbar.LeftHealthbarValue  = IntSlider((int) healthbar.LeftHealthbarValue, 0, 100);
        LabelField("Right Healthbar", EditorStyles.boldLabel);
        healthbar.RightHealthbarValue = IntSlider((int) healthbar.RightHealthbarValue, 0, 100);
        
           Space();
        
        // Display the debugging tools.
        LabelField("Debugging Tools", EditorStyles.boldLabel);
        
        // Buttons to set the healthbars to 100.
        EditorGUILayout.BeginHorizontal();
        if (Button("Set Left to 100"))
        {
            healthbar.LeftHealthbarValue = 100;
        }
        if (Button("Set Right to 100"))
        {
            healthbar.RightHealthbarValue = 100;
        }
        EditorGUILayout.EndHorizontal();
        
        // Set the healthbars to 0.
        EditorGUILayout.BeginHorizontal();
        if (Button("Set Left to 0"))
        {
            healthbar.LeftHealthbarValue = 0;
        }
        if (Button("Set Right to 0"))
        {
            healthbar.RightHealthbarValue = 0;
        }
        EditorGUILayout.EndHorizontal();
    }
}
