using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Attributes;

public abstract class AttributesEditor
{
    /// <summary>
    ///     Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector.
    ///     Small but useful script, to make your inspectors look pretty and useful.
    ///     <example> [SerializeField, ReadOnly] int myInt; </example>
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}
