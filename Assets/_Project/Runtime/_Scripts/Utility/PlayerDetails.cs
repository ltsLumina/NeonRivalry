using System;
using UnityEngine;

public partial class PlayerManager
{
    [Serializable]
    public struct PlayerDetails
    {
        [Space(10)]
        public PlayerColor playerColors;
        
        [Space(10)]
        public PlayerSpawnPoint playerSpawnPoints;
    }

    [Serializable]
    public struct PlayerColor
    {
        public Color playerOneColor;
        public Color playerTwoColor;
    }

    [Serializable]
    public struct PlayerSpawnPoint
    {
        public Vector2 playerOneSpawnPoint;
        public Vector2 playerTwoSpawnPoint;
    }
}
