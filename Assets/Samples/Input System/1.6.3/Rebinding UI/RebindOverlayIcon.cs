using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindOverlayIcon : MonoBehaviour
{
    [SerializeField] Image gamepadIcon;
    [SerializeField] Image keyboardIcon;

    void Start()
    {
        gamepadIcon.gameObject.SetActive(false); 
        keyboardIcon.gameObject.SetActive(false);
    }

    public void ShowIcon(InputDevice device)
    {
        if (device is Gamepad) gamepadIcon.gameObject.SetActive(true);
        else keyboardIcon.gameObject.SetActive(true);
    }

    public void HideIcon()
    {
        keyboardIcon.gameObject.SetActive(false);
        gamepadIcon.gameObject.SetActive(false);
    }
}
