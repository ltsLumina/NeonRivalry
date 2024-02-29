#region
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

/*--------------------------------------
 Use Time.fixedDeltaTime instead of Time.deltaTime when working with the State Machine.
--------------------------------------*/
public class GameManager : MonoBehaviour
{
    // The target frame rate of the game. It is set to 60 FPS as fighting games typically run at 60 FPS.
    const int TARGET_FPS = 60;
    
    static GameState gameState;

    public enum GameState
    {
        Transitioning,
        Intro,
        MainMenu,
        CharSelect,
        Playing,
        Paused,
        GameOver
    }

    public static GameState State
    {
        get => gameState;
        private set // Ensures the game state is only set using the SetState method
        {
            // Get the calling method
            string callingMethod = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;

            // Check if the calling method is not SetState
            if (callingMethod != nameof(SetState)) Debug.LogWarning("Warning: The game state should ONLY be set using the SetState method.");

            gameState = value;
        }
    }

    public static bool IsPaused { get; private set; }
    public static PlayerController PausingPlayer { get; private set; }

    /// <summary> Event that is invoked when the game STATE changes. </summary>
    public static event Action<GameState> OnGameStateChanged;
    
    void Awake()
    {
        Application.targetFrameRate = TARGET_FPS;
        
        // Reset timescale if the game was paused when the scene was unloaded
        IsPaused = false;
        if (IsPaused) Time.timeScale = 1;

        InitializeStateByScene();
        
        OnGameStateChanged += HandleGameStateChanged;
    }

    void HandleGameStateChanged(GameState state)
    {
        // Not implemented yet.
        //Logger.Log("Game state changed to: " + state);
    }
    
    static void InitializeStateByScene()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Intro":
                SetState(GameState.Intro);
                break;
            
            case "MainMenu":
                SetState(GameState.MainMenu);
                break;
            
            case "CharacterSelect":
                SetState(GameState.CharSelect);
                break;
            
            case "Game":
                SetState(GameState.Playing);
                break;
        }
    }
    

    // void OnEnable() => Healthbar.OnPlayerDeath += HandlePlayerDeath;
    // void OnDisable() => Healthbar.OnPlayerDeath -= HandlePlayerDeath;
    //
    // static void HandlePlayerDeath(PlayerController player = default) => SetState(GameState.GameOver);

    public static void SetState(GameState state)
    {
        State = state;
        OnGameStateChanged?.Invoke(state);
    }
    
    void Update()
    {
        switch (State)
        {
            case GameState.Transitioning:
                break;
            
            case GameState.Intro:
                break;
            
            case GameState.MainMenu:
                break;
            
            case GameState.CharSelect:
                break;

            case GameState.Playing:
                // Allow the player to pause the game
                if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
                break;

            case GameState.Paused:
                // Allow the player to unpause the game
                if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
                break;

            case GameState.GameOver:

                // DEBUG:
                if (Input.GetKeyDown(KeyCode.Space)) SetState(GameState.Playing);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.LogWarning("Reloaded Scene with a debug key! (Backspace)");
            SceneManagerExtended.ReloadScene();
        }
        
        // If the user is in a build, pressing the escape key will quit the game.
        if (Input.GetKeyDown(KeyCode.Escape) && !Application.isEditor)
        {
            Application.Quit();
        }
    }

    public static void TogglePause()
    {
        IsPaused = !IsPaused;
        SetState(IsPaused ? GameState.Paused : GameState.Playing);

        var player = FindObjectOfType<PlayerController>();
        player.enabled = !IsPaused;

        var UIManager = FindObjectOfType<UIManager>();
        UIManager.PauseMenu.SetActive(IsPaused);
    }

    public static bool IsCurrentState(params GameState[] states) => states.Contains(State);
}
