#region
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class ControlSchemeSwitcher : MonoBehaviour
{
    PlayerInput playerInput;

    // Called through Unity Event in the inspector on the button that is used to switch control schemes. Currently set to the "Settings" button.
    public void ToggleControlScheme(int playerToAssignByPlayerID)
    {
        if (Gamepad.current == null)
        {
            Debug.LogError("No Gamepad detected! \nCan't switch control schemes if there is no Gamepad connected.");
            return;
        }

        playerInput = playerToAssignByPlayerID switch
        { 1 => PlayerManager.PlayerOneInput,
          2 => PlayerManager.PlayerTwoInput,
          _ => playerInput };
        
        string currentScheme = playerInput.currentControlScheme;

        switch (currentScheme)
        {
            case "Keyboard": // Switch to Gamepad if the user is using Keyboard.
                StartCoroutine(InputDeviceSwitcher.AssignGamepadToPlayerInput(playerInput));
                
                Debug.LogWarning("Player " + playerToAssignByPlayerID + " switched to Gamepad");
                break;

            case "Gamepad": // Switch to Keyboard if the user is using Gamepad.
                PlayerManager.PlayerInputs.FirstOrDefault()?.SwitchCurrentControlScheme("Keyboard", Keyboard.current);

                Debug.LogWarning("Player " + playerToAssignByPlayerID + " switched to Keyboard");
                break;
        }
    }
}
