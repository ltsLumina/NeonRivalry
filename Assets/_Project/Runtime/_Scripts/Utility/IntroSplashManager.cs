using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Logger = Lumina.Debugging.Logger;

/// <summary>
/// Manages the intro splash screen.
/// This script 
/// </summary>
public class IntroSplashManager : MonoBehaviour
{
    // -- Serialized Fields --
    [SerializeField] Image splashScreen;
    [SerializeField] float splashScreenDuration;

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

    void Start()
    {
        GameManager.SetState(GameManager.GameState.Transitioning);
        
        // If debug mode is enabled, skip the splash screen.
        if (Logger.DebugMode)
        {
            Destroy(splashScreen.gameObject);
            gamepadPrompts.transform.localScale = Vector3.one;
            buttonPromptsManager.ShowGamepadPrompts(SceneManagerExtended.ActiveSceneName, true);
            return;
        } 

        splashScreen.gameObject.SetActive(true);
        
        // Disable input until the splash screen is done fading.
        playerInitializer.enabled = false;
        deviceManager.enabled = false;
        
        // Default to showing the Gamepad prompts.
        buttonPromptsManager.ShowGamepadPrompts(SceneManagerExtended.ActiveSceneName, true);
        gamepadPrompts.transform.localScale = Vector3.zero;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(splashScreen.DOFade(0f, splashScreenDuration));
        sequence.Append(gamepadPrompts.transform.DOScale(1.25f, 1f));
        sequence.Append(gamepadPrompts.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));
        sequence.AppendInterval(0.5f);
        sequence.OnComplete
        (() =>
        {
            Destroy(splashScreen.gameObject);

            playerInitializer.enabled = true;
            deviceManager.enabled     = true;

            GameManager.SetState(GameManager.GameState.Intro);
        });
    }
}
