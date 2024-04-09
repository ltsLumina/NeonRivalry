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
            PlayerController otherPlayer = PlayerManager.Players.Find(player => player.Device != device);
            if (!otherPlayer) // if a device is not found, default to the keyboard.
                otherPlayer = PlayerManager.Players.Find(player => player.Device is Keyboard);
            
            Debug.Log($"Other player's device: {otherPlayer.Device.name}");

            GameManager.TogglePause(otherPlayer);
            
            deviceDisconnectImage.enabled = true;
            
            foreach (var player in PlayerManager.Players)
            {
                if (player == null) continue;
                player.DisablePlayer(true);
            } 
        }
    }
}
