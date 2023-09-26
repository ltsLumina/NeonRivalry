using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapManager : SingletonPersistent<InputMapManager>
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


