#region
using System;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

namespace Lumina.Essentials.Attributes
{
    // ReadOnly Attribute //

    /// Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector.
    /// <example> [SerializeField, ReadOnly] bool readOnlyBool; </example>
    /// <remarks> Must either be declared public or [SerializeField]</remarks>
    public class ReadOnlyAttribute : PropertyAttribute
    { }

    ////////////////////////////
    // Ranged Float Attribute //
    ////////////////////////////

    /// Allows you to add a range to a float variable in the inspector.
    /// <example> [RangedFloat(0, 1)] float exampleFloat; </example>
    /// <remarks> </remarks>
    [Serializable]
    public class RangedFloat
    {
        public float min;
        public float max;

        public RangedFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        ///     Gets a random value within the min and max range.
        /// </summary>
        /// <returns></returns>
        public float GetRandomValue() => Random.Range(min, max); //End of getRandomValue

        public override string ToString() => $"[Class: {nameof(RangedFloat)}, Min: {min}, Max: {max}]";

        /// <summary>
        ///     Implicit operator that will automatically fetch a random value within range when a <see cref="RangedFloat" /> is
        ///     used as a <see cref="float" />.
        /// </summary>
        /// <param name="someRangedFloat"></param>
        public static implicit operator float(RangedFloat someRangedFloat) => someRangedFloat.GetRandomValue(); //End of implicit operator float
    }

    /// <summary>
    ///     Attribute that allows you to assign a range to a float variable in the inspector with different options for how the
    ///     range is displayed.
    ///     <example> [RangedFloat(0, 1), RangeDisplayType] float exampleFloat; </example>
    /// </summary>
    [Serializable]
    public class RangedFloatAttribute : PropertyAttribute
    {
        ///////////////
        //    Enum   //
        ///////////////

        public enum RangeDisplayType
        {
            LockedRanges,
            EditableRanges,
            HideRanges,
        }

        ///////////////
        // Variables //
        ///////////////

        public float max;
        public float min;
        public RangeDisplayType rangeDisplayType;

        /////////////////
        // Constructor //
        /////////////////

        public RangedFloatAttribute(float min, float max, RangeDisplayType rangeDisplayType = RangeDisplayType.LockedRanges)
        {
            this.min              = min;
            this.max              = max;
            this.rangeDisplayType = rangeDisplayType;
        }
    }
}
