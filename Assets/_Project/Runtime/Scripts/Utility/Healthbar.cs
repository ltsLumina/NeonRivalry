using System;
using Lumina.Essentials.Attributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("Reference"), Space(5)]
    [SerializeField, ReadOnly] Slider slider;
    
    [Header("Healthbar Options"), Space(5)]
    [SerializeField]
    public int health;

    // Unity event for onPlayerDeath
    public UnityEvent onPlayerDeath;
    
    // Properties
    /// <summary> The slider associated with this <see cref="Healthbar"/>. </summary>
    public Slider Slider => slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        Debug.Log(slider, slider);
    }

    void Update()
    {
        health = (int) Slider.value;
    }

    public void OnDeath()
    {
        Debug.Log("Player has died!");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Healthbar))]
public class HealthbarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        Healthbar healthbar = (Healthbar) target;
        Slider    slider    = healthbar.Slider;
        
        if (slider == null) return;

        GUILayout.Space(10);
        
        EditorGUILayout.LabelField("Health", EditorStyles.boldLabel);
        slider.value = EditorGUILayout.IntSlider((int) slider.value, 0, 100);
        
        GUILayout.Space(25);
        
    }
}
#endif
