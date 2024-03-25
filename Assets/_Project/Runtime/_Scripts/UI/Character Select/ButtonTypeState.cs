using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public sealed class ButtonTypeState : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] List<RebindActionUI> rebindButtons;

    const string DefaultState = "Default";
    const string CustomState = "Custom";
    
    void OnEnable()
    {
        if (rebindButtons.Any(rebind => rebind.AreAnyBindingsOverridden())) stateText.text = CustomState;

        foreach (var rebindActionUI in rebindButtons) { rebindActionUI.stopRebindEvent.AddListener(OnStopRebind); }
        ResetDeviceBindings.OnResetBindings += () => stateText.text = DefaultState;
    }

    void OnDisable()
    {
        foreach (var rebindActionUI in rebindButtons) { rebindActionUI.stopRebindEvent.RemoveListener(OnStopRebind); }
    }

    void OnStopRebind(RebindActionUI arg0, InputActionRebindingExtensions.RebindingOperation rebindingOperation)
    {
        bool isCustom = rebindingOperation.completed;

        stateText.text = isCustom ? CustomState : DefaultState;
    }
}
