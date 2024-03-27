#region
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
#endregion

public class InputDeviceMonitor : MonoBehaviour //TODO: FINISH THIS
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Image deviceDisconnectImage;
    
    void OnEnable() => InputSystem.onDeviceChange += DeviceChangeListener;
    void OnDisable() => InputSystem.onDeviceChange -= DeviceChangeListener;
    
    void DeviceChangeListener(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Disconnected)
        {
            Debug.LogWarning($"Device lost: {device.name}");

            // Get the other player's device
            Player otherPlayer = PlayerManager.Players.Find(player => player.PlayerController.Device != device);
            if (!otherPlayer.PlayerController) // if a device is not found, default to the keyboard.
                otherPlayer = PlayerManager.Players.Find(player => player.PlayerController.Device is Keyboard);
            
            Debug.Log($"Other player's device: {otherPlayer.PlayerController.Device.name}");

            GameManager.TogglePause(otherPlayer.PlayerController);
            
            deviceDisconnectImage.enabled = true;
            
            foreach (var player in PlayerManager.Players)
            {
                if (player.PlayerController == null) continue;
                player.PlayerController.DisablePlayer(true);
            } 
        }
    }
}
