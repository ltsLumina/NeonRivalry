#region
using System;
using System.Linq;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using static SceneManagerExtended;
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
        Game,
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

#if !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void DisableCursor()
    {
        // Disable the cursor in a build
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
#endif
    
    void Awake()
    {
#if !UNITY_EDITOR
        DisableCursor();
#endif
        
        // Needs to be reset due to EnterPlaymodeOptions
        PlayerManager.Players.Clear();
        PlayerManager.MenuNavigators.Clear();
        
        Application.targetFrameRate = TARGET_FPS;
        
        // Reset timescale if the game was paused when the scene was unloaded
        IsPaused = false;
        if (IsPaused) Time.timeScale = 1;
    }

    void Start()
    {
        InitializeStateByScene();
    }

    static void InitializeStateByScene()
    {
        // Things to do when any scene is loaded:
        MenuManager.LoadSettings(); // Also loads audio.
        AudioManager.StopAll();
        
        
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case var index when index == Intro:
                SetState(GameState.Intro);
                break;

            case var mainMenu when mainMenu == MainMenu:
                SetState(GameState.MainMenu);
                
                // Play Main Menu music
                Music mainMenuMusic = new (Track.mainMenu);
                mainMenuMusic.SetOutput(Output.Music).SetVolume(1f);
                mainMenuMusic.Play();
                break;
            
            case var charSelect when charSelect == CharacterSelect:
                SetState(GameState.CharSelect);
                break;
            
            case var game when game == Game || game == Bar || game == Street:
                SetState(GameState.Game);

                // Play Game music
                Music gameMusic = new (Track.LoveTheSubhumanSelf);
                gameMusic.SetOutput(Output.Music).SetVolume(1f);
                gameMusic.Play(2f);

                // Play timeline.
                //TimelinePlayer.Play();
                break;
        }
    }

    public static void SetState(GameState state)
    {
        State = state;
        OnGameStateChanged?.Invoke(state);
    }
    
    void Update()
    {
        // Things to do regardless of state:
        
        // Enable/show the mouse if the developer console pops up (due to an error being thrown in a build)
        if (Debug.developerConsoleVisible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
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

            case GameState.Game:
                break;

            case GameState.Paused:
                break;

            case GameState.GameOver:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace) && Application.isEditor)
        {
            //Debug.LogWarning("Reloaded Scene with a debug key! (Backspace)");
            //SceneManagerExtended.ReloadScene();
        }
    }

    public static void TogglePause(PlayerController playerThatPaused)
    {
        IsPaused = !IsPaused;
        SetState(IsPaused ? GameState.Paused : GameState.Game);

        foreach (var player in PlayerManager.Players)
        {
            if (player == null) continue;
            player.DisablePlayer(IsPaused);
        }

        var UIManager = FindObjectOfType<UIManager>();
        UIManager.PauseMenu.SetActive(IsPaused);

        if (IsPaused)
        {
            PausingPlayer = playerThatPaused;
            UIManager.PauseMenuTitle.text = $"Paused (Player {PausingPlayer.PlayerID})";

            PausingPlayer.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(UIManager.PauseMenuButtons[0].gameObject);
            var otherPlayer = PlayerManager.OtherPlayer(PausingPlayer);
            if (otherPlayer != null) otherPlayer.PlayerInput.enabled = !IsPaused;
        }
        else
        {
            PausingPlayer.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(null);
            var otherPlayer = PlayerManager.OtherPlayer(PausingPlayer);
            if (otherPlayer != null) otherPlayer.PlayerInput.enabled = !IsPaused;

            PausingPlayer = null;
        }
    }

    public static bool IsCurrentState(params GameState[] states) => states.Contains(State);
}
