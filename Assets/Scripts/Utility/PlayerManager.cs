using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The PlayerManager class is used to manage ALL players in the game including their settings, properties, and actions.
/// <seealso cref="PlayerController"/>
/// </summary>
public class PlayerManager : SingletonPersistent<PlayerManager>
{
    /// <summary> Player enum is used in a context where the <see cref="PlayerController.PlayerID"/> is needed. </summary>
    [Flags]
    public enum TargetPlayer
    {
        None = 0,
        PlayerOne = 1,
        PlayerTwo = 2,
        Both = PlayerOne | PlayerTwo
    }
    
    [Serializable]
    public struct PlayerColor
    {
        public Color playerOneColor;
        public Color playerTwoColor;
    }
    
    [Serializable]
    public struct PlayerSpawnPoints
    {
        public Vector2 playerOneSpawnPoint;
        public Vector2 playerTwoSpawnPoint;
    }
    
    [Serializable] //TODO: Implement this when the healthbar is done.
    public struct PlayerHealthbars
    {
        public Slider playerOneHealthbar;
        public Slider playerTwoHealthbar;
    }

    [Header("Player Settings")]
    [SerializeField] TargetPlayer targetPlayer;
    [Space(5)]
    [SerializeField] PlayerColor playerColor;
    [Space(5)] 
    [SerializeField] PlayerSpawnPoints playerSpawnPoints;
    [Space(5)]
    [SerializeField] PlayerHealthbars playerHealthbars;
    
    // -- Player Settings --
    public PlayerColor PlayerColors => playerColor;
    public PlayerSpawnPoints PlayerSpawns => playerSpawnPoints;
    
    // -- Properties --
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

    void OnValidate()
    {
        InvokePlayerActions(
            targetPlayer, 
            () => ChangePlayerColor(PlayerOne, playerColor.playerOneColor), 
            () => ChangePlayerColor(PlayerTwo, playerColor.playerTwoColor));
    }

    // -- Utility --

    /// <summary>
    ///     Invokes actions specified for players if conditions are met.
    /// </summary>
    /// <param name="targetPlayersToInvoke"> The target players to which the method invocation is desired.</param>
    /// <param name="playerOneAction"> The action that will be invoked for Player One if Player One is a valid player.</param>
    /// <param name="playerTwoAction"> The action that will be invoked for Player Two if Player Two is a valid player. </param>
    public static void InvokePlayerActions(TargetPlayer targetPlayersToInvoke, Action playerOneAction = null, Action playerTwoAction = null)
    {
        if (Players.Count <= 0) return;

        if (targetPlayersToInvoke.HasFlag(TargetPlayer.PlayerOne) && PlayerOne != null) playerOneAction?.Invoke();
        if (targetPlayersToInvoke.HasFlag(TargetPlayer.PlayerTwo) && PlayerTwo != null) playerTwoAction?.Invoke();
    }
    
    public static void AddPlayer(PlayerController player) => Players.Add(player);
    public static void RemovePlayer(PlayerController player) => Players.Remove(player);
    public static void ChangePlayerColor(Component player, Color newColor) => player.GetComponentInChildren<MeshRenderer>().material.color = newColor;
    public static void SetPlayerSpawnPoint(Component player, Vector2 newSpawnPoint)
    {
        player.transform.position = newSpawnPoint; 
        Debug.Log("New spawn point set: " + newSpawnPoint);
    }
}