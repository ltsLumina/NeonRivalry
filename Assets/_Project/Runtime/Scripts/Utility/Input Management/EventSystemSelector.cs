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

    GameObject FindButtonBySceneIndex(int sceneIndex) => sceneIndex switch
    { 0 => // Intro
          localPlayerID == 1 ? GameObject.Find("Press any button!") : null,
      1 => // MainMenu
          localPlayerID == 1 ? GameObject.Find("Play") : null,
      2 => // Character Select
          GameObject.Find($"Reaper (Player {localPlayerID})"),
      3 => // Game 
          localPlayerID == 1 ? GameObject.Find("Debug Button") : null,
      _ => null };

    GameObject FindButtonBySceneName(string sceneName) => sceneName switch
    { "Intro" => // Intro
          localPlayerID == 1 ? GameObject.Find("Press any button!") : null,
      "MainMenu" => // MainMenu
          localPlayerID == 1 ? GameObject.Find("Play") : null,
      "CharacterSelect" => // Character Select
          GameObject.Find($"Reaper (Player {localPlayerID})"),
      "Game" => // Game 
          localPlayerID == 1 ? GameObject.Find("Rematch Button (Player 1)") : null,
      _ => null };

    void ProcessButton(GameObject button)
    {
        if (button == null)
        {
            Debug.LogError("No button was found in this scene. (Scene name and index failed to return a button).");
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