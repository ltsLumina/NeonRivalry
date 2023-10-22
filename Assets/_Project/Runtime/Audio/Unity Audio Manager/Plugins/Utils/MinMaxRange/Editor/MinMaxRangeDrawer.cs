using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var range = attribute as MinMaxRangeAttribute;
        
        var minValue = property.FindPropertyRelative("RangeMin");
        var maxValue = property.FindPropertyRelative("RangeMax");
        var newMin = minValue.floatValue;
        var newMax = maxValue.floatValue;

        var height = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, height), label);

        newMin = Mathf.Clamp(EditorGUI.FloatField(new Rect(EditorGUIUtility.labelWidth + 15, position.y, EditorGUIUtility.fieldWidth, height), newMin), range.MinLimit, newMax);

        float sliderWidth = position.width - EditorGUIUtility.labelWidth - (EditorGUIUtility.fieldWidth * 2) - 13;
        EditorGUI.MinMaxSlider(new Rect(EditorGUIUtility.labelWidth + 70, position.y, sliderWidth, height), ref newMin, ref newMax, range.MinLimit, range.MaxLimit);

        newMax = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + position.width - 50, position.y, 50, height), newMax), newMin, range.MaxLimit);

        newMin = float.Parse(newMin.ToString("F2"));
        newMax = float.Parse(newMax.ToString("F2"));

        EditorGUI.EndProperty();

        minValue.floatValue = newMin;
        maxValue.floatValue = newMax;
    }
}