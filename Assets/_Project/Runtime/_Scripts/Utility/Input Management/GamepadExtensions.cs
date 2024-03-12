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
    /// <param name="host">The MonoBehaviour that will act as a coroutine host for the sequence.</param>
    /// <param name="lowFrequency">The low (light) frequency for the rumble.</param>
    /// <param name="highFrequency">The high (heavy) frequency for the rumble, defaults to 0.25f.</param>
    /// <param name="duration">The duration for the rumble, defaults to 0.25 (float) seconds.</param>
    public static void Rumble(this Gamepad gamepad, MonoBehaviour host, float lowFrequency = 0.25f, float highFrequency = 0.25f, float duration = 0.25f)
    {
        if (gamepad == null) return;
        
        lowFrequency  = LerpFrequency(lowFrequency);
        highFrequency = LerpFrequency(highFrequency);

        // Rumble the controller that joined for 'duration' time.
        var rumbleSequence = new Sequence(host);
        rumbleSequence.Execute(() => gamepad.SetMotorSpeeds(lowFrequency, highFrequency)).WaitThenExecute(duration, () => gamepad.SetMotorSpeeds(0, 0));

        return; // Local function
        float LerpFrequency(float frequency) => Mathf.Lerp(frequency, frequency * 2, duration);
    }

    /// <summary>
    ///     This is an overload that initiates a rumble effect on the specified gamepad controller.
    ///     The controller is associated with a PlayerController instance rather than a MonoBehaviour.
    /// </summary>
    /// <param name="gamepad">The Gamepad to initiate the rumble effect on.</param>
    /// <param name="player">The PlayerController associated with the gamepad.</param>
    /// <param name="lowFrequency">The low frequency at which the gamepad should rumble. Defaults to 0.25f.</param>
    /// <param name="highFrequency">The high frequency at which the gamepad should rumble. Defaults to 0.25f.</param>
    /// <param name="duration">The duration for which the gamepad should rumble. Defaults to 0.25f.</param>
    /// <remarks>
    ///     The method uses a sequence to execute the rumble effect and then stop it after the specified duration.
    /// </remarks>
    public static void Rumble(this Gamepad gamepad, PlayerController player, float lowFrequency = 0.25f, float highFrequency = 0.25f, float duration = 0.25f)
    {
        if (gamepad == null) return;
        
        lowFrequency  = LerpFrequency(lowFrequency);
        highFrequency = LerpFrequency(highFrequency);

        // Rumble the controller that joined for 'duration' time.
        var rumbleSequence = new Sequence(player);
        rumbleSequence
           .Execute(() => gamepad.SetMotorSpeeds(lowFrequency, highFrequency))
           .WaitThenExecute(duration, () => gamepad.SetMotorSpeeds(0, 0));

        return; // Local function
        float LerpFrequency(float frequency) => Mathf.Lerp(frequency, frequency * 2, duration);
    }
}
