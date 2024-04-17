#region
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Linq;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SceneManagerExtended;
using Sequence = Lumina.Essentials.Sequencer.Sequence;
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

    Sound acceptSFX;

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

#if UNITY_EDITOR
    void OnEnable() => EditorApplication.playModeStateChanged += PlayModeState;
    void OnDisable() => EditorApplication.playModeStateChanged -= PlayModeState;

    static void PlayModeState(PlayModeStateChange state)
    { // Repaint the window when entering play mode.
        if (state == PlayModeStateChange.ExitingPlayMode) GamepadExtensions.StopAllRumble();
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
        InitializeMusic();
    }

    static void InitializeStateByScene()
    {
        // Things to do when any scene is loaded:
        MenuManager.LoadSettings(); // Also loads volumes.
        
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case var index when index == Intro:
                SetState(GameState.Intro);
                break;

            case var mainMenu when mainMenu == MainMenu:
                SetState(GameState.MainMenu);
                break;
            
            case var charSelect when charSelect == CharacterSelect:
                SetState(GameState.CharSelect);
                break;
            
            case var game when game == Game || game == Bar || game == Street:
                SetState(GameState.Game);

                // Play timeline.
                TimelinePlayer.Play();
                break;
        }
    }

    void InitializeMusic()
    {
        acceptSFX = new (SFX.Accept);
        acceptSFX.SetOutput(Output.SFX);

        Music streetMusic = new (Track.TheAlarmist);
        Music barMusic = new (Track.ThisIBelieve);

        switch (ActiveScene)
        {
            case var mainMenu when mainMenu == MainMenu:
                // Play Main Menu music
                Music mainMenuMusic = new (Track.MainMenu);
                mainMenuMusic.SetOutput(Output.Music).SetVolume(.5f);
                mainMenuMusic.Play();
                break;

            case var charSelect when charSelect == CharacterSelect:
                Music charSelectMusic = new (Track.CharSelect);
                charSelectMusic.SetOutput(Output.Music).SetVolume(.35f);
                charSelectMusic.Play();
                break;

            case var game when game == Bar:
                barMusic.SetOutput(Output.Music).SetVolume(1f);
                streetMusic.SetOutput(Output.Music).SetVolume(1f);
                
                if (barMusic.Playing || streetMusic.Playing) return;
                if (barMusic.Paused || streetMusic.Paused)
                {
                    barMusic.Resume();
                    streetMusic.Resume();
                    return;
                }
                
                // choose one at random
                if (UnityEngine.Random.value > 0.5f) barMusic.Play();
                else streetMusic.Play();

                break;

            case var game when game == Street:
                barMusic.SetOutput(Output.Music).SetVolume(1f);
                streetMusic.SetOutput(Output.Music).SetVolume(1f);

                if (barMusic.Playing || streetMusic.Playing) return;

                if (barMusic.Paused || streetMusic.Paused)
                {
                    barMusic.Resume();
                    streetMusic.Resume();
                    return;
                }

                // choose one at random
                if (UnityEngine.Random.value > 0.5f) barMusic.Play();
                else streetMusic.Play();
                break;
        }
    }

    public static void SetState(GameState state)
    {
        State = state;
        OnGameStateChanged?.Invoke(state);
    }

    public void SubmitSFX() => acceptSFX.Play();

    public void FadeOutMusic(float duration = 1.5f)
    {
        AudioManager.StopAll(duration);
    }

    public void LoadMainMenu()
    {
        FadeOutMusic();
        
        var sequence = new Sequence(this);
        sequence.WaitThenExecute
        (1f, () =>
        {
            LoadScene(MainMenu);
        });
    }

    public void LoadBar()
    {
        FadeOutMusic();
        
        var sequence = new Sequence(this);
        sequence.WaitThenExecute
        (1.5f, () =>
        {
            LoadScene(Bar);
        });
    }
    
    public void LoadStreet()
    {
        FadeOutMusic();
        
        var sequence = new Sequence(this);
        sequence.WaitThenExecute
        (1.5f, () =>
        {
            LoadScene(Street);
        });
    }

    public void ReloadScene()
    {
        FadeOutMusic();
        
        var sequence = new Sequence(this);
        sequence.WaitThenExecute(1.5f, () => GameObject.FindWithTag("ReloadTransition").GetComponent<TransitionAnimator>().enabled = true);
    }
    
    public void QuitGame()
    {
        FadeOutMusic();
        
        var sequence = new Sequence(this);
        sequence.WaitThenExecute
        (1.5f, () =>
        {
            Application.Quit();
            Debug.LogWarning("Usually this would quit the game, but you're in the editor.");
        });
    }
    
    void Update()
    {
#if !UNITY_EDITOR // Enable/show the mouse if the developer console pops up (due to an error being thrown in a build)
        if (Debug.developerConsoleVisible)
        {
            Cursor.visible   = true;
            Cursor.lockState = CursorLockMode.None;
        }
#endif
        
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
        if (TimelinePlayer.IsPlaying) return;
        
        IsPaused = !IsPaused;
        SetState(IsPaused ? GameState.Paused : GameState.Game);

        foreach (var player in PlayerManager.Players)
        {
            if (player == null) continue;
            player.DisablePlayer(IsPaused);
        }

        if (IsPaused)
        {
            PausingPlayer = playerThatPaused; 
            Debug.Log($"Player {PausingPlayer.PlayerID} paused the game.");
            var menuManager = FindObjectOfType<MenuManager>();
            menuManager.PauseTitle.text = $"Paused (Player {PausingPlayer.PlayerID})";
            
            // Disable the healthbars parent
            PausingPlayer.Healthbar.transform.parent.gameObject.SetActive(false);

            PausingPlayer.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(menuManager.showEffectsToggle.gameObject);
            var otherPlayer = PlayerManager.OtherPlayer(PausingPlayer);
            if (otherPlayer != null) otherPlayer.GetComponentInChildren<InputSystemUIInputModule>().enabled = !IsPaused;
        }
        else // Game is unpaused
        {
            PausingPlayer.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(null);
            var otherPlayer = PlayerManager.OtherPlayer(PausingPlayer);
            if (otherPlayer != null) otherPlayer.GetComponentInChildren<InputSystemUIInputModule>().enabled = !IsPaused;

            PausingPlayer.Healthbar.transform.parent.gameObject.SetActive(true);
            
            PausingPlayer = null;
        }
    }

    public void ScaleUpButton(Button button) => button.transform.DOScale(2.1f, 0.5f).SetEase(Ease.OutBack);
    public void ScaleDownButton(Button button) => button.transform.DOScale(2, 0.5f).SetEase(Ease.InBack);

    public static bool IsCurrentState(params GameState[] states) => states.Contains(State);
}
