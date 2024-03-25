using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;

public class RebindSaveLoad : MonoBehaviour
{
    public InputActionAsset player1Actions;
    public InputActionAsset player2Actions;

    RebindActionUI rebindActionUI;
    
    public void OnEnable()
    {
        rebindActionUI = FindObjectOfType<RebindActionUI>();
        
        // Load rebinds from PlayerPrefs
        
        string player1Rebinds = PlayerPrefs.GetString("player1Rebinds");
        if (!string.IsNullOrEmpty(player1Rebinds) && player1Actions) player1Actions.LoadBindingOverridesFromJson(player1Rebinds);

        string player2Rebinds = PlayerPrefs.GetString("player2Rebinds");
        if (!string.IsNullOrEmpty(player2Rebinds) && player2Actions) player2Actions.LoadBindingOverridesFromJson(player2Rebinds);
    }

    void Start() => rebindActionUI?.UpdateBindingDisplay();

    public void OnDisable()
    {
        // Save rebinds to PlayerPrefs
        
        if (player1Actions)
        {
            string player1Rebinds = player1Actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("player1Rebinds", player1Rebinds);
        }

        if (player2Actions)
        {
            string player2Rebinds = player2Actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("player2Rebinds", player2Rebinds);
        }
    }
}
