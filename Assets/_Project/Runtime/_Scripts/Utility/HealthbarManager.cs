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
    public static Healthbar PlayerOne => Healthbars[0];

    /// <summary>
    /// Gets the Healthbar of the second player.
    /// </summary>
    public static Healthbar PlayerTwo => Healthbars[1];
    
    /// <summary>
    /// Used to determine whether or not the player is invincible.
    /// Only meant to be used for debugging purposes.
    /// </summary>
    public bool Invincible { get; set; }
}