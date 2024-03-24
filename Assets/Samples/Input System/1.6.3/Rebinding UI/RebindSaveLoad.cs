using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    public InputActionAsset player1Actions;
    public InputActionAsset player2Actions;

    public void OnEnable()
    {
        string player1Rebinds = PlayerPrefs.GetString("player1Rebinds");
        if (!string.IsNullOrEmpty(player1Rebinds)) player1Actions.LoadBindingOverridesFromJson(player1Rebinds);

        string player2Rebinds = PlayerPrefs.GetString("player2Rebinds");
        if (!string.IsNullOrEmpty(player2Rebinds)) player2Actions.LoadBindingOverridesFromJson(player2Rebinds);
    }

    public void OnDisable()
    {
        string player1Rebinds = player1Actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("player1Rebinds", player1Rebinds);

        string player2Rebinds = player2Actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("player2Rebinds", player2Rebinds);
    }
}
