using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Lumina.Debugging.Logger;

public class InputDeviceManager : MonoBehaviour
{
    [SerializeField] GameObject menuNavigator;
    [SerializeField] GameObject shelbyPrefab;
    [SerializeField] GameObject dorathyPrefab;
    
    readonly static Dictionary<InputDevice, int> persistentPlayerDevices = new (); // TODO: There is a chance that this doesn't work in builds.
    readonly Dictionary<InputDevice, int> playerDevices = new();
    
    public delegate void PlayerJoin();
    public static event PlayerJoin OnPlayerJoin;
    
    // -- Scenes --
    
    const int Intro = 0;
    const int MainMenu = 1;
    const int CharacterSelect = 2;
    const int Game = 3;

    void Awake()
    {
        // Clear the persistent devices if the scene is the Intro scene.
        // This is done to circumvent a bug where the persistent devices are not cleared when the game is restarted. (Due to Enter Playmode Options)
        if (SceneManagerExtended.ActiveScene is Intro) persistentPlayerDevices.Clear();
    }

    void Start() =>
        // Instantiate the players into the scene if there are persistent devices from previous scenes.
        LoadPersistentPlayers();

    /// <summary>
    /// Loads in the players that were registered on previous scenes.
    /// This method is used in most situations to load in the players.
    /// <para></para>
    /// <seealso cref="TryJoinPlayer"/>
    /// </summary>
    void LoadPersistentPlayers() // Note: Which character is instantiated is currently a work in progress.
    {
        //Load the devices that were registered on previous scenes.
        foreach (KeyValuePair<InputDevice, int> kvp in persistentPlayerDevices)
        {
            // Add the device to the playerDevices dictionary.
            playerDevices[kvp.Key] = kvp.Value;

            string     controlScheme = kvp.Key is Keyboard ? "Keyboard" : "Gamepad";
            GameObject prefabToInstantiate;

            switch (CharacterSelector.GetSelectedCharacter(kvp.Value))
            {
                case "Shelby":
                    prefabToInstantiate = shelbyPrefab;
                    break;

                case "Dorathy":
                    prefabToInstantiate = dorathyPrefab;
                    break;

                default:
                    prefabToInstantiate = menuNavigator;
                    break;
            }

            var player = PlayerInput.Instantiate(prefabToInstantiate, kvp.Value, controlScheme, -1, kvp.Key);
            Debug.Log($"Player {kvp.Value + 1} joined using {controlScheme} control scheme! \nThis player was loaded in from a previous scene or game restart.", player);

            OnPlayerJoin?.Invoke();
        }
    }

    void Update()
    {
        if (PlayerInputManager.instance.playerCount >= 2) return;

        // If the player is allowed to join, instantiate the player.
        // Tries to join the player every frame. Returns if the player is not allowed to join.
        TryJoinPlayer();
    }

    /// <summary>
    ///    Attempts to join a player to the game, after checking if they are allowed to.
    ///    Players may not be allowed to join in certain scenes for instance, the Game scene.
    ///    Persistent players are loaded in from previous scenes such as the Intro and Character Select scenes.
    /// </summary>
    void TryJoinPlayer()
    {
        if (!CheckCanJoin()) return;

        // If all checks pass, instantiate the player.
        InstantiatePlayer();
    }

    /// <summary>
    ///     Checks if the player is allowed to join the game.
    /// </summary>
    bool CheckCanJoin()
    {
        InputDevice activeDevice = GetActiveDevice();

        // Get the 'allowedScenes' list and check if the current scene allows player to join.
        List<int> allowedScenes = GetAllowedScenes();

        if (!allowedScenes.Contains(SceneManagerExtended.ActiveScene)) return false;

        // Check if any control button was pressed this frame, if not then return false.
        if (!Keyboard.current.anyKey.wasPressedThisFrame && (Gamepad.current == null || !Gamepad.current.AnyButtonDown())) return false;

        // Scene-specific logic to control player join rules.
        int  activeSceneIndex  = allowedScenes.IndexOf(SceneManagerExtended.ActiveScene);
        bool isIntroOrMainMenu = activeSceneIndex < CharacterSelect;
        bool isCharSelect      = activeSceneIndex == CharacterSelect;

        // 'Intro' or 'MainMenu' scenes where only one player can exist.
        if (isIntroOrMainMenu && PlayerInputManager.instance.playerCount >= 1 && !playerDevices.ContainsKey(activeDevice))
        {
            Debug.LogWarning("Only one player can join in the Intro and Main Menu scenes.");
            return false;
        }
        
        // 'Character Select' scene where max of two players can exist.
        if (isCharSelect && PlayerInputManager.instance.playerCount >= 2)
        {
            Debug.LogWarning("There can be a maximum of two players!");
            return false;
        }

        return true;
    }

    /// <summary>
    ///    Instantiates a Menu Navigator or Player.
    /// </summary>
    void InstantiatePlayer()
    {
        InputDevice device = GetActiveDevice();
        if (device == null || playerDevices.ContainsKey(device) || PlayerInputManager.instance.playerCount >= 2) return;
        
        playerDevices[device] = PlayerInputManager.instance.playerCount;
        //playerDevices.Add(device, PlayerInputManager.instance.playerCount);
        // Also update the device to be persistent in next scenes
        persistentPlayerDevices.Add(device, PlayerInputManager.instance.playerCount);

        string controlScheme = device is Keyboard ? "Keyboard" : "Gamepad";

        // TODO: TEMPORARY
        if (SceneManagerExtended.ActiveScene == Game) PlayerInput.Instantiate(shelbyPrefab, playerDevices[device], controlScheme, -1, device);
        
        // original
        PlayerInput.Instantiate(menuNavigator, playerDevices[device], controlScheme, -1, device);

        // Uses the default (recommended) rumble amount and duration.
        if (device is Gamepad gamepad) gamepad.Rumble(this); // bug: The rumble might be too weak on some gamepads, making it nearly unnoticeable.

        Debug.Log($"Player {playerDevices[device] + 1} joined using {controlScheme} control scheme!" + "\n");
        OnPlayerJoin?.Invoke();
    }

    static InputDevice GetActiveDevice()
    {
        if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame) return Keyboard.current;
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame) return Gamepad.current;
        return null;
    }

    // Hopefully this is never used because it just causes issues.
    // Let's just not remove players once they're added.
    public void OnPlayerLeave()
    {
        int playerIndex = playerDevices.LastOrDefault().Value;
        
        // If the player is not in the game, return.
        (InputDevice device, _) = playerDevices.FirstOrDefault(p => p.Value == playerIndex);
        if (device == null) return;

        // Remove the player from the game.
        playerDevices.Remove(device);
        
        // Also remove the device from the persistent list
        persistentPlayerDevices.Remove(device);

        // If the player is not in the game scene, return.
        string controlScheme = device is Keyboard ? "Keyboard" : "Gamepad";
        Debug.Log($"Player {playerIndex + 1} left the game using {controlScheme} control scheme!");
    }

    #region Utility
    static List<int> GetAllowedScenes()
    {
        List<int> allowedScenes = new ()
        { Intro, MainMenu, CharacterSelect };

        // If debug mode is active, allow the player to join directly into the game scene as well.
        if (Logger.DebugMode) allowedScenes.Add(Game);
        return allowedScenes;
    }
    #endregion
}