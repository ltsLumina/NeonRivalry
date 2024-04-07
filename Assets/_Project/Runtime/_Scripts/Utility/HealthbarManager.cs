using System.Collections.Generic;
using UnityEngine;

public class HealthbarManager : MonoBehaviour
{
    public delegate void PlayerDeath(PlayerController player);
    public static event PlayerDeath OnAnyPlayerDeath; // Checks if any player has died. Globally accessible. 
    
    public static void Initialize()
    {
        foreach (Healthbar healthbar in Healthbars) { healthbar.OnPlayerDeath += SubscribeHealthbars; }
    }

    void OnDestroy()
    {
        foreach (Healthbar healthbar in Healthbars) { healthbar.OnPlayerDeath -= SubscribeHealthbars; }
    }

    static void SubscribeHealthbars(PlayerController player) => OnAnyPlayerDeath?.Invoke(player);
    
    // -- Properties --

    /// <summary>
    /// Represents a list of Healthbars in the game.
    /// </summary>
    public static List<Healthbar> Healthbars { get; } = new ();

    /// <summary>
    /// Gets the Healthbar of the first player.
    /// </summary>
    public static Healthbar PlayerOne => Healthbars.Count > 0 ? Healthbars[0] : null;

    /// <summary>
    /// Gets the Healthbar of the second player.
    /// </summary>
    public static Healthbar PlayerTwo => Healthbars.Count > 1 ? Healthbars[1] : null;

    void Awake() =>
        // Clear the list of Healthbars as Unity won't clear the list when the game is restarted due to Enter Playmode Options.
        Healthbars.Clear();
}