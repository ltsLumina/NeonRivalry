using UnityEngine;
using UnityEngine.InputSystem;

public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] string targetControlScheme;

    public delegate void ResetBindings();
    public static event ResetBindings OnResetBindings;
    
    public void ResetAllBindings()
    {
        foreach (var map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
            OnResetBindings?.Invoke();
        }
    }

    public void ResetControlSchemeBinding()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(targetControlScheme));
            }
        }
    }
}
