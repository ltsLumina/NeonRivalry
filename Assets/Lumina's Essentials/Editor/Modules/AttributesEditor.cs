#region
using System;
using System.Globalization;
using Lumina.Essentials.Editor.UI.Management;
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Attributes.Editor
{
    public static class AttributesEditor
    {
        //////////////////////////
        //  ReadOnly Attribute  //
        //////////////////////////

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

        //////////////////////////////////
        //  RangedFloatAttributeDrawer  //
        //////////////////////////////////

        [CustomPropertyDrawer(typeof(RangedFloatAttribute))]
        class RangedFloatAttributeDrawer : PropertyDrawer
        {
            const float COMPONENT_HEIGHT = 16f;
            const float VERTICAL_PADDING = 2f;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.type.Equals(nameof(RangedFloat)))
                {
                    #region Variable Initialization
                    SerializedProperty currentMin = property.FindPropertyRelative("min");
                    SerializedProperty currentMax = property.FindPropertyRelative("max");

                    float currentMaxCopy = currentMax.floatValue;
                    float currentMinCopy = currentMin.floatValue;

                    var validRange = attribute as RangedFloatAttribute;
                    #endregion Variable Initialization

                    #region Editor Drawing
                    label = EditorGUI.BeginProperty(position, label, property);

                    var sliderRect = new Rect(position.x, position.y, position.width, COMPONENT_HEIGHT);

                    EditorGUI.BeginChangeCheck();

                    // Given how the valid ranges themselves aren't stored anywhere, when you close the inspector the valid range values will reset.
                    // In an attempt to "remember" them, if the current value "breaks" the limits, it means that the limit was lowered, or increased manually last time,
                    // so the current values will be used as temporal limits when reloaded.
                    if (validRange?.rangeDisplayType == RangedFloatAttribute.RangeDisplayType.EditableRanges)
                    {
                        if (validRange.min > currentMinCopy) validRange.min = currentMinCopy;

                        if (validRange.max < currentMaxCopy) validRange.max = currentMaxCopy;
                    }

                    if (validRange != null)
                    {
                        EditorGUI.MinMaxSlider(sliderRect, label, ref currentMinCopy, ref currentMaxCopy, validRange.min, validRange.max);

                        if (validRange.rangeDisplayType != RangedFloatAttribute.RangeDisplayType.HideRanges)
                        {
                            Rect lower = EditorGUI.PrefixLabel(sliderRect, label);
                            lower.y += COMPONENT_HEIGHT + VERTICAL_PADDING;
                            var upper = new Rect(lower.x, lower.y + COMPONENT_HEIGHT + VERTICAL_PADDING, lower.width, COMPONENT_HEIGHT);

                            switch (validRange.rangeDisplayType)
                            {
                                case RangedFloatAttribute.RangeDisplayType.LockedRanges:
                                    currentMinCopy = EditorGUI.FloatField
                                        (lower, $"Lower (Min: {validRange.min.ToString(CultureInfo.InvariantCulture)})", currentMinCopy);

                                    currentMaxCopy = EditorGUI.FloatField
                                        (upper, $"Upper (Max: {validRange.max.ToString(CultureInfo.InvariantCulture)})", currentMaxCopy);

                                    break;

                                case RangedFloatAttribute.RangeDisplayType.EditableRanges:
                                    // Draw lower
                                    lower.width /= 4f;
                                    EditorGUI.LabelField(lower, new GUIContent("Lowest", "Minimal value that the lower bound can get to."));
                                    lower.x        += lower.width;
                                    validRange.min =  EditorGUI.FloatField(lower, validRange.min);
                                    lower.x        += lower.width;
                                    EditorGUI.LabelField(lower, new GUIContent("Current", "The current min value in the slider."));
                                    lower.x        += lower.width;
                                    currentMinCopy =  EditorGUI.FloatField(lower, currentMinCopy);

                                    // Draw upper
                                    upper.width /= 4f;
                                    EditorGUI.LabelField(upper, new GUIContent("Highest", "Maximum value that the upper bound can get to."));
                                    upper.x        += upper.width;
                                    validRange.max =  EditorGUI.FloatField(upper, validRange.max);
                                    upper.x        += upper.width;
                                    EditorGUI.LabelField(upper, new GUIContent("Current", "The current max value in the slider."));
                                    upper.x        += upper.width;
                                    currentMaxCopy =  EditorGUI.FloatField(upper, currentMaxCopy);
                                    break;

                                case RangedFloatAttribute.RangeDisplayType.HideRanges:
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        #endregion Editor Drawing

                        #region Clamp Values
                        if (EditorGUI.EndChangeCheck())
                        {
                            // If it is attempted to make upper limit smaller than lower, clamp the upper limit to the value of the lower.
                            if (currentMaxCopy < currentMinCopy) currentMax.floatValue = currentMin.floatValue;

                            // Is the provided lower limit valid? If so, keep it.
                            else if (currentMinCopy < validRange.min) currentMin.floatValue = validRange.min;

                            // The provided lower value is smaller than the lower limit, clamp to minimal accepted.
                            else currentMin.floatValue = currentMinCopy;

                            // If it is attempted to make lower limit greater than upper, clamp the lower limit to the value of the upper.
                            if (currentMinCopy > currentMaxCopy) currentMin.floatValue = currentMax.floatValue;

                            // Is the provided upper value valid? If so, keep it.
                            else if (currentMaxCopy > validRange.max) currentMax.floatValue = validRange.max;

                            // The provided max value is greater than the allowed max, clamp it to max allowed.
                            else currentMax.floatValue = currentMaxCopy;
                        }
                    }
                    #endregion Clamp Values

                    EditorGUI.EndProperty();
                }
                else
                {
                    EssentialsDebugger.LogError
                    ($"Attempting to use the <b>'[{nameof(RangedFloat)}(float min, float max)]'</b> attribute on a <color=red>{property.type}</color> type field. " +
                     $"Should be <color=green>'{nameof(RangedFloat)}'</color> instead.");
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var rangedFloatAttribute = attribute as RangedFloatAttribute;
                int additionalRows       = rangedFloatAttribute?.rangeDisplayType == RangedFloatAttribute.RangeDisplayType.HideRanges ? 0 : 2;
                return base.GetPropertyHeight(property, label) + COMPONENT_HEIGHT + additionalRows * COMPONENT_HEIGHT;
            }
        }

        //////////////////////////
        //  RangedFloatDrawer  //
        //////////////////////////

        [CustomPropertyDrawer(typeof(RangedFloat))]
        public class RangedFloatDrawer : PropertyDrawer
        {
            //------------------------------------------------------------------------------------//
            //----------------------------------- FIELDS -----------------------------------------//
            //------------------------------------------------------------------------------------//

            const int AMOUNT_OF_ITEMS = 1;
            const float SPACER_HEIGHT = 20f;
            const float LINE_HEIGHT = 16f;
            string name = string.Empty;
            string tooltip = string.Empty;
            bool cache;

            //------------------------------------------------------------------------------------//
            //---------------------------------- METHODS -----------------------------------------//
            //------------------------------------------------------------------------------------//

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => AMOUNT_OF_ITEMS * SPACER_HEIGHT;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUIUtility.labelWidth /= 4f;
                position.height             =  LINE_HEIGHT;
                position.width              /= 4f;

                if (!cache)
                {
                    //get the name before it's gone
                    name    = property.displayName;
                    tooltip = property.tooltip;

                    cache = true;
                }

                EditorGUI.PrefixLabel(position, new (name, $"Base Tooltip: {(tooltip.Equals(string.Empty) ? "" : $"\n\n{name}'s Tooltip:\n{tooltip}")}"));

                position.x += position.width;

                position.width *= 4f;
                position.width *= 0.375f;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("min"), new GUIContent("Min"));

                position.x += position.width;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("max"), new GUIContent("Max"));
            } // OnGUI()
        }     // class
    }

}