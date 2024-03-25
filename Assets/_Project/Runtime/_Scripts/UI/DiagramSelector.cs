using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class DiagramSelector : MonoBehaviour
{
    [SerializeField] Image gamepadDiagram;
    [SerializeField] Image keyboardDiagram;
    
    GamepadIconsExample gamepad;
    KeyboardIconsExample keyboard;

    public void ShowDiagram()
    {
        gamepad  = transform.parent.parent.GetComponentInChildren<GamepadIconsExample>(true);
        keyboard = transform.parent.parent.GetComponentInChildren<KeyboardIconsExample>(true);
        
        // Check which object is active
        if (gamepad       && gamepad.gameObject.activeSelf) gamepadDiagram.gameObject.SetActive(true);
        else if (keyboard && keyboard.gameObject.activeSelf) keyboardDiagram.gameObject.SetActive(true);
    }
}
