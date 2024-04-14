#region
using System.Collections.Generic;
using System.Linq;
using Lumina.Essentials.Sequencer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endregion

public static class GamepadExtensions
{
    public static bool StartButtonPressed(this Gamepad gamepad) => gamepad != null && gamepad.startButton.wasPressedThisFrame;
    public static bool SelectButtonPressed(this Gamepad gamepad) => gamepad != null && gamepad.selectButton.wasPressedThisFrame;

    public static bool AnyButtonDown(this Gamepad gamepad)
    {
        // Create a list of all buttons on the gamepad
        List<ButtonControl> buttons = new()
        { gamepad.aButton,
          gamepad.bButton,
          gamepad.xButton,
          gamepad.yButton,
          gamepad.leftShoulder,
          gamepad.rightShoulder,
          gamepad.startButton,
          gamepad.selectButton,
          gamepad.leftStickButton,
          gamepad.rightStickButton };

        // Return true if any of the buttons are pressed
        return buttons.Any(button => button.wasPressedThisFrame);
    }

    /// <summary>
    ///     Rumbles the gamepad at specified frequencies for a specific duration.
    /// </summary>
    /// <param name="gamepad">The gamepad to rumble.</param>
    /// <param name="playerID"> The player ID to retrieve the rumble strength from.</param>
    /// <param name="ignorePreferences">Whether to ignore the rumble preferences for the player.</param>
    /// <param name="lowFrequency">The low (light) frequency for the rumble.</param>
    /// <param name="highFrequency">The high (heavy) frequency for the rumble, defaults to 0.25f.</param>
    /// <param name="duration">The duration for the rumble, defaults to 0.25 (float) seconds.</param>
    public static void Rumble(this Gamepad gamepad, int playerID, float lowFrequency = 0.25f, float highFrequency = 0.25f, float duration = 0.25f, bool ignorePreferences = false)
    {
        if (gamepad == null) return;

        if (!ignorePreferences)
        {
            // Retrieve the rumble strength for the player
            float rumbleStrength = playerID == 1 ? SettingsManager.Player1RumbleStrength : SettingsManager.Player2RumbleStrength;
            
            // Apply the rumble strength to the frequencies
            lowFrequency  *= rumbleStrength;
            highFrequency *= rumbleStrength;
        }

        lowFrequency  = LerpFrequency(lowFrequency);
        highFrequency = LerpFrequency(highFrequency);

        // Rumble the controller that joined for 'duration' time.
        MonoBehaviour host           = CoroutineHelper.GetHost();
        var           rumbleSequence = new Sequence(host);
        rumbleSequence.Execute(() => gamepad.SetMotorSpeeds(lowFrequency, highFrequency)).WaitThenExecute(duration, () => gamepad.SetMotorSpeeds(0, 0));

        return; // Local function

        float LerpFrequency(float frequency) => Mathf.Lerp(frequency, frequency * 2, duration);
    }

    public static void RumbleAll(bool ignorePreferences = false, float lowFrequency = 0.25f, float highFrequency = 0.25f, float duration = 0.25f)
    {
        if (Gamepad.all.Count == 0) return;

        // Get the gamepads for each player
        var player1Gamepad = InputDeviceManager.PlayerOneDevice as Gamepad;
        var player2Gamepad = InputDeviceManager.PlayerTwoDevice as Gamepad;

        // Rumble each player's controller with their respective rumble strength
        player1Gamepad.Rumble(1, lowFrequency, highFrequency, duration, ignorePreferences); // Player 1's controller rumbles according to Player1RumbleStrength
        player2Gamepad.Rumble(2, lowFrequency, highFrequency, duration, ignorePreferences); // Player 2's controller rumbles according to Player2RumbleStrength
    }

    public static void StopAllRumble()
    {
        foreach (var gamepad in Gamepad.all)
        {
            gamepad.SetMotorSpeeds(0 , 0);
        }
    }
}

public static class KeyboardExtensions
{
    public static bool EscapeKeyPressed(this Keyboard keyboard) => keyboard.escapeKey.wasPressedThisFrame;
    public static bool AnyKeyDown(this Keyboard keyboard) => keyboard.anyKey.wasPressedThisFrame;
}
