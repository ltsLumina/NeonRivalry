using System.Linq;
using Lumina.Essentials.Attributes;
using MelenitasDev.SoundsGood;
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

    Sound navigateUp;
    Sound navigateDown;
    Sound navigateLeft;
    Sound navigateRight;
    Sound acceptSFX;
    Sound cancelSFX;
    Sound closeMenu;
    Sound CSNavigateLeft;
    Sound CSNavigateRight;
    
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
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null) transform.parent.GetComponentInChildren<PlayerInput>();
        eventSystem = GetComponent<MultiplayerEventSystem>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (eventSystem.currentSelectedGameObject == null) return;

        if (context.performed)
        {
            if (eventSystem.currentSelectedGameObject == null) return;
            Vector2 input = context.ReadValue<Vector2>();

            if (input.y > 0) navigateUp.Play();
            if (input.y < 0) navigateDown.Play();
            if (input.x < 0) navigateLeft.Play();
            if (input.x > 0) navigateRight.Play();
        }
    }
    
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            acceptSFX.Play();
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var menuManager = FindObjectOfType<MenuManager>();
            if (menuManager == null) return;

            if (menuManager.IsAnyMenuActive())
            {
                menuManager.CloseCurrentMainMenu();
                menuManager.CloseCurrentSettingsMenu();
                closeMenu.Play();
            }
            
            cancelSFX.Play();
        }
    }
    
    public void OnSwitchSettingsMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var menuManager = FindObjectOfType<MenuManager>();
            if (menuManager == null) return;

            var input = context.ReadValue<Vector2>();

            if (menuManager.IsAnyMenuActive())
            {
                if (input.x > 0) CSNavigateLeft.Play();
                if (input.x < 0) CSNavigateRight.Play();
            }
        }
    }
    
    void Start()
    {
        // Initialize the navigation sounds.
        navigateUp    = new (SFX.NavigateUp);
        navigateDown  = new (SFX.NavigateDown);
        navigateLeft  = new (SFX.NavigateLeft);
        navigateRight = new (SFX.NavigateRight);
        
        navigateUp.SetOutput(Output.SFX);
        navigateDown.SetOutput(Output.SFX);
        navigateLeft.SetOutput(Output.SFX);
        navigateRight.SetOutput(Output.SFX);
        
        // Initialize the accept and cancel sounds.
        acceptSFX = new (SFX.Accept);
        cancelSFX = new (SFX.Cancel);
        
        acceptSFX.SetOutput(Output.SFX);
        cancelSFX.SetOutput(Output.SFX);
        
        // Initialize the close menu sound.
        closeMenu = new (SFX.MenuClose);
        closeMenu.SetOutput(Output.SFX);
        
        // Initialize the switch menu navigation sounds.
        CSNavigateLeft  = new (SFX.CSNavigateLeft);
        CSNavigateRight = new (SFX.CSNavigateRight);
        
        CSNavigateLeft.SetOutput(Output.SFX);
        CSNavigateRight.SetOutput(Output.SFX);
        
        // -- other --
        
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

        if (firstSelected != null && eventSystem != null) 
            eventSystem.SetSelectedGameObject(firstSelected.gameObject);
    }
    
    // Debug button.
    public void OnPressButton()
    {
        Debug.Log($"Player {localPlayerID} pressed a button using \"{playerInput.currentControlScheme}\" control scheme!");
    }
}