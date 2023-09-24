using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerManager
{
    [Serializable]
    public struct PlayerDetails
    {
        [Space(5)]
        public TargetPlayer targetPlayer;
        
        [Space(10)]
        public PlayerColor playerColors;
        
        [Space(10)]
        public PlayerSpawnPoints playerSpawnPoints;
        
        [Space(10)]
        public PlayerHealthbars playerHealthbars;
    }

    /// <summary> Player enum is used in a context where the <see cref="PlayerController.PlayerID"/> is needed. </summary>
    [Flags]
    public enum TargetPlayer
    {
        PlayerOne = 1,
        PlayerTwo = 2,
        [UsedImplicitly] // Used in the inspector through the "Everything" option.
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
}
