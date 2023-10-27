#region
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#endregion

public class RoundManager : MonoBehaviour
{
    [SerializeField] int maxRounds;
    [SerializeField] int currentRounds;
    [SerializeField] int player1WonRounds;
    [SerializeField] int player2WonRounds;

    [SerializeField] TMP_Text player1WonRoundsText;
    [SerializeField] TMP_Text player2WonRoundsText;

    RoundTimer timer;

    // In Awake method, we initiate the timer and clear player round text fields
    void Awake()
    {
        timer = FindObjectOfType<RoundTimer>();

        // Clear the text fields
        player1WonRoundsText = null;
        player2WonRoundsText = null;
    }

    // The Update method checks for updates in each frame of the game
    void Update() => CheckRoundStatus();

    // CheckRoundStatus checks if rounds have exceeded maximum, or if players have won 2 rounds, then reset
    // Also checks if players healthbars and updates rounds accordingly 
    // Z and X keys are debug keys to manually increment each player's victories
    void CheckRoundStatus()
    {
        // Reset game if rounds exceeded maximum, or if a player won 2 rounds
        if (currentRounds > maxRounds || player1WonRounds >= 2 || player2WonRounds >= 2) ResetGame();

        if (PlayerManager.PlayerOne == null || PlayerManager.PlayerTwo == null) return;
        
        var playerOneWon = PlayerManager.PlayerOne.Healthbar.Value > PlayerManager.PlayerTwo.Healthbar.Value;
        var playerTwoWon = PlayerManager.PlayerTwo.Healthbar.Value > PlayerManager.PlayerOne.Healthbar.Value;
        
        // Comparing player healthbars and incrementing the player victories accordingly
        if (timer.Finished && currentRounds <= maxRounds)
        {
            if (playerOneWon) PlayerVictory(ref player1WonRounds, player1WonRoundsText);
            else if (playerTwoWon) PlayerVictory(ref player2WonRounds, player2WonRoundsText);
        }

        // Press Z to manually increment Player 1 victories
        if (Input.GetKeyDown(KeyCode.Z)) PlayerVictory(ref player1WonRounds, player1WonRoundsText);

        // Press X to manually increment Player 2 victories
        if (Input.GetKeyDown(KeyCode.X)) PlayerVictory(ref player2WonRounds, player2WonRoundsText);
    }

    // PlayerVictory increments the victories of a player and increments current rounds
    void PlayerVictory(ref int playerWonRounds, TMP_Text playerWonRoundsText)
    {
        playerWonRounds++;
        currentRounds++;
        playerWonRoundsText.text = $"Rounds won: \n{playerWonRounds}/2";
    }

    // ResetGame resets the game status and empties the round text
    void ResetGame()
    {
        currentRounds             = 0;
        player1WonRounds          = 0;
        player2WonRounds          = 0;
        player1WonRoundsText.text = string.Empty;
        player2WonRoundsText.text = string.Empty;
    }
}
