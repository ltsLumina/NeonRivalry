using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputDeviceManager : MonoBehaviour
{
    public static Dictionary<InputDevice, int> persistPlayerDevices = new (); // TODO: There is a chance that this doesn't work in builds.
    readonly Dictionary<InputDevice, int> playerDevices = new();
    
    PlayerInputManager playerInputManager;
   
    void Awake() 
    {     
        // Get the PlayerInputManager
        playerInputManager = GetComponent<PlayerInputManager>(); 
        if (playerInputManager == null) Debug.LogError("No PlayerInputManager found in the scene!");

        //Load the devices from previous scene
        foreach(KeyValuePair<InputDevice, int> kvp in persistPlayerDevices)
        {
            playerDevices[kvp.Key] = kvp.Value;
            
            string controlScheme = kvp.Key is Keyboard ? "Keyboard" : "Gamepad";
            playerInputManager.JoinPlayer(kvp.Value, -1, controlScheme, kvp.Key);

            Debug.Log($"Player {kvp.Value + 1} joined using {controlScheme} control scheme!");
        }
    }
   
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 3) //TODO: Make it so the player can only join in select scenes. Don't allow them to join at all, don't just debug "Player 2 is not supported in this scene."
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.AnyButton()))
            {
                JoinPlayerConfig();
            }
        } 
    }

    void JoinPlayerConfig()
    {
        InputDevice device = GetActiveDevice();
        if (device == null || playerDevices.ContainsKey(device) || playerInputManager.playerCount >= 2) return;
        
        playerDevices[device] = playerInputManager.playerCount;
        // Also update the device to be persistent in next scenes
        persistPlayerDevices[device] = playerInputManager.playerCount;

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

    public void OnPlayerLeft(int playerIndex)
    {
        InputDevice device = playerDevices.FirstOrDefault(p => p.Value == playerIndex).Key;
        if (device == null) return;

        playerDevices.Remove(device);
        // Also remove the device from the persistent list
        persistPlayerDevices.Remove(device);

        Debug.Log($"Player {playerIndex + 1} left the game!");
    }
}