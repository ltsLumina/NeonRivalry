using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{ 
    [Header("References")]
    [SerializeField] GameObject[] objectsToEnable;
    [SerializeField] GameObject[] objectsToDisable;

    public void OpenMenu()
    {
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
    }

    public void CloseMenu()
    {
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
}
