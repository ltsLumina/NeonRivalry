using UnityEngine;
using UnityEngine.InputSystem;


// This script probably wont be needed for a long time, but it's here just in case.
public class InputMapManager : MonoBehaviour
{
    public InputActionMap gameplayActionMap;
    public InputActionMap uiActionMap;

    public void SwitchToGameplayControls()
    {
        uiActionMap.Disable();
        gameplayActionMap.Enable();
    }

    public void SwitchToUIControls()
    {
        gameplayActionMap.Disable();
        uiActionMap.Enable();
    }
}


