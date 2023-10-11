using System.Collections.Generic;
using System.Linq;
using Lumina.Debugging;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    public readonly static Dictionary<InputDevice, int> persistentPlayerDevices = new (); // TODO: There is a chance that this doesn't work in builds.
    readonly Dictionary<InputDevice, int> playerDevices = new();
    
    PlayerInputManager playerInputManager;

    // -- Scenes --
    
    const int Intro = 0;
    const int MainMenu = 1;
    const int CharacterSelect = 2;
    const int Game = 3;

    void Awake() => playerInputManager = GetComponent<PlayerInputManager>();

    void Start()
    {
        // Clear the persistent devices if the scene is the Intro scene.
        // This is done to circumvent a bug where the persistent devices are not cleared when the game is restarted. (Due to Enter Playmode Options)
        if (SceneManagerExtended.ActiveScene is Intro) persistentPlayerDevices.Clear();

        // Instantiate the players into the game scene if there are persistent devices from previous scenes.
            LoadPersistentPlayers();
    }

    /// <summary>
    /// Loads in the players that were registered on previous scenes.
    /// This method is used in most situations to load in the players.
    /// <para></para>
    /// <seealso cref="OnPlayerJoin"/>
    /// </summary>
    void LoadPersistentPlayers()
    {
        //Load the devices that were registered on previous scenes.
        foreach(KeyValuePair<InputDevice, int> kvp in persistentPlayerDevices)
        {
            playerDevices[kvp.Key] = kvp.Value;
            
            string controlScheme = kvp.Key is Keyboard ? "Keyboard" : "Gamepad";
            playerInputManager.JoinPlayer(kvp.Value, -1, controlScheme, kvp.Key);

            Debug.Log($"Player {kvp.Value + 1} joined using {controlScheme} control scheme!");
        }
    }

    void Update()
    {
        if (playerInputManager.playerCount >= 2) return;

        // Handles players joining the game.
        OnPlayerJoin();
    }
    
    /// <summary>
    ///    Handles players joining the game, if they are allowed to.
    /// For instance, players are not allowed to join in the Game scene.
    /// <para></para>
    /// LoadPersistentPlayers loads in players if they have already joined in previous scenes.
    /// For instance the Intro and Character Select scenes.
    /// <seealso cref="LoadPersistentPlayers"/> 
    /// </summary>
    void OnPlayerJoin()
    {
        InputDevice activeDevice = GetActiveDevice();

        // Get the 'allowedScenes' list and check if the current scene allows player to join.
        List<int> allowedScenes = GetAllowedScenes(Intro, MainMenu, CharacterSelect, Game);
        if (!allowedScenes.Contains(SceneManagerExtended.ActiveScene)) return;

        // Check if any control button was pressed this frame, if not then return.
        if (!Keyboard.current.anyKey.wasPressedThisFrame && (Gamepad.current == null || !Gamepad.current.AnyButtonDown())) return;

        // Scene-specific logic to control player join rules.
        bool isIntroOrMainMenu = allowedScenes.IndexOf(SceneManagerExtended.ActiveScene) < CharacterSelect;
        bool isCharSelect      = allowedScenes.IndexOf(SceneManagerExtended.ActiveScene) == CharacterSelect;

        // 'Character Select' scene where max of two players can exist.
        if (isCharSelect && playerInputManager.playerCount >= 2)
        {
            Debug.LogWarning("There can be a maximum of two players!");
            return;
        }

        // 'Intro' or 'MainMenu' scenes where only one player can exist.
        if (isIntroOrMainMenu && playerInputManager.playerCount >= 1 && !playerDevices.ContainsKey(activeDevice))
        {
            Debug.LogWarning("Only one player can join in the Intro and Main Menu scenes.");
            return;
        }

        // If all checks pass, instantiate the player.
        InstantiatePlayer();
    }

    /// <summary>
    ///    Instantiates a Menu Navigator or Player into the game scene.
    /// </summary>
    
    // BUG:
    // Due to a weird bug with the PlayerInputManager, we instantiate a UI Navigator in the Intro, Main Menu, and Character Select scenes,
    // but in the Game scene we instantiate the player prefab.
    // The object to instantiate is selected in the PlayerInputManager component on the game object, but you must switch from "Join Manually" to "Join On Button Press" to be able to select it.
    // Once you have switched the option, you must select the player prefab and then switch back to "Join Manually".
    // Odd bug, but this is the only way I could think of to fix it.
    void InstantiatePlayer()
    {
        InputDevice device = GetActiveDevice();
        if (device == null || playerDevices.ContainsKey(device) || playerInputManager.playerCount >= 2) return;
        
        playerDevices[device] = playerInputManager.playerCount;
        // Also update the device to be persistent in next scenes
        persistentPlayerDevices[device] = playerInputManager.playerCount;

        string controlScheme = device is Keyboard ? "Keyboard" : "Gamepad";
        playerInputManager.JoinPlayer(playerDevices[device], -1, controlScheme, device);

        // Uses the default (recommended) rumble amount and duration.
        if (device is Gamepad gamepad) gamepad.Rumble(this); // bug: The rumble might be too weak on some gamepads.

        Debug.Log($"Player {playerDevices[device] + 1} joined using {controlScheme} control scheme!");
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
    static List<int> GetAllowedScenes(int Intro, int MainMenu, int CharacterSelect, int Game)
    {
        List<int> allowedScenes = new ()
        { Intro, MainMenu, CharacterSelect };

        // If debug mode is active, allow the player to join in the game scene as well.
        if (FGDebugger.debugMode) allowedScenes.Add(Game);
        return allowedScenes;
    }
    #endregion
}