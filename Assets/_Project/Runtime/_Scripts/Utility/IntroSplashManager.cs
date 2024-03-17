using System.Collections;
using DG.Tweening;
using TMPro;
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
    [SerializeField] TMP_Text pressAnyButtonText;

    // -- Cached References --
    IntroPlayerInitializer playerInitializer;
    InputDeviceManager deviceManager;
    
    void Awake()
    {
        playerInitializer = FindObjectOfType<IntroPlayerInitializer>();
        deviceManager = FindObjectOfType<InputDeviceManager>();
    }

    IEnumerator Start()
    {
        GameManager.SetState(GameManager.GameState.Transitioning);
        
        // If debug mode is enabled, skip the splash screen.
        if (Logger.DebugMode)
        {
            Destroy(splashScreen.gameObject);
            yield break;
        } 

        splashScreen.gameObject.SetActive(true);
        
        // Disable input until the splash screen is done fading.
        playerInitializer.enabled = false;
        deviceManager.enabled = false;
        
        // Make the "Press any button" text invisible.
        pressAnyButtonText.alpha = 0f;
        
        yield return new WaitForSeconds(0.2f);
        
        splashScreen.DOFade(0f, splashScreenDuration).OnComplete(() =>
        {
            Destroy(splashScreen.gameObject);
            FadeInText();
        });
    }

    // Fades in the buttons and text once the splash screen is done fading.
    void FadeInText() //TODO: DOTween makes this easier.
    {
        pressAnyButtonText.DOFade(1f, 1f).OnComplete(() =>
        {
            // Enable all input after the splash screen is done fading and the buttons and text are visible.
            playerInitializer.enabled = true;
            deviceManager.enabled = true;
            
            GameManager.SetState(GameManager.GameState.Intro);
        });
    }
}
