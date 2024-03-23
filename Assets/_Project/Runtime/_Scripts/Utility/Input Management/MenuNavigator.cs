using System.Linq;
using Lumina.Essentials.Attributes;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static SceneManagerExtended;

/// <summary>
/// The <see cref="MenuNavigator"/> class is responsible for navigating through the UI in every scene.
/// It is also responsible for selecting the first button in the scene upon a player joining.
/// </summary>
[RequireComponent(typeof(MultiplayerEventSystem))]
public class MenuNavigator : MonoBehaviour
{
    [SerializeField, ReadOnly] int playerID;
    
    // -- Fields --
    PlayerInput playerInput;

    Sound navigateUp;
    Sound navigateDown;
    Sound navigateLeft;
    Sound navigateRight;
    Sound cancelSFX;
    Sound closeMenu;
    Sound CSNavigateLeft;
    Sound CSNavigateRight;
    
    // -- Cached References --
    
    MultiplayerEventSystem eventSystem;
    GameObject previousSelectedGameObject;

    // In the case of the Menu Navigator game object, the PlayerInput is tied to this game object.
    // The Player's UI Navigator, however, is childed to the Player's game object.
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null) transform.parent.GetComponentInChildren<PlayerInput>();
        eventSystem = GetComponent<MultiplayerEventSystem>();
    }
    
    void Start()
    {
        InitializeAudio();

        // -- other --
        
        string sceneName  = ActiveSceneName;
        int sceneIndex = ActiveScene;

        // If the scene is the Intro or Game scene, return, as we don't want to select a button in these scenes.
        if (sceneIndex == Intro || sceneIndex == Game) return;

        // Assign the localPlayerID based on the playerInput's playerIndex.
        // Usually we would use player.PlayerID, but there is no "player" instance until the game scene.
        playerID = playerInput.playerIndex + 1;

        if (playerID == 2 && IsNotSupportedForPlayer2(sceneIndex))
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

        ProcessButton(button.GetComponent<Button>());
    }
    
    public void OnNavigate(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        
        if (ActiveScene == MainMenu)
        {
            if (input.y > 0) navigateUp.Play();
            if (input.y < 0 ) navigateDown.Play();
            if (input.x < 0) navigateLeft.Play();
            if (input.x > 0) navigateRight.Play();
        }
        
        if (ActiveScene == CharacterSelect)
        {
            GameObject marker = GameObject.Find($"P{playerID} Marker");
            marker.SetActive(true);
    
            marker.GetComponent<RectTransform>().anchoredPosition = eventSystem.currentSelectedGameObject.name switch
            { "Shelby"  => new (-100, -325),
              "Dorathy" => new (100, -325),
              _         => marker.GetComponent<RectTransform>().anchoredPosition };

            // Audio
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        // cancelSFX.Play();
        
        if (context.performed)
        {
            if (ActiveScene == MainMenu)
            {
                var menuManager = FindObjectOfType<MenuManager>();

                if (menuManager.IsAnyMenuActive())
                {
                    menuManager.CloseCurrentMainMenu();
                    menuManager.CloseCurrentSettingsMenu();
                    closeMenu.Play();
                }
            }
            
            if (ActiveScene == CharacterSelect)
            {
                eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().CharacterSelected = false;
                CharacterSelector.DeselectCharacter(playerID, GameObject.Find($"Player {playerID}").transform.GetChild(0).GetComponent<Button>());
            }
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
    
    void InitializeAudio()
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
        cancelSFX = new (SFX.Cancel);
        cancelSFX.SetOutput(Output.SFX);
        
        // Initialize the close menu sound.
        closeMenu = new (SFX.MenuClose);
        closeMenu.SetOutput(Output.SFX);
        
        // Initialize the switch menu navigation sounds.
        CSNavigateLeft  = new (SFX.CSNavigateLeft);
        CSNavigateRight = new (SFX.CSNavigateRight);
        
        CSNavigateLeft.SetOutput(Output.SFX);
        CSNavigateRight.SetOutput(Output.SFX);
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
            case var _ when sceneIndex == MainMenu: 
                return GameObject.Find("Play");
            
            case var _ when sceneIndex == CharacterSelect:

                return GameObject.Find($"Player {playerID}").transform.GetChild(0).gameObject;

            default:
                return null;
        }
    }

    void ProcessButton(Button button)
    {
        if (button != null && eventSystem != null) 
            eventSystem.SetSelectedGameObject(button.gameObject);

        //button.Select();
        
        // if (CurrentSelectedGameObject.name == "Shelby")
        // {
        //     GameObject P1Marker = GameObject.Find("P1 Marker");
        //     P1Marker.SetActive(true);
        //     P1Marker.GetComponent<RectTransform>().anchoredPosition = new (-100, -325);
        // }
        // else if (CurrentSelectedGameObject.name == "Dorathy")
        // {
        //     GameObject P1Marker = GameObject.Find("P1 Marker");
        //     P1Marker.SetActive(true);
        //     P1Marker.GetComponent<RectTransform>().anchoredPosition = new (100, -325);
        // }
    }
    
    // Debug button.
    public void OnPressButton()
    {
        Debug.Log($"Player {playerID} pressed a button using \"{playerInput.currentControlScheme}\" control scheme!");
    }
}