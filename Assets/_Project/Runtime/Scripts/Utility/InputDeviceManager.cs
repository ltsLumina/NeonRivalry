using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : SingletonPersistent<InputDeviceManager>
{
    readonly Dictionary<InputDevice, int> playerDevices = new ();
    readonly List<string> controlSchemes = new () { "Keyboard", "Gamepad" };

    
    // -- Cached References --
    PlayerInputManager inputManager;

    protected override void Awake()
    {
        inputManager = FindObjectOfType<PlayerInputManager>(); 
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            OnJoinPlayer();
        }
    }

    void OnJoinPlayer()
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
        inputManager.JoinPlayer(playerDevices[device], -1, controlScheme, device);

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
        var item = playerDevices.FirstOrDefault(k => k.Value == playerIndex - 1);

        if (item.Key != null)
        {
            playerDevices.Remove(item.Key);
            Debug.Log($"Player {playerIndex} left!");
        }
    }
}
