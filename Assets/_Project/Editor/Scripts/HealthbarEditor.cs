using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(Healthbar))]
public class HealthbarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Healthbar healthbar = (Healthbar) target;
        Slider    slider    = healthbar.Slider;

        if (slider == null) return;

        GUILayout.Space(5);

        healthbar.Slider.value = EditorGUILayout.IntSlider((int)healthbar.Slider.value, (int) slider.minValue, (int) slider.maxValue);

        GUILayout.Space(25);
    }
}
