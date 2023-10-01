using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventSystemSelector : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField, ReadOnly] int localPlayerID;
    
    MultiplayerEventSystem eventSystem;
    Button firstSelected;

    GameObject playButton;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        // Get the current scene name and index
        string sceneName  = SceneManager.GetActiveScene().name;
        int    sceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Get the local player ID
        localPlayerID = playerInput.playerIndex + 1;

        if (localPlayerID == 2 && (sceneName == "MainMenu" || sceneIndex == 0))
        {
            Debug.LogWarning("Player 2 is not supported in this scene.");
            return;
        }
        
        GameObject button = FindButtonBySceneName(sceneName);

        if (button == null) // if no button was found by scene name
            button = FindButtonBySceneIndex(sceneIndex);

        // If a button still can't be found, log a warning and return
        if (button == null)
        {
            Debug.LogError("No button was found in this scene. (Scene Name and Index failed to return a button).");
            return;
        }

        // If a button was found, get the button component and set it as the first selected button
        if (button != null)
        {
            firstSelected = button.GetComponent<Button>();
            eventSystem   = GetComponent<MultiplayerEventSystem>();
            if (eventSystem != null && firstSelected != null) 
                eventSystem.firstSelectedGameObject = firstSelected.gameObject;
        }
        else { Debug.LogWarning("No button was found in this scene."); }
    }

    GameObject FindButtonBySceneName(string sceneName)
    {
        GameObject button = null;

        switch (sceneName)
        {
            case "MainMenu":
                switch (localPlayerID)
                {
                    case 1:
                        button = GameObject.Find("Play");
                        break;

                    case 2:
                        Debug.LogWarning("Player 2 is not supported in the Main Menu scene.");
                        break;
                }

                break;

            case "CharacterSelect":
                switch (localPlayerID)
                {
                    case 1:
                        button = GameObject.Find("Reaper (Player 1)");
                        break;

                    case 2:
                        button = GameObject.Find("Reaper (Player 2)");
                        break;
                }

                break;

            case "Game":
                switch (localPlayerID)
                {
                    case 1:
                        //button = GameObject.Find("Debug Button");
                        button = GameObject.Find("Rematch Button (Player 1)");
                        break;

                    case 2:
                        Debug.LogWarning("Player 2 is not supported for the Debug Button.");
                        break;
                }

                break;
        }

        if (button == null) Debug.LogWarning($"No button was found in the {sceneName} scene. \nChecking by scene index...");
        
        return button;
    }

    // Fallback method in-case the button cannot be found by scene name.
    GameObject FindButtonBySceneIndex(int sceneIndex)
    {
        GameObject button = null;

        switch (sceneIndex)
        {
            case 0: // Main Menu
                switch (localPlayerID)
                {
                    case 1:
                        button = GameObject.Find("Play");
                        break;

                    case 2:
                        Debug.LogWarning("Player 2 is not supported in the Main Menu scene.");
                        return null;
                }

                break;

            case 1: // Character Select
                switch (localPlayerID)
                {
                    case 1:
                        button = GameObject.Find("Reaper (Player 1)");
                        break;

                    case 2:
                        button = GameObject.Find("Reaper (Player 2)");
                        break;
                }

                break;

            case 2:
                switch (localPlayerID)
                {
                    case 1:
                        button = GameObject.Find("Debug Button");
                        //button = GameObject.Find("Rematch Button (Player 1)");
                        break;

                    case 2:
                        Debug.LogWarning("Player 2 is not supported for the Debug Button.");
                        break;
                }

                break;
        }
        
        return button;
    }

    public void OnPressButton()
    {
        Debug.Log($"Player {localPlayerID} pressed a button using \"{playerInput.currentControlScheme}\" control scheme!");        
    }
}