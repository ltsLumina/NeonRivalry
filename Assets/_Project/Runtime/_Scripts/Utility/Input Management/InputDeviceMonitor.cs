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
            
            pauseMenu.SetActive(true);
            deviceDisconnectImage.enabled = true;
            
            foreach (var player in PlayerManager.Players)
            {
                player.PlayerController.DisablePlayer(true);
            } 
        }
    }
}
