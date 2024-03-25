using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class SettingsInfoText : MonoBehaviour
{
    [SerializeField] string messageTemplate = "General Settings are controlled by Player 1.\nFor Per-Player Settings, press \"{0}\" when selecting character.";
    
    InputDevice playerOneDevice => InputDeviceManager.PlayerOneDevice;
    TextMeshProUGUI text;

    void Awake() => text = GetComponent<TextMeshProUGUI>();

    void OnEnable() => InputDeviceManager.OnPlayerJoin += OnPlayerJoin;
    void OnDisable() => InputDeviceManager.OnPlayerJoin -= OnPlayerJoin;

    void OnPlayerJoin()
    {
        string key     = playerOneDevice is Keyboard ? "Escape" : "Start";
        string message = string.Format(messageTemplate, key);
        text.text = message;
    }
}
