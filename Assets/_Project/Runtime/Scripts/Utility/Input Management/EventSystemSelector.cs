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

    void OnEnable()
    {
        string sceneName  = SceneManager.GetActiveScene().name;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneName == "Game" || sceneIndex == 2) return;
        
        localPlayerID = playerInput.playerIndex + 1;

        if (localPlayerID == 2 && (sceneName == "MainMenu" || sceneIndex == 0))
        {
            Debug.LogWarning("Player 2 is not supported in this scene.");
            return;
        }
        
        GameObject button = FindButtonBySceneName(sceneName) ?? FindButtonBySceneIndex(sceneIndex);
        
        ProcessButton(button);
    }

    GameObject FindButtonBySceneName(string sceneName) => sceneName switch
    { "MainMenu"        => // MainMenu
          localPlayerID == 1 ? GameObject.Find("Play") : null,
      
      "CharacterSelect" => // Character Select
          GameObject.Find($"Reaper (Player {localPlayerID})"),
      
      "Game"            => // Game 
          localPlayerID == 1 ? GameObject.Find("Rematch Button (Player 1)") : null,
      
      _                 => null }; 

    GameObject FindButtonBySceneIndex(int sceneIndex) => sceneIndex switch
    { 0 => // MainMenu
          localPlayerID == 1 ? GameObject.Find("Play") : null,
      
      1 => // Character Select
          GameObject.Find($"Reaper (Player {localPlayerID})"),
      
      2 => // Game 
          localPlayerID == 1 ? GameObject.Find("Debug Button") : null,
      
      _ => null 
    };
    
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
    
    public void OnPressButton()
    {
        Debug.Log($"Player {localPlayerID} pressed a button using \"{playerInput.currentControlScheme}\" control scheme!");
    }
}