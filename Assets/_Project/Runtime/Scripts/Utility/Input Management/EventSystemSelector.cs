using System.Collections.Generic;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MultiplayerEventSystem))]
public class EventSystemSelector : MonoBehaviour
{
    // -- Fields --
    
    [SerializeField] PlayerInput playerInput;
    [SerializeField, ReadOnly] int localPlayerID;

    // -- Cached References --
    
    MultiplayerEventSystem eventSystem;
    Button firstSelected;

    // In the case of the Menu Navigator game object, the PlayerInput is tied to this game object.
    // The Player's UI Navigator, however, is childed to the Player's game object.
    void Awake() => playerInput = GetComponent<PlayerInput>();

    void Update()
    {
        string sceneName  = SceneManager.GetActiveScene().name;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneName == "Game" || sceneIndex == 3) return;
        
        localPlayerID = playerInput.playerIndex + 1;

        if (localPlayerID == 2 && IsNotSupportedForPlayer2(sceneName, sceneIndex))
        {
            Debug.LogWarning("Player 2 is not supported in this scene.");
            return;
        }

        GameObject button = FindButtonBySceneIndex(sceneIndex) ?? FindButtonBySceneName(sceneName);
        
        ProcessButton(button);
    }

    static bool IsNotSupportedForPlayer2(string sceneName, int sceneIndex)
    {
        List<string> nonSupportedScenes = new() { "Intro", "MainMenu" };

        return nonSupportedScenes.Contains(sceneName) || sceneIndex == 0 || sceneIndex == 1;
    }

    GameObject FindButtonBySceneIndex(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 0: // Intro
                Debug.LogWarning("There is no button to find in the 'Intro' scene. Returning null. (This is not an error)");
                return null;

            case 1: // MainMenu
                return localPlayerID == 1 ? GameObject.Find("Play") : null;

            case 2: // Character Select
                return GameObject.Find($"Reaper (Player {localPlayerID})");

            case 3: // Game 
                return localPlayerID == 1 ? GameObject.Find("Debug Button") : null;

            default:
                return null;
        }
    }

    GameObject FindButtonBySceneName(string sceneName)
    {
        switch (sceneName)
        {
            case "Intro": // Intro
                Debug.LogWarning("There is no button to find in the 'Intro' scene. Returning null. (This is not an error)");
                return null;

            case "MainMenu": // MainMenu
                return localPlayerID == 1 ? GameObject.Find("Play") : null;

            case "CharacterSelect": // Character Select
                return GameObject.Find($"Reaper (Player {localPlayerID})");

            case "Game": // Game 
                return localPlayerID == 1 ? GameObject.Find("Rematch Button (Player 1)") : null;

            default:
                return null;
        }
    }

    void ProcessButton(GameObject button)
    {
        if (button == null)
        {
            Debug.LogWarning("No button was found in this scene. (Scene name and index failed to return a button).");
            return;
        }

        firstSelected = button.GetComponent<Button>();
        eventSystem = GetComponent<MultiplayerEventSystem>();
        
        if (firstSelected != null && eventSystem != null) 
            eventSystem.firstSelectedGameObject = firstSelected.gameObject;
    }
    
    // Debug button.
    public void OnPressButton()
    {
        Debug.Log($"Player {localPlayerID} pressed a button using \"{playerInput.currentControlScheme}\" control scheme!");
    }
}