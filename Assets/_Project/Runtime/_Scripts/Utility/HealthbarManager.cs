using System.Collections.Generic;
using UnityEngine;

public class HealthbarManager : MonoBehaviour
{
    // -- Properties --

    /// <summary>
    /// Represents a list of Healthbars in the game.
    /// </summary>
    public static List<Healthbar> Healthbars { get; } = new();

    /// <summary>
    /// Gets the Healthbar of the first player.
    /// </summary>
    public static Healthbar PlayerOne => Healthbars.Count > 0 ? Healthbars[0] : null;

    /// <summary>
    /// Gets the Healthbar of the second player.
    /// </summary>
    public static Healthbar PlayerTwo => Healthbars.Count > 1 ? Healthbars[1] : null;
}