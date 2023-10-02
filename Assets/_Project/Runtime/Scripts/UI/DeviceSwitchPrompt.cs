using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceSwitchPrompt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text ellipsisText;
    [SerializeField] GameObject[] objectsToEnable;
    [SerializeField] GameObject[] objectsToDisable;
    
    // -- Private Variables --
    
    const string baseText = "Press the START/SPACEBAR button to continue";
    
    // -- Properties --
    
    public static bool IsWaitingForInput { get; private set; }

    void OnEnable()
    {
        ellipsisText.text = baseText; 
        StopCoroutine(ShowDotsWithInterval(ellipsisText, 3, 1f));

        // Reset the isWaitingForInput bool to false.
        IsWaitingForInput = false;
    }

    public void StartWaitingForInput() => StartCoroutine(WaitingForInput());

    IEnumerator WaitingForInput()
    {
        // Start the coroutine that shows the ellipsis
        StartCoroutine(ShowDotsWithInterval(ellipsisText, 3, 1f));
        
        // Set the isWaitingForInput bool to true. This is used to prevent a player from joining while the prompt is active.
        IsWaitingForInput = true;
        
        // Disable all objects in the objectsToDisable array
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
        
        // Enable all objects in the objectsToEnable array
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }
        
        // Wait for the player to press a button on the keyboard
        while (true)
        {
            // Check if the player pressed the space key or the start button on the gamepad
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame)
            {
                // If so, break out of the while loop
                break; 
            }
            
            // Wait for the next frame
            yield return null;
        }
        
        // An input was detected, so stop the coroutine
        StopCoroutine(ShowDotsWithInterval(ellipsisText, 3, 1f));
        
        // Set the isWaitingForInput bool to false.
        IsWaitingForInput = false;
        
        // Disable all objects in the objectsToEnable array
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(false);
        }
        
        // Enable all objects in the objectsToDisable array
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(true);
        }
    }

    static IEnumerator ShowDotsWithInterval(TMP_Text promptText, int numberOfDots, float intervalInSeconds)
    {
        var sb = new StringBuilder(baseText, baseText.Length + numberOfDots);

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                sb.Append('.');
                promptText.text = sb.ToString();
                yield return new WaitForSeconds(intervalInSeconds);
            }

            // This resets the string back to the baseText.
            sb.Length       = baseText.Length;
            promptText.text = baseText;
            yield return new WaitForSeconds(intervalInSeconds);
        }
    }
}
