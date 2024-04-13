using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Manages the intro splash screen.
/// This script 
/// </summary>
public class IntroSplashManager : MonoBehaviour
{
    [SerializeField] GameObject keyboardPrompts;
    [SerializeField] GameObject gamepadPrompts;

    // -- Cached References --
    IntroPlayerInitializer playerInitializer;
    InputDeviceManager deviceManager;
    ButtonPromptsManager buttonPromptsManager;
    
    void Awake()
    {
        playerInitializer = FindObjectOfType<IntroPlayerInitializer>();
        deviceManager = FindObjectOfType<InputDeviceManager>();
        buttonPromptsManager = FindObjectOfType<ButtonPromptsManager>();
    }

    IEnumerator Start()
    {
        GameManager.SetState(GameManager.GameState.Transitioning);
        
        // Disable input until the splash screen is done fading.
        playerInitializer.enabled = false;
        deviceManager.enabled = false;
        
        // Default to showing the Gamepad prompts.
        buttonPromptsManager.ShowGamepadPrompts(SceneManagerExtended.ActiveSceneName, true);
        gamepadPrompts.transform.localScale = Vector3.zero;
       
        const float duration = 2.5f;
        yield return new WaitForSeconds(duration);
        
        // Once the splash screen is done animating, show the prompts.
        gamepadPrompts.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCirc);
        
        // Enable all the stuff.
        playerInitializer.enabled = true;
        deviceManager.enabled = true;
    }
}
