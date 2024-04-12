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
    /// <param name="lowFrequency">The low (light) frequency for the rumble.</param>
    /// <param name="highFrequency">The high (heavy) frequency for the rumble, defaults to 0.25f.</param>
    /// <param name="duration">The duration for the rumble, defaults to 0.25 (float) seconds.</param>
    public static void Rumble(this Gamepad gamepad, float lowFrequency = 0.25f, float highFrequency = 0.25f, float duration = 0.25f)
    {
        if (gamepad == null) return;
        
        lowFrequency  = LerpFrequency(lowFrequency);
        highFrequency = LerpFrequency(highFrequency);

        // Rumble the controller that joined for 'duration' time.
        MonoBehaviour host           = CoroutineHelper.GetHost();
        var           rumbleSequence = new Sequence(host);
        rumbleSequence.Execute(() => gamepad.SetMotorSpeeds(lowFrequency, highFrequency)).WaitThenExecute(duration, () => gamepad.SetMotorSpeeds(0, 0));

        return; // Local function
        float LerpFrequency(float frequency) => Mathf.Lerp(frequency, frequency * 2, duration);
    }

    public static void RumbleAll(float lowFrequency = 0.25f, float highFrequency = 0.25f, float duration = 0.25f)
    {
        if (Gamepad.all.Count == 0) return;

        lowFrequency  = LerpFrequency(lowFrequency);
        highFrequency = LerpFrequency(highFrequency);

        // Rumble the controller that joined for 'duration' time.
        var rumbleSequence = new Sequence(CoroutineHelper.GetHost());

        foreach (Gamepad gamepad in Gamepad.all)
        {
            rumbleSequence.Execute(() => gamepad.SetMotorSpeeds(lowFrequency, highFrequency)).WaitThenExecute(duration, () => gamepad.SetMotorSpeeds(0, 0));
        }

        return; // Local function
        float LerpFrequency(float frequency) => Mathf.Lerp(frequency, frequency * 2, duration);
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
