#region
using UnityEngine;
#endregion

public abstract class MoveEffect : ScriptableObject
{
    /// <summary>
    ///     Applies the effect of an attack.
    /// </summary>
    /// <param name="target"> The target player to be affected by the effect.</param>
    public abstract void ApplyEffect(PlayerController target);
}
