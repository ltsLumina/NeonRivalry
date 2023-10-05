#region
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endregion

public static class GamepadExtensions
{
    public static bool AnyButton(this Gamepad gamepad)
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
}
