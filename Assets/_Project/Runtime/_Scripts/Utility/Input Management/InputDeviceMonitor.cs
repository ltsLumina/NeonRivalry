#region
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
#endregion

public class InputDeviceMonitor : MonoBehaviour
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
            Time.timeScale = 0f;
        }
    }
}
