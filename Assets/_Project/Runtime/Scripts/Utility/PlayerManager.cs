using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PlayerManager class is used to manage ALL players in the game including their settings, properties, and actions.
/// <seealso cref="PlayerController"/>
/// </summary>
public partial class PlayerManager : SingletonPersistent<PlayerManager>
{
    // -- Player Settings --
    
    [Header("Settings"), Space(5)]
    [SerializeField] PlayerDetails playerDetails;

    [Space(10)]

    [Header("Utility"), Space(5)]
    [SerializeField] int placeholder;
    
    public TargetPlayer TargetPlayerEnum => playerDetails.targetPlayer;
    public PlayerColor PlayerColors => playerDetails.playerColors;
    public PlayerSpawnPoints PlayerSpawns => playerDetails.playerSpawnPoints;
    
    // -- Player --
    public static List<PlayerController> Players { get; } = new ();

    /// <summary> PlayerOne/<see cref="PlayerTwo"/> properties are used in a context where the <see cref="PlayerController"/> component is needed. </summary>
    public static PlayerController PlayerOne => Players.Count >= 1 ? Players[0] : null; // This is the same as PlayerID == 1 ? Player : null;
    /// <summary> <see cref="PlayerOne"/>/Two properties are used in a context where the <see cref="PlayerController"/> component is needed. </summary>
    public static PlayerController PlayerTwo => Players.Count >= 2 ? Players[1] : null; // This is the same as PlayerID == 2 ? Player : null;
    
    protected override void Awake()
    { 
        base.Awake();
        
        Players.Clear();
    }

    void OnValidate() => InvokePlayerActions(
        TargetPlayerEnum, 
        () => ChangePlayerColor(PlayerOne, playerDetails.playerColors.playerOneColor), 
        () => ChangePlayerColor(PlayerTwo, playerDetails.playerColors.playerTwoColor));

    // -- Utility --

    #region Utility
    
    /// <summary>
    ///     Invokes actions specified for players if conditions are met.
    /// </summary>
    /// <param name="targetPlayersToInvoke"> The target players to which the method invocation is desired.</param>
    /// <param name="playerOneAction"> The action that will be invoked for Player One if Player One is a valid player.</param>
    /// <param name="playerTwoAction"> The action that will be invoked for Player Two if Player Two is a valid player. </param>
    static void InvokePlayerActions(TargetPlayer targetPlayersToInvoke, Action playerOneAction = null, Action playerTwoAction = null)
    {
        if (Players.Count <= 0) return;

        if (targetPlayersToInvoke.HasFlag(TargetPlayer.PlayerOne) && PlayerOne != null) playerOneAction?.Invoke();
        if (targetPlayersToInvoke.HasFlag(TargetPlayer.PlayerTwo) && PlayerTwo != null) playerTwoAction?.Invoke();
    }
    
    public static void AddPlayer(PlayerController player) => Players.Add(player);
    public static void RemovePlayer(PlayerController player) => Players.Remove(player);
    public static void ChangePlayerColor(PlayerController player, Color newColor) => player.GetComponentInChildren<MeshRenderer>().material.color = newColor;
    public static void SetPlayerSpawnPoint(PlayerController player, Vector2 newSpawnPoint) => player.transform.position = newSpawnPoint;

    public static void SetPlayerHealthbar(PlayerController player, int playerID)
    {
        var healthbarManager = FindObjectOfType<HealthbarManager>();

        // Set the player's healthbar depending on the player's ID. Player 1 is on the left, Player 2 is on the right.
        string tag;
        string name;

        switch (playerID)
        {
            case 1:
                tag  = "[Healthbar] Left";
                name = "Healthbar (Player 1) (Left)";
                break;

            case 2:
                tag  = "[Healthbar] Right";
                name = "Healthbar (Player 2) (Right)";
                break; 

            default:
                return;
        }

        var healthbar = GameObject.FindGameObjectWithTag(tag).GetComponent<Healthbar>();
        AssignHealthbar(player, healthbar, healthbarManager, name);
    }
    
    static void AssignHealthbar(PlayerController player, Healthbar healthbar, HealthbarManager healthbarManager, string name)
    {
        // Assign the healthbar to the healthbar manager
        healthbarManager.Healthbars.Add(healthbar);

        // Assign the healthbar to player.
        player.Healthbar = healthbar;

        // Set the healthbar's name.
        healthbar.name = name;

        // Assign the player to the healthbar.
        healthbar.Player = player;

        // Initialize the healthbar.
        healthbar.Initialize();
    }
    #endregion
}