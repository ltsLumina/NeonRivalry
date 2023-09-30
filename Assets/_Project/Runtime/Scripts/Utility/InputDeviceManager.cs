using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    // The key is the device and the value is the player index
    readonly Dictionary<InputDevice, int> playerDevices = new ();
    
    // -- Properties --
    
    // Returns the list of devices that are currently associated with a player
    public Dictionary<InputDevice, int> PlayerDevices => playerDevices;
    
    // Returns the list of PlayerInput components that are currently associated with a player
    public List<PlayerInput> PlayerInputs => playerDevices.Keys.Select(device => PlayerInput.all.FirstOrDefault(p => p.devices.Contains(device))).ToList();
    
    // -- Cached References --
    PlayerInputManager inputManager;

   void Awake()
    {
        inputManager = FindObjectOfType<PlayerInputManager>(); 
    }

    void Update()
    {
        // Check if a player is not currently in the prompt to switch control scheme.
        if (!DeviceSwitchPrompt.IsWaitingForInput)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
            {
                JoinPlayerConfig();
            }
        }
    }

    void JoinPlayerConfig()
    {
        // Get the currently active device
        InputDevice device = GetActiveDevice();

        // If no active device or if the device is already associated with a player, ignore the join request
        if (device == null || playerDevices.ContainsKey(device)) return;

        // Ignore the join request if there are already 2 players
        if (inputManager.playerCount >= 2) return;

        // At this point, the device is not associated with any player and there are less than 2 players
        // Therefore, associate the device with the current player count and join the player
        playerDevices[device] = inputManager.playerCount;

        // Determine control scheme based on type of device
        string controlScheme = device is Keyboard ? "Keyboard" : "Gamepad";

        // Set Control Scheme
        inputManager.JoinPlayer(playerDevices[device], -1, controlScheme);

        Debug.Log($"Player {playerDevices[device] + 1} joined using {controlScheme} control scheme!");
    }

    public void JoinPlayerCharacter(GameObject playerPrefab)
    {
        // Get the currently active device
        InputDevice device = playerDevices.FirstOrDefault(p => p.Value == inputManager.playerCount).Key;

        // If no active device or if the device is already associated with a player, ignore the join request
        if (device == null || playerDevices.ContainsKey(device)) return;

        // Ignore the join request if there are already 2 players
        if (inputManager.playerCount >= 2) return;

        // At this point, the device is not associated with any player and there are less than 2 players
        // Therefore, associate the device with the current player count and join the player
        playerDevices[device] = inputManager.playerCount;

        // Determine control scheme based on type of device
        string controlScheme = device is Keyboard ? "Keyboard" : "Gamepad";

        // Set Control Scheme
        //inputManager.JoinPlayer(playerDevices[device], -1, controlScheme, device);
        PlayerInput.Instantiate(playerPrefab, playerDevices[device], controlScheme, -1, device);

        Debug.Log($"Player {playerDevices[device] + 1} joined using {controlScheme} control scheme!");
    }

    static InputDevice GetActiveDevice()
    {
        if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame) return Keyboard.current;

        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame) return Gamepad.current;

        return null;
    }

    public void OnPlayerLeft(int playerIndex)
    {
        // Get the device that was associated with the player
        InputDevice device = playerDevices.FirstOrDefault(p => p.Value == playerIndex).Key;

        // If no device was associated with the player, ignore the leave request
        if (device == null) return;

        // At this point, the device is associated with the player, so remove it from the dictionary
        playerDevices.Remove(device);

        Debug.Log($"Player {playerIndex +1} left the game!");
    }
}
