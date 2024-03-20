using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public void UpdatePlayer1RumbleStrength(Slider slider) { PlayerPrefs.SetFloat("Player1_RumbleStrength", slider.value); }

    public void UpdatePlayer2RumbleStrength(Slider slider) { PlayerPrefs.SetFloat("Player2_RumbleStrength", slider.value); }

    public void Rumble(int playerID)
    {
        float   speed   = PlayerPrefs.GetFloat(playerID == 1 ? "Player1_RumbleStrength" : "Player2_RumbleStrength");
        Gamepad gamepad;

        if (playerID == 1) gamepad = (Gamepad) InputDeviceManager.PlayerOneDevice;
        else gamepad               = (Gamepad) InputDeviceManager.PlayerTwoDevice;

        StartCoroutine(RumbleCoroutine());
        
        return; // Local function
        IEnumerator RumbleCoroutine()
        {
            gamepad?.SetMotorSpeeds(speed, speed);
            yield return new WaitForSeconds(0.1f);
            gamepad?.SetMotorSpeeds(0, 0);
        }
    }
}
