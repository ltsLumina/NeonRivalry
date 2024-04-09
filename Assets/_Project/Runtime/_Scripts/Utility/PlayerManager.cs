using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The PlayerManager class is used to manage all players in the game.
/// Contains useful methods and properties for managing players.
/// <seealso cref="PlayerController"/>
/// </summary>
public static class PlayerManager
{
    #region Player List & Properties
    /// <summary>
    /// Maintains a list of all Player instances.
    /// </summary>
    public static List<PlayerController> Players { get; } = new ();
    public static List<MenuNavigator> MenuNavigators { get; } = new ();
    
    public static int PlayerCount => Players.Count;

    /// <summary>
    /// Gets the first player in the Players list if it exists; otherwise returns null.
    /// This property is often used when accessing the PlayerController component of the first player.
    /// </summary>
    //public static PlayerController PlayerOne => Players.Count >= 1 ? Players[0].PlayerController : null;
    public static PlayerController PlayerOne => Players.Count >= 1 ? Players[0] : null;
    public static MenuNavigator MenuNavigatorOne => Players.Count >= 1 ? MenuNavigators[0] : null;
    
    /// <summary>
    /// Gets the second player in the Players list if it exists; otherwise returns null.
    /// This property is often used when accessing the PlayerController component of the second player.
    /// </summary>
    public static PlayerController PlayerTwo => Players.Count >= 2 ? Players[1] : null;
    public static MenuNavigator MenuNavigatorTwo => Players.Count >= 2 ? MenuNavigators[1] : null;

    /// <summary>
    /// Gets the player with the specified player ID if it exists.
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    public static PlayerController GetPlayer(int playerID) => playerID switch
    { 1 => PlayerOne,
      2 => PlayerTwo,
      _ => null };
    
    public static MenuNavigator GetMenuNavigator(int playerID) => playerID switch
    { 1 => MenuNavigatorOne,
      2 => MenuNavigatorTwo,
      _ => null };


    /// <summary>
    /// Retrieves a player based on the input device they are using.
    /// </summary>
    /// <param name="device">The input device (keyboard, gamepad etc.) used by the player.</param>
    /// <returns>The player using the specified input device, or null if no such player exists.</returns>
    public static PlayerController GetPlayer(InputDevice device) => Players.Find(player => player.PlayerInput.devices.Contains(device));
    
    /// <summary>
    /// Gets the first player in the Players list if it exists; otherwise returns null.
    /// This property provides a quick reference to verify if there are any players active. Useful in scenarios such as determining if any player is currently taking damage.
    /// </summary>
    public static PlayerController AnyPlayer => Players.Count >= 1 ? Players[0] : null;
    
    public static MenuNavigator AnyMenuNavigator => MenuNavigators.Count >= 1 ? MenuNavigators[0] : null;
    #endregion

    #region Hurtboxes
    /// <summary> <returns> The <see cref="HurtBox"/> component of Player One. </returns> </summary>
    public static HurtBox PlayerOneHurtBox => PlayerOne.HurtBox;
    /// <summary> <returns> The <see cref="HurtBox"/> component of Player Two. </returns> </summary>
    public static HurtBox PlayerTwoHurtBox => PlayerTwo.HurtBox;
    #endregion

    #region PlayerInput
    /// <summary>
    /// A list of all PlayerInput instances.
    /// </summary>
    public static IEnumerable<PlayerInput> PlayerInputs => Players.Select(player => player.PlayerInput);

    /// <summary>
    /// Gets the first PlayerInput instance if it exists; otherwise returns null.
    /// </summary>
    public static PlayerInput PlayerOneInput => PlayerOne.PlayerInput;
    
    /// <summary>
    /// Gets the second PlayerInput instance if it exists; otherwise returns null.
    /// </summary>
    public static PlayerInput PlayerTwoInput => PlayerTwo.PlayerInput;
    #endregion

    #region Utility

    public static PlayerController OtherPlayer(PlayerController player) => player == PlayerOne ? PlayerTwo : PlayerOne;

    static MenuNavigator OtherMenuNavigator(MenuNavigator navigator)
    {
        return navigator == MenuNavigatorOne ? MenuNavigatorTwo : MenuNavigatorOne;
    }

    public static void AddPlayer(PlayerController player) => Players.Add(player);
    public static void AddMenuNavigator(MenuNavigator navigator) => MenuNavigators.Add(navigator);
    
    public static void RemovePlayer(PlayerController player) => Players.Remove(player);
    public static void RemoveMenuNavigator(MenuNavigator navigator) => MenuNavigators.Remove(navigator);

    public static void AssignHealthbarToPlayer(PlayerController player, int playerID)
    {
        // Dictionary that maps the player's ID to the healthbar's tag and name.
        // The tag is used to find the healthbar in the scene, and the name is used to set the healthbar's name.
        Dictionary<int, (string tag, string name)> idMapping = new()
        { { 1, ("[Healthbar] Left", "Healthbar (Player 1) (Left)") },
          { 2, ("[Healthbar] Right", "Healthbar (Player 2) (Right)") } };

        // If the player's ID is not in the dictionary, return.
        if (!idMapping.TryGetValue(playerID, out (string tag, string name) key)) return;

        // Get the tag and name of the healthbar.
        (string tag, string name) = key;

        // Set the player's healthbar depending on the player's ID. Player 1 is on the left, Player 2 is on the right.
        var healthbar = GameObject.FindGameObjectWithTag(tag).GetComponent<Healthbar>();
        AssignHealthbar(player, healthbar, name);
    }
    
    static void AssignHealthbar(PlayerController player, Healthbar healthbar, string name)
    {
        // Assign the healthbar to the healthbar manager
        HealthbarManager.Healthbars.Add(healthbar);

        // Assign the healthbar to the player.
        player.Healthbar = healthbar;

        // Assign the player to the healthbar.
        healthbar.Player = player;

        // Set the healthbar's name.
        healthbar.name = name;
        
        // Initialize the healthbar.
        player.Healthbar.Initialize();
    }
    #endregion
}