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
    
    public static void DEBUG_ResetAllBindings()
    {
        // Load all input action assets
        var inputActions = Resources.FindObjectsOfTypeAll<InputActionAsset>();
        foreach (var asset in inputActions)
        {
            Debug.Log($"Resetting all bindings in {asset.name}");
            
            foreach (var map in asset.actionMaps)
            {
                map.RemoveAllBindingOverrides();
            }
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
