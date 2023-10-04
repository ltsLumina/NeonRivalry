using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// Waits for a player to join.
/// We check this by checking if a Menu Navigator exists. (Multiplayer Event System component would suffice as "proof")
/// </summary>
public class IntroPlayerInitializer : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] TMP_Text pressAnyButtonText;

    void Awake()
    {
        InputDeviceManager.persistPlayerDevices.Clear();

        // Reset rumble
        foreach (var gamepad in Gamepad.all) { gamepad.SetMotorSpeeds(0f, 0f); }
    }

    IEnumerator Start()
    {
        MultiplayerEventSystem player = null;
        
        // This might seem like a non-performant way of doing this, but it's the only way I could think of.
        // But due to the lack of objects in the 'Intro' scene, it's not a big deal.
        yield return new WaitUntil(() => (player = FindObjectOfType<MultiplayerEventSystem>()) != null);

        if (player != null)
        {
            Debug.Log("Player joined intro scene!");

            //TODO: Make a UI_Utility class that handles things like this. (or a manager)
            IEnumerator LoadingScreenRoutine()
            {
                // Enable the loading screen and disable the text.
                pressAnyButtonText.enabled = false;
                loadingScreen.SetActive(true);
                Debug.Log("Loading... (Hold Shift to speed up the process.)");

                // Fake loading bar
                var loadingBar = loadingScreen.GetComponentInChildren<Slider>();
                loadingBar.value = 0f;

                // The point at which the loading bar will increase at a faster rate.
                const float cutoff = 0.5f;

                while (loadingBar.value < 1f)
                {
                    bool  shiftKeyDown    = Input.GetKey(KeyCode.LeftShift); // Check if Shift key is pressed
                    float additionalSpeed = shiftKeyDown ? 0.4f : 0.0f;      // Increase speed when Shift is pressed

                    loadingBar.value += loadingBar.value <= cutoff
                        ? Random.Range(0.01f, 0.05f) + additionalSpeed  // Increases the value by a smaller rate until specified cutoff point
                        : Random.Range(0.05f, 0.2f)  + additionalSpeed; // Increase value by larger rate after cutoff point

                    yield return new WaitForSeconds(Random.Range(0.05f, 1.2f));
                }

                // Stop all rumble before loading next scene.
                foreach (var gamepad in Gamepad.all) { gamepad.SetMotorSpeeds(0f, 0f); }

                // When the loading bar is full, load the next scene.
                SceneManagerExtended.LoadNextScene();
            }

            StartCoroutine(LoadingScreenRoutine());
        }
    }
}
