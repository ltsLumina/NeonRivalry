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
    public enum PlayerID
    {
        PlayerOne = 1,
        PlayerTwo = 2
    }
    
    #region Player List & Properties
    /// <summary>
    /// Maintains a list of all Player instances.
    /// </summary>
    public static List<Player> Players { get; } = new ();
    
    //public static List<MenuNavigator> Players { get; } = new ();

    /// <summary>
    /// Gets the first player in the Players list if it exists; otherwise returns null.
    /// This property is often used when accessing the PlayerController component of the first player.
    /// </summary>
    //public static PlayerController PlayerOne => Players.Count >= 1 ? Players[0].PlayerController : null;
    public static Player PlayerOne => Players.Count >= 1 ? new Player(playerController: Players[0].PlayerController, menuNavigator: Players[0].MenuNavigator) : null;
    
    /// <summary>
    /// Gets the second player in the Players list if it exists; otherwise returns null.
    /// This property is often used when accessing the PlayerController component of the second player.
    /// </summary>
    public static Player PlayerTwo => Players.Count >= 2 ? new Player(playerController: Players[1].PlayerController, menuNavigator: Players[1].MenuNavigator) : null;

    /// <summary>
    /// Gets the player with the specified player ID if it exists.
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    public static Player GetPlayer(PlayerID playerID) => playerID switch
    { PlayerID.PlayerOne => PlayerOne,
      PlayerID.PlayerTwo => PlayerTwo,
      _                  => null };

    /// <summary>
    /// Gets the first player in the Players list if it exists; otherwise returns null.
    /// This property provides a quick reference to verify if there are any players active. Useful in scenarios such as determining if any player is currently taking damage.
    /// </summary>
    public static PlayerController AnyPlayer => Players.Count >= 1 ? Players[0].PlayerController : null;

    /// <summary>
    /// Gets the number of players that are currently in the 'Players' list as PlayerControllers.
    /// </summary>
    public static int PlayerControllerCount => Players.Count(p => p.PlayerController);
    
    public static int MenuNavigatorCount => Players.Count(p => p.MenuNavigator);
    #endregion

    #region Hurtboxes
    /// <summary> <returns> The <see cref="HurtBox"/> component of Player One. </returns> </summary>
    public static HurtBox PlayerOneHurtBox => PlayerOne.PlayerController.HurtBox;
    /// <summary> <returns> The <see cref="HurtBox"/> component of Player Two. </returns> </summary>
    public static HurtBox PlayerTwoHurtBox => PlayerTwo.PlayerController.HurtBox;
    #endregion

    #region PlayerInput
    /// <summary>
    /// A list of all PlayerInput instances.
    /// </summary>
    public static IEnumerable<PlayerInput> PlayerInputs => Players.ConvertAll(player => player.PlayerController.PlayerInput);

    /// <summary>
    /// Gets the first PlayerInput instance if it exists; otherwise returns null.
    /// </summary>
    public static PlayerInput PlayerOneInput => PlayerOne.PlayerController.PlayerInput;
    
    /// <summary>
    /// Gets the second PlayerInput instance if it exists; otherwise returns null.
    /// </summary>
    public static PlayerInput PlayerTwoInput => PlayerTwo.PlayerController.PlayerInput;
    #endregion

    #region Utility
    public static PlayerController OtherPlayerController(PlayerController player)
    {
        if (player && PlayerTwo != null) return player == PlayerOne.PlayerController ? PlayerTwo.PlayerController : PlayerOne.PlayerController;
        return null;
    }

    public static MenuNavigator OtherMenuNavigator(Player player)
    {
        if (player != null && PlayerTwo != null) return player == PlayerOne ? PlayerTwo.MenuNavigator : PlayerOne.MenuNavigator;
        return null;
    }
    
    public static void AddPlayer(PlayerController playerController = default, MenuNavigator menuNavigator = default)
    {
        Player player = new(playerController, menuNavigator);
        Players.Add(player);
    }
    
    public static void RemovePlayer(Player player) => Players.Remove(player);

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

public class Player
{
    public PlayerController PlayerController { get; }
    public MenuNavigator MenuNavigator { get; }

    public Player(PlayerController playerController, MenuNavigator menuNavigator)
    {
        PlayerController = playerController;
        MenuNavigator    = menuNavigator;
    }
}