using UnityEngine;

namespace Lumina.Essentials
{
    public abstract class Attributes
    {
        /// <example> [SerializeField, ReadOnly] bool readOnlyBool; </example>
        /// <remarks> Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector. </remarks>
        public class ReadOnlyAttribute : PropertyAttribute
        { }
    }
}
