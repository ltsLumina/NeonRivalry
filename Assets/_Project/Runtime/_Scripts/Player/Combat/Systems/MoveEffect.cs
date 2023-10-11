#region
using UnityEngine;
#endregion

public abstract class MoveEffect : ScriptableObject
{
    /// <summary>
    ///     Applies the effect of an attack.
    /// </summary>
    /// <param name="abilities">A reference to the PlayerAbilities class that holds all the data for each ability. </param>
    /// <param name="target"> The target player to be affected by the effect.</param>
    public abstract void ApplyEffect(PlayerAbilities abilities, PlayerController target);
}
