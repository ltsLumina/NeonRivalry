using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : SingletonPersistent<InputDeviceManager>
{
    readonly Dictionary<InputDevice, int> playerDevices = new ();
    readonly List<string> controlSchemes = new () { "Keyboard", "(Co-op) Keyboard", "Gamepad" };

    
    // -- Cached References --
    PlayerInputManager inputManager;

    protected override void Awake()
    {
        inputManager = FindObjectOfType<PlayerInputManager>(); 
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            OnJoinPlayer();
        }
    }

    void OnJoinPlayer()
    {
        InputDevice device = GetActiveDevice();

        // Ignore if a device isn't active or is already associated with a player.
        if (device == null || playerDevices.ContainsKey(device)) return;

        // Only allow up to 3 players.
        if (inputManager.playerCount >= 3) return;

        playerDevices[device] = inputManager.playerCount;

        string controlScheme = controlSchemes[playerDevices[device]];
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
