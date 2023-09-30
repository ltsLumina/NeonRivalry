using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSchemeSwitcher : MonoBehaviour
{
    PlayerInput playerInput;
    InputDeviceSwitcher inputDeviceSwitcher;

    public void ToggleControlScheme(int playerToAssign)
    {
        switch (playerToAssign)
        {
            case 1:
                playerInput = PlayerInput.all.FirstOrDefault(p => p.playerIndex == 0);
                inputDeviceSwitcher = playerInput.gameObject.GetComponent<InputDeviceSwitcher>();
                break;
            
            case 2:
                playerInput         = PlayerInput.all.FirstOrDefault(p => p.playerIndex == 1);
                inputDeviceSwitcher = playerInput.gameObject.GetComponent<InputDeviceSwitcher>();
                break;
        }

        string currentScheme = playerInput.currentControlScheme;

        switch (currentScheme)
        {
            case "Keyboard":
                StartCoroutine(inputDeviceSwitcher.AssignGamepadToPlayerInput(playerInput));
                break;

            case "Gamepad":
                PlayerInput.all.FirstOrDefault()                                      // Get the first active PlayerInput
                          ?.SwitchCurrentControlScheme("Keyboard", Keyboard.current); // Switch to Keyboard

                break;
        }
    }
}
