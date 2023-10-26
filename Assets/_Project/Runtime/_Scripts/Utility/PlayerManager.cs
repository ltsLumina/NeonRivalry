using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The PlayerManager class is used to manage ALL players in the game including their settings, properties, and actions.
/// <seealso cref="PlayerController"/>
/// </summary>
public partial class PlayerManager : MonoBehaviour
{
    // -- Player Settings --
    
    [Header("Settings"), Space(5)]
    [SerializeField] PlayerDetails playerDetails;
    
    public PlayerColor PlayerColors => playerDetails.playerColors;
    public PlayerSpawnPoint PlayerSpawnPoints => playerDetails.playerSpawnPoints;

    #region Player List & Properties
    /// <summary>
    /// Maintains a list of all PlayerController instances.
    /// </summary>
    public static List<PlayerController> Players { get; } = new ();

    /// <summary>
    /// Gets the first player in the Players list if it exists; otherwise returns null.
    /// This property is often used when accessing the PlayerController component of the first player.
    /// </summary>
    public static PlayerController PlayerOne => Players.Count >= 1 ? Players[0] : null;

    /// <summary>
    /// Gets the second player in the Players list if it exists; otherwise returns null.
    /// This property is often used when accessing the PlayerController component of the second player.
    /// </summary>
    public static PlayerController PlayerTwo => Players.Count >= 2 ? Players[1] : null;

    /// <summary>
    /// Gets the first player in the Players list if it exists; otherwise returns null.
    /// This property provides a quick reference to verify if there are any players active. Useful in scenarios such as determining if any player is currently taking damage.
    /// </summary>
    public static PlayerController AnyPlayer => Players.Count > 0 ? Players[0] : null;
    
    /// <summary>
    /// Gets the player with the specified player ID if it exists.
    /// <para> Use ID 1 for Player One and ID 2 for Player Two. </para>
    /// </summary>
    /// <param name="playerID"> The player ID of the player to get. </param>
    /// <returns> The player with the specified player ID if it exists; otherwise returns null. </returns>
    public static PlayerController GetPlayer(int playerID) => Players[playerID - 1];

    /// <summary>
    /// Gets the number of players that are currently in the 'Players' list.
    /// Does not reflect the number of players that are currently in the game.
    /// </summary>
    public static int PlayerCount => Players.Count;
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
    public static List<PlayerInput> PlayerInputs => new (PlayerInput.all);
    
    /// <summary>
    /// Gets the first PlayerInput instance if it exists; otherwise returns null.
    /// </summary>
    public static PlayerInput PlayerOneInput => PlayerInputs[0];
    
    /// <summary>
    /// Gets the second PlayerInput instance if it exists; otherwise returns null.
    /// </summary>
    public static PlayerInput PlayerTwoInput => PlayerInputs[1];
    #endregion
    
    void Awake()
    {
        // Clear the players list on awake to prevent duplicate players,
        // as well as due to the nature of Enter Playmode Options not clearing static variables on restart.
        Players.Clear();
    }

    #region Utility
    public static void AddPlayer(PlayerController player) => Players.Add(player);
    public static void RemovePlayer(PlayerController player) => Players.Remove(player);
    public static void ChangePlayerColor(PlayerController player, Color newColor) => player.GetComponentInChildren<MeshRenderer>().material.color = newColor;
    public static void SetPlayerSpawnPoint(PlayerController player, Vector2 newSpawnPoint) => player.transform.position = newSpawnPoint;

    public static void SetPlayerHealthbar(PlayerController player, int playerID)
    {
        // Dictionary that maps the player's ID to the healthbar's tag and name.
        // The tag is used to find the healthbar in the scene, and the name is used to set the healthbar's name.
        Dictionary<int, (string tag, string name)> idMapping = new()
        { { 1, ("[Healthbar] Left", "Healthbar (Player 1) (Left)") },
          { 2, ("[Healthbar] Right", "Healthbar (Player 2) (Right)") } };

        // If the player's ID is not in the dictionary, return.
        if (!idMapping.ContainsKey(playerID)) return;

        // Get the tag and name of the healthbar.
        (string tag, string name) = idMapping[playerID];

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
    
    public static void SetPlayerInput(PlayerController player, PlayerInput playerInput) => player.PlayerInput = playerInput;
    #endregion
}