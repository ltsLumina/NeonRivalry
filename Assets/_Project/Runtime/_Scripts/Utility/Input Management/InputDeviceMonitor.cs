#region
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
#endregion

public class InputDeviceMonitor : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Image deviceDisconnectImage;
    
    // Start is called before the first frame update
    void Start() => InputSystem.onDeviceChange += DeviceChangeListener;

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

    // make sure to unregister the event handler to prevent memory leaks
    void OnDestroy() => InputSystem.onDeviceChange -= DeviceChangeListener;
}
