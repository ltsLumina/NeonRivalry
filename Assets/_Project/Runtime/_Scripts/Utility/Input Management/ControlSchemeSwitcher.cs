#region
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class ControlSchemeSwitcher : MonoBehaviour
{
    PlayerInput playerInput;

    // Called through Unity Event in the inspector on the button that is used to switch control schemes.
    public void ToggleControlScheme(int playerToAssign)
    {
        if (Gamepad.current == null)
        {
            Debug.LogError("No Gamepad detected! \nCan't switch control schemes if there is no Gamepad connected.");
            return;
        }

        playerInput = playerToAssign switch
        { 1 => PlayerInput.all.FirstOrDefault(p => p.playerIndex == 0),
          2 => PlayerInput.all.FirstOrDefault(p => p.playerIndex == 1),
          _ => playerInput };

        if (playerInput == null) return; // This should never happen, but just in case.
        string currentScheme = playerInput.currentControlScheme;

        switch (currentScheme)
        {
            case "Keyboard":
                StartCoroutine(InputDeviceSwitcher.AssignGamepadToPlayerInput(playerInput));
                Debug.LogWarning("Player " + playerToAssign + " switching to Gamepad");
                break;

            case "Gamepad":
                PlayerInput.all.FirstOrDefault()                                      // Get the first active PlayerInput
                          ?.SwitchCurrentControlScheme("Keyboard", Keyboard.current); // Switch to Keyboard

                Debug.LogWarning("Player " + playerToAssign + " switching to Keyboard");
                break;
        }
    }
}
