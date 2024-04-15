#region
using System.Collections;
using System.Collections.Generic;
using Lumina.Essentials.Sequencer;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
#endregion

public class RoundManager : MonoBehaviour
{
    [Tab("Rounds")]
    [SerializeField] int maxRounds;
    [Space(10)]
    [SerializeField] SerializedDictionary<string, int> playerScores = new()
    {
        {"Player 1", 0},
        {"Player 2", 0}
    };
    
    [Tab("Checkpoints")]
    [Header("Player 1")]
    [SerializeField] List<Image> p1Checkpoints;
    
    [Header("Player 2")]
    [SerializeField] List<Image> p2Checkpoints;
    
    public delegate void RoundEnded();
    public static event RoundEnded OnRoundEnded;
    
    static int currentRound;
    static int player1WonRounds;
    static int player2WonRounds;


    // -- Properties --

    public static int CurrentRound
    {
        get
        {
            currentRound = Mathf.Clamp(currentRound, 1, 5);
            return currentRound;
        }
        private set => currentRound = value;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => HealthbarManager.Healthbars.Count == 2);
        
        // Subscribe the incremendround method to each healthbar's OnPlayerDeath event.
        foreach (Healthbar healthbar in FindObjectsOfType<Healthbar>()) { healthbar.OnPlayerDeath += IncrementRound; }

        OnRoundEnded += UpdateCheckpoints;
        
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
            
            UpdateCheckpoints();
        }
        
        if (CurrentRound == maxRounds || player1WonRounds == 3 || player2WonRounds == 3)
        {
            // End the game.
            // Display the winner.
            // Return to the main menu.
            
            ResultScreen resultScreen = FindObjectOfType<ResultScreen>(true);
            PlayerController winningPlayer = playerScores["Player 1"] > playerScores["Player 2"] ? PlayerManager.PlayerOne : PlayerManager.PlayerTwo;
            StartCoroutine(resultScreen.EnableResultScreen(winningPlayer));
        }
    }

    void OnDestroy()
    {
        foreach (Healthbar healthbar in HealthbarManager.Healthbars)
        {
            healthbar.OnPlayerDeath -= IncrementRound;
        }
        
        OnRoundEnded -= UpdateCheckpoints;
    }

    public void IncrementRound(PlayerController playerThatDied)
    {
        OnRoundEnded?.Invoke();
        CurrentRound++;
     
        // Increment the score of the player that didn't die.
        if (playerThatDied == PlayerManager.PlayerOne)
        {
            playerScores["Player 2"]++;
            player2WonRounds++;
        }
        else
        {
            playerScores["Player 1"]++;
            player1WonRounds++;
        }
        
        SceneManagerExtended.ReloadScene(3f);
    }
    
    void UpdateCheckpoints()
    {
        for (int i = 0; i < playerScores["Player 1"]; i++)
        {
            p1Checkpoints[i].gameObject.SetActive(true);
        }
        
        for (int i = 0; i < playerScores["Player 2"]; i++)
        {
            p2Checkpoints[i].gameObject.SetActive(true);
        }
    }

    void Reset()
    {
        SceneManagerExtended.PreviousScene = SceneManagerExtended.Intro;
        
        CurrentRound = 1;
        maxRounds = 5;
        player1WonRounds = 0;
        player2WonRounds = 0;
        
        playerScores["Player 1"] = 0;
        playerScores["Player 2"] = 0;
        
        UpdateCheckpoints();
    }

    public void ReloadScene()
    {
        Reset();
        
        GameManager manager = FindObjectOfType<GameManager>();
        manager.FadeOutMusic(1f);

        var sequence = new Sequence(this);
        sequence.WaitThenExecute(1f, () => GameObject.FindWithTag("ReloadTransition").GetComponent<TransitionAnimator>().enabled = true);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        playerScores["Player 1"] = Mathf.Clamp(playerScores["Player 1"], 0, maxRounds);
        playerScores["Player 2"] = Mathf.Clamp(playerScores["Player 2"], 0, maxRounds);
    }
#endif
}
