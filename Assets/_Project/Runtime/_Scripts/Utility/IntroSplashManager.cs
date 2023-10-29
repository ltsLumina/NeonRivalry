using System.Collections;
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

    void Start()
    {
        // If debug mode is enabled, skip the splash screen.
        if (Logger.DebugMode)
        {
            Destroy(splashScreen.gameObject);
            return;
        } 

        splashScreen.gameObject.SetActive(true);
        
        // Disable input until the splash screen is done fading.
        playerInitializer.enabled = false;
        deviceManager.enabled = false;
        
        // Make the "Press any button" text invisible.
        pressAnyButtonText.alpha = 0f;
        
        StartCoroutine(FadeSplashScreenBackground());
    }

    IEnumerator FadeSplashScreenBackground()
    {
        // Disable all input until the splash screen is done fading.
        
        // Fade the splash screen background out.
        splashScreen.CrossFadeAlpha(0f, splashScreenDuration, true);

        yield return new WaitForSeconds(splashScreenDuration);
        
        Destroy(splashScreen.gameObject);
        
        // Fade in the buttons and text once the splash screen is done fading.
        StartCoroutine(FadeInText());
    }

    // Fades in the buttons and text once the splash screen is done fading.
    IEnumerator FadeInText() //TODO: DOTween makes this easier.
    {
        Color color = pressAnyButtonText.color;

        while (color.a < 1.0f)
        {
            // Increase alpha
            color.a += 1 * Time.deltaTime;

            // Clamp it to 1.0
            color.a = Mathf.Min(color.a, 1.0f);

            pressAnyButtonText.color = color;

            // Yield execution until next frame.
            yield return null;
        }

        // Enable all input after the splash screen is done fading and the buttons and text are visible.
        playerInitializer.enabled = true;
        deviceManager.enabled     = true;
    }
}
