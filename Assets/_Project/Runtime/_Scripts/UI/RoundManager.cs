#region
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

    public bool player1Victory;

    void Start()
    {
        // Clear the text fields
        //player1WonRoundsText = null;
        //player2WonRoundsText = null;
    }

    void OnEnable()
    {
        // TODO: Wont work anymore
        foreach (var healthbar in HealthbarManager.Healthbars) { Healthbar.OnPlayerDeath += CheckRoundStatus; }
    }

    void OnDisable()
    {
        // TODO: Wont work anymore
        foreach (var healthbar in HealthbarManager.Healthbars) { Healthbar.OnPlayerDeath -= CheckRoundStatus; }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) PlayerVictory(ref player1WonRounds, player1WonRoundsText);

        // Press X to manually increment Player 2 victories
        if (Input.GetKeyDown(KeyCode.X)) PlayerVictory(ref player2WonRounds, player2WonRoundsText);
        if (currentRounds > maxRounds || player1WonRounds >= 2 || player2WonRounds >= 2)
        {
            ResetGame();
            return;
        }
    }

    // The Update method checks for updates in each frame of the game

    // CheckRoundStatus checks if rounds have exceeded maximum, or if players have won 2 rounds, then reset
    // Also checks if players healthbars and updates rounds accordingly 
    // Z and X keys are debug keys to manually increment each player's victories
    void CheckRoundStatus(PlayerController playerThatDied)
    {
        // Reset game if rounds exceeded maximum, or if a player won 2 rounds
        if (currentRounds >= maxRounds || player1WonRounds >= 2 || player2WonRounds >= 2)
        {
            ResetGame();
            return;
        }

        if (playerThatDied      == PlayerManager.PlayerOne && PlayerManager.PlayerTwo != null && PlayerManager.PlayerTwo.Healthbar.Value > 0) { PlayerVictory(ref player2WonRounds, player2WonRoundsText); }
        else if (playerThatDied == PlayerManager.PlayerTwo && PlayerManager.PlayerOne != null && PlayerManager.PlayerOne.Healthbar.Value > 0) { PlayerVictory(ref player1WonRounds, player1WonRoundsText); }

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
        
        Debug.Log($"Player has won {playerWonRounds} rounds!");
    }

    // ResetGame resets the game status and empties the round text
    void ResetGame()
    {
        currentRounds             = 1;
        player1WonRounds          = 0;
        player2WonRounds          = 0;
        player1WonRoundsText.text = string.Empty;
        player2WonRoundsText.text = string.Empty;
    }
}
