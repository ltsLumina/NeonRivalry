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
    [DisableIf(nameof(disabled))]
    [SerializeField] SerializedDictionary<string, int> playerScores = new()
    {
        {"Player 1", 0},
        {"Player 2", 0}
    };

    #region VInspector make dictionary readonly.
#pragma warning disable CS0414 // Field is assigned but its value is never used
    bool disabled = true;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    #endregion
    
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

    void OnEnable() => HealthbarManager.OnPlayerDeath += IncrementRound;
    void OnDisable() => HealthbarManager.OnPlayerDeath -= IncrementRound;

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

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Z)) PlayerVictory(ref player1WonRounds, player1WonRoundsText);
    //
    //     // Press X to manually increment Player 2 victories
    //     if (Input.GetKeyDown(KeyCode.X)) PlayerVictory(ref player2WonRounds, player2WonRoundsText);
    //     if (currentRound > maxRounds || player1WonRounds >= 2 || player2WonRounds >= 2) { ResetGame(); }
    // }

    // The Update method checks for updates in each frame of the game

    // CheckRoundStatus checks if rounds have exceeded maximum, or if players have won 2 rounds, then reset
    // Also checks if players healthbars and updates rounds accordingly 
    // Z and X keys are debug keys to manually increment each player's victories
    // void CheckRoundStatus(PlayerController playerThatDied)
    // {
    //     // Reset game if rounds exceeded maximum, or if a player won 2 rounds
    //     if (currentRound >= maxRounds || player1WonRounds >= 2 || player2WonRounds >= 2)
    //     {
    //         ResetGame();
    //         return;
    //     }
    //
    //     if (playerThatDied      == PlayerManager.PlayerOne && PlayerManager.PlayerTwo != null && PlayerManager.PlayerTwo.Healthbar.Value > 0) PlayerVictory(ref player2WonRounds, player2WonRoundsText);
    //     else if (playerThatDied == PlayerManager.PlayerTwo && PlayerManager.PlayerOne != null && PlayerManager.PlayerOne.Healthbar.Value > 0) PlayerVictory(ref player1WonRounds, player1WonRoundsText);
    //
    //     // Press Z to manually increment Player 1 victories
    //     if (Input.GetKeyDown(KeyCode.Z)) PlayerVictory(ref player1WonRounds, player1WonRoundsText);
    //
    //     // Press X to manually increment Player 2 victories
    //     if (Input.GetKeyDown(KeyCode.X)) PlayerVictory(ref player2WonRounds, player2WonRoundsText);
    // }
    //
    // // PlayerVictory increments the victories of a player and increments current rounds
    // void PlayerVictory(ref int playerWonRounds, TMP_Text playerWonRoundsText)
    // {
    //     playerWonRounds++;
    //     currentRound++;
    //     playerWonRoundsText.text = $"Rounds won: \n{playerWonRounds}/2";
    //
    //     Debug.Log($"Player has won {playerWonRounds} rounds!");
    // }
    //
    // // ResetGame resets the game status and empties the round text
    // void ResetGame()
    // {
    //     currentRound             = 1;
    //     player1WonRounds          = 0;
    //     player2WonRounds          = 0;
    //     player1WonRoundsText.text = string.Empty;
    //     player2WonRoundsText.text = string.Empty;
    // }
}
