using System.Collections;
using System.Linq;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [SerializeField, ReadOnly] PlayerInput playerInput;
    string lastUsedControlScheme;
    
    public void LoadNextScene()
    {
        SceneManagerExtended.LoadNextScene();
    }

    void OnEnable()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null) { Debug.LogError("No PlayerInput component was found in the scene!"); return; }
        
        // sets the initial control scheme to the currently used one
        lastUsedControlScheme = playerInput.currentControlScheme;

        playerInput.onControlsChanged += OnControlsChanged;
    }

    void OnControlsChanged(PlayerInput obj)
    {
        // This delegate is called when controls are changed
        string scheme = obj.currentControlScheme;

        // Since Unity's InputSystem calls this delegate even on the same control scheme, we place this check to avoid infinite loop
        if (lastUsedControlScheme == scheme) return;

        lastUsedControlScheme = scheme; // Assign the lastUsedControlScheme here

        Debug.Log("Current control scheme: " + lastUsedControlScheme);

        // Get the device that was used to trigger the change
        InputDevice device = obj.devices.FirstOrDefault();

        // Switch to the new control scheme and device
        if (device != null) { playerInput.SwitchCurrentControlScheme(scheme, device); }
    }
    
    public void ToggleControlScheme()
    {
        string currentScheme = playerInput.currentControlScheme;

        switch (currentScheme)
        {
            case "Keyboard":
                StartCoroutine(AssignGamepadToPlayerInput(playerInput));
                break;

            case "Gamepad":
                PlayerInput.all.FirstOrDefault()                                      // Get the first active PlayerInput
                          ?.SwitchCurrentControlScheme("Keyboard", Keyboard.current); // Switch to Keyboard

                break;
        }
    }

    static IEnumerator AssignGamepadToPlayerInput(PlayerInput input)
    {
        Gamepad device = null;

        while (device == null)
        {
            foreach (Gamepad gamepad in Gamepad.all.Where(gamepad => gamepad.buttonSouth.wasPressedThisFrame && !PlayerInput.all.Any(p => p.devices.Contains(gamepad))))
            {
                device = gamepad;
                break;
            }

            yield return null;
        }

        input.SwitchCurrentControlScheme("Gamepad", device);
    }

    void OnDestroy()
    {
        playerInput.onControlsChanged -= OnControlsChanged; // Unsubscribe when the object is destroyed
    }
}
