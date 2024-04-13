using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.InputSystem;

#region Old Loading Screen Code
#endregion

/// <summary>
/// Waits for a player to join.
/// We check this by checking if a Menu Navigator exists. (Multiplayer Event System component would suffice as "proof")
/// </summary>
public class IntroPlayerInitializer : MonoBehaviour
{
    Sound intro;

    IEnumerator Start()
    {
        // Enable the button prompts.
        var buttonPromptsManager = FindObjectOfType<ButtonPromptsManager>();
        
        // ReSharper disable once NotAccessedVariable
        MenuNavigator player = null;

        // Wait until a player joins.
        // This might seem redundant, but it is necessary to prevent the game from loading the next scene before a player joins.
        yield return new WaitUntil(() => (player = FindObjectOfType<MenuNavigator>()) != null);

        player.GetComponent<PlayerInput>().enabled = true;
        
        // If the player joined using a keyboard, show the keyboard prompts.
        if (player.GetComponent<PlayerInput>().currentControlScheme == "Keyboard")
        {
            buttonPromptsManager.HideGamepadPrompts();
            buttonPromptsManager.ShowKeyboardPrompts(SceneManagerExtended.ActiveSceneName, true);
        }

        List<ButtonPrompt> currentPrompts = buttonPromptsManager.CurrentPrompts;
        // Tween out the button prompts.
        foreach (ButtonPrompt prompt in currentPrompts)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(prompt.transform.DOScale(Vector3.one * 1.15f, 0.3f).SetEase(Ease.InCirc));
            sequence.Append(prompt.transform.DOScale(Vector3.zero, 0.5f)).SetEase(Ease.InCirc);
        }
        
        // Play the intro sound.
        intro = new (SFX.Intro);
        intro.SetVolume(1f);
        intro.Play();
        
        yield return new WaitForSeconds(.85f);
        
        TransitionAnimator transition = FindObjectOfType<TransitionAnimator>();
        transition.enabled = true;
    }

    #region Old Loading Screen Code
    //     // Check if debug mode is enabled, and if so, skip the loading screen.
    //     // Else, start the loading screen.
    //     StartCoroutine(!Logger.DebugMode ? LoadingScreenRoutine() : DebugSkipLoadingScreen());
    //
    //     yield break;
    //     IEnumerator LoadingScreenRoutine() //TODO: Make a UI_Utility class that handles things like loading screens etc.
    //     {
    //         // Enable the loading screen and disable the text.
    //         pressAnyButtonText.enabled = false;
    //         loadingScreen.SetActive(true);
    //         Debug.Log("Loading... (Hold Shift to speed up the process.)");
    //
    //         // Fake loading bar
    //         var loadingBar = loadingScreen.GetComponentInChildren<Slider>();
    //         loadingBar.value = 0f;
    //
    //         // The point at which the loading bar will increase at a faster rate.
    //         const float cutoff = 0.5f;
    //
    //         while (loadingBar.value < 1f)
    //         {
    //             // Speed modifier based on input commands
    //             float speedModifer = Input.GetKey(KeyCode.LeftShift) || (Gamepad.current != null && Gamepad.current.rightShoulder.isPressed) ? 0.5f : 0.0f;
    //
    //             // Rate of increase in loading bar value
    //             float minimumRate    = loadingBar.value <= cutoff ? 0.01f : 0.05f;
    //             float maximumRate    = loadingBar.value <= cutoff ? 0.05f : 0.2f;
    //             float rateOfIncrease = Random.Range(minimumRate, maximumRate) + speedModifer;
    //
    //             loadingBar.value += rateOfIncrease;
    //
    //             yield return new WaitForSeconds(Random.Range(0.05f, 1.2f));
    //         }
    //
    //         // Stop all rumble before loading next scene.
    //         if (Gamepad.all.Count > 0)
    //             foreach (var gamepad in Gamepad.all) { gamepad.SetMotorSpeeds(0f, 0f); }
    //
    //         // When the loading bar is full, load the next scene.
    //         SceneManagerExtended.LoadNextScene();
    //     }
    // }
    //
    // #region Utility
    // IEnumerator DebugSkipLoadingScreen()
    // {
    //     const string debugWarning = "Debug Mode is enabled, skipping the loading screen!";
    //
    //     // Warn the player through the console and on screen.
    //     Logger.Log(debugWarning, LogType.Warning);
    //     pressAnyButtonText.text = "Skipping Loading Screen...";
    //
    //     // Wait a few seconds before loading the next scene.
    //     yield return new WaitForSeconds(1f);
    //
    //     SceneManagerExtended.LoadNextScene();
    // }
    // #endregion
    #endregion
}
