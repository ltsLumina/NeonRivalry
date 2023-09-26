using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    PlayerInputManager inputManager;
    Dictionary<int, InputDevice> playerDevices = new ();
    List<string> controlSchemes = new () { "Keyboard", "(Co-op) Keyboard", "Gamepad" };

    void Awake()
    {
        inputManager = FindObjectOfType<PlayerInputManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Gamepad.current.startButton.wasPressedThisFrame)
        {
            OnJoinPlayer();
        }
    }

    void OnJoinPlayer()
    {
        // For keyboard.
        if (Keyboard.current.wasUpdatedThisFrame && inputManager.playerCount < 3)
        {
            InputDevice device = Keyboard.current;

            int playerIndex = inputManager.playerCount;

            if (playerIndex > 1)
            {
                if (playerDevices.ContainsValue(device)) return;
            }

            playerDevices[playerIndex] = device;

            string controlScheme = controlSchemes[playerIndex];
            
            inputManager.JoinPlayer(playerIndex, -1, controlScheme, device);

            Debug.Log($"Player {playerIndex} joined using {controlScheme}");
        }
        
        // For Gamepad.
        if (Gamepad.current == null) return;
        {
            if (!Gamepad.current.wasUpdatedThisFrame || inputManager.playerCount >= 2) return;
            InputDevice device = Gamepad.current;

            int playerIndex = inputManager.playerCount;

            if (playerIndex > 1)
            {
                if (playerDevices.ContainsValue(device)) return;
            }

            playerDevices[playerIndex] = device;
            
            string controlScheme = controlSchemes[2]; // Will always be Gamepad.

            inputManager.JoinPlayer(playerIndex, -1, controlScheme, device);

            Debug.Log($"Player {playerIndex} joined using {controlScheme}");
        }
    }

    public void OnPlayerLeave()
    {
        InputDevice device = Keyboard.current;

        int playerIndex = inputManager.playerCount;

        if (playerIndex > -1)
        {
            if (playerDevices.ContainsValue(device))
            {
                playerDevices.Remove(playerIndex);
                Debug.Log($"Player {playerIndex} left");
            }
        }
    }
}