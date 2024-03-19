using System.Linq;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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

    // -- Scenes --
    
    const int Intro = 0;
    const int MainMenu = 1;
    const int CharacterSelect = 2;
    const int Game = 3;
    
    public GameObject CurrentSelectedGameObject
    {
        get
        {
            OnSelectionChange?.Invoke(eventSystem.currentSelectedGameObject);
            return eventSystem.currentSelectedGameObject;
        }
    }

    public delegate void SelectionChange(GameObject newSelectedGameObject);
    public event SelectionChange OnSelectionChange;

    // In the case of the Menu Navigator game object, the PlayerInput is tied to this game object.
    // The Player's UI Navigator, however, is childed to the Player's game object.
    void Awake() => playerInput = GetComponent<PlayerInput>();

    void Start()
    {
        string sceneName  = SceneManagerExtended.ActiveSceneName;
        int sceneIndex = SceneManagerExtended.ActiveScene;

        // If the scene is the Intro or Game scene, return, as we don't want to select a button in these scenes.
        if (sceneIndex is Intro or Game) return;

        // Assign the localPlayerID based on the playerInput's playerIndex.
        // Usually we would use player.PlayerID, but there is no "player" instance until the game scene.
        localPlayerID = playerInput.playerIndex + 1;

        if (localPlayerID == 2 && IsNotSupportedForPlayer2(sceneIndex))
        {
            Debug.LogWarning($"Player 2 is not supported in the {sceneName} scene.");
            return;
        }

        GameObject button = FindButtonBySceneIndex(sceneIndex);

        if (button == null)
        {
            Debug.LogWarning("No button was found in this scene. (Scene index failed to return a button).");
            return;
        }

        ProcessButton(button);
    }

    static bool IsNotSupportedForPlayer2(int sceneIndex)
    {
        int[] nonSupportedSceneIndex =
        { Intro, MainMenu }; 

        return nonSupportedSceneIndex.Contains(sceneIndex);
    }

    GameObject FindButtonBySceneIndex(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case MainMenu: 
                return GameObject.Find("Play");
            
            case CharacterSelect: 
                return GameObject.Find($"Shelby (Player {localPlayerID})");

            default:
                return null;
        }
    }

    void ProcessButton(GameObject button)
    {
        firstSelected = button.GetComponent<Button>();
        eventSystem = GetComponent<MultiplayerEventSystem>();

        if (firstSelected != null && eventSystem != null) 
            eventSystem.SetSelectedGameObject(firstSelected.gameObject);
    }
    
    // Debug button.
    public void OnPressButton()
    {
        Debug.Log($"Player {localPlayerID} pressed a button using \"{playerInput.currentControlScheme}\" control scheme!");
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            menuManager.CloseCurrentMainMenu();
            menuManager.CloseCurrentSettingsMenu();
        }
    }
}