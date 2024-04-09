#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using VInspector;
#endregion

public class RoundManager : MonoBehaviour
{
    [SerializeField] int maxRounds;
    [SerializeField, ReadOnly] int currentRound;
    [Space(10)]
    [SerializeField] SerializedDictionary<string, int> playerScores = new()
    {
        {"Player 1", 0},
        {"Player 2", 0}
    };
    
    public delegate void RoundEnded();
    public static event RoundEnded OnRoundEnded;
    
    static int player1WonRounds;
    static int player2WonRounds;

    // -- Properties --

    public int CurrentRound
    {
        get => currentRound;
        private set => currentRound = value;
    }

    void Start()
    {
        // If the previous scene was the character select scene, reset the round manager.
        // This means a new game is to be played.
        if (SceneManagerExtended.PreviousScene == SceneManagerExtended.CharacterSelect)
        {
            Reset();
        }
        else
        {
            playerScores["Player 1"] = player1WonRounds;
            playerScores["Player 2"] = player2WonRounds;
        }
    }

    void OnEnable() => HealthbarManager.OnAnyPlayerDeath += IncrementRound;
    void OnDisable() => HealthbarManager.OnAnyPlayerDeath -= IncrementRound;

    void IncrementRound(PlayerController playerThatDied)
    {
        OnRoundEnded?.Invoke();
        currentRound++;
     
        // Increment the score of the player that didn't die.
        if (playerThatDied == PlayerManager.PlayerOne)
        {
            playerScores["Player 2"]++;
            player1WonRounds++;
        }
        else
        {
            playerScores["Player 1"]++;
            player2WonRounds++;
        }
        
        // TODO: uncomment this line when the game is ready to be played. 
        //SceneManagerExtended.ReloadScene(2f);
    }

    void Reset()
    {
        currentRound = 1;
        player1WonRounds = 0;
        player2WonRounds = 0;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        playerScores["Player 1"] = Mathf.Clamp(playerScores["Player 1"], 0, maxRounds);
        playerScores["Player 2"] = Mathf.Clamp(playerScores["Player 2"], 0, maxRounds);
    }
#endif
}
