using System.Linq;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public int PlayerID
    {
        get => playerID;
        set => playerID = value;
    }
    
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

    public GameObject PreviousSelectedGameObject
    {
        get => previousSelectedGameObject;
        set => previousSelectedGameObject = value;
    }

    public delegate void SelectionChanged(GameObject previousSelectedGameObject, GameObject currentSelectedGameObject);
    public static event SelectionChanged OnSelectionChanged;

    // In the case of the Menu Navigator game object, the PlayerInput is tied to this game object.
    // The Player's UI Navigator, however, is childed to the Player's game object.
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        eventSystem = GetComponent<MultiplayerEventSystem>();
    }

    void OnEnable()
    {
        // Get all EventTrigger components in the scene
        EventTrigger[] eventTriggers = FindObjectsOfType<EventTrigger>(true);

        foreach (EventTrigger eventTrigger in eventTriggers)
        {
            // Check if the OnSelect event trigger already exists
            bool onSelectExists = eventTrigger.triggers.Any(entry => entry.eventID == EventTriggerType.Select);

            // If the OnSelect event trigger does not exist, add it
            if (!onSelectExists)
            {
                // Create a new EventTrigger.Entry
                EventTrigger.Entry entry = new EventTrigger.Entry();

                // Set the event type
                entry.eventID = EventTriggerType.Select;

                // Set the callback function
                entry.callback.AddListener
                (_ =>
                {
                    int previousIndex = previousSelectedGameObject.transform.parent.GetSiblingIndex();
                    int currentIndex  = eventSystem.currentSelectedGameObject.transform.parent.GetSiblingIndex();

                    if (currentIndex      < previousIndex) navigateUp.Play();
                    else if (currentIndex > previousIndex) navigateDown.Play();
                });

                // Add the entry to the triggers list
                eventTrigger.triggers.Add(entry);
            }
        }
    }

    void Start()
    {
        PlayerManager.AddPlayer(this);

        Initialize();
        InitializeAudio();

        // -- other --
        
        string sceneName  = ActiveSceneName;
        int sceneIndex = ActiveScene;
        
        // If the scene is the Intro or Game scene, return, as we don't want to select a button in these scenes.
        if (sceneIndex == Intro || sceneIndex == Game) return;

        if (playerID == 2 && SceneNotSupportedForPlayer2(sceneIndex))
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
        
        previousSelectedGameObject = eventSystem.currentSelectedGameObject;
    }
    
    void Initialize()
    {
        if (!GameScene)
        {
            // Update the player's ID.
            PlayerID = playerInput.playerIndex + 1;
        
            // Set the player's default control scheme and action map.
            playerInput.defaultControlScheme = InputDeviceManager.GetDevice(playerInput) is Keyboard ? "Keyboard" : "Gamepad";
            playerInput.defaultActionMap     = "UI";

            // Switch the player's action map to the correct player.
            playerInput.actions.Disable();
            playerInput.SwitchCurrentActionMap("UI");
            playerInput.actions.Enable();
        }
        else // (Game-scene specific):
        {
            // The PlayerInput component is in a different game object on the Player prefab.
            if (!playerInput) playerInput = transform.GetSibling(0).GetComponent<PlayerInput>();

            // Update the player's ID.
            PlayerID = playerInput.playerIndex + 1;
        }
        
        // If intro scene, disable the player.
        if (IntroScene) playerInput.enabled = false;

        // Player has been fully initialized.
        // Invoke the OnPlayerJoin event from the InputDeviceManager.
        InputDeviceManager.TriggerPlayerJoin(playerInput, PlayerID);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();

        // Checks the current selected game object and the previous selected game object.
        // If the current selected game object is not the same as the previous selected game object,
        // then play the navigation sound.
        if (previousSelectedGameObject != null && eventSystem.currentSelectedGameObject != null)
            if (eventSystem.currentSelectedGameObject != previousSelectedGameObject)
            {
                if (MainMenuScene)
                {
                    int previousIndex = previousSelectedGameObject.transform.GetSiblingIndex();
                    int currentIndex  = eventSystem.currentSelectedGameObject.transform.GetSiblingIndex();

                    if (MenuManager.IsAnySettingsMenuActive())
                    {
                        previousIndex = previousSelectedGameObject.transform.parent.GetSiblingIndex();
                        currentIndex  = eventSystem.currentSelectedGameObject.transform.parent.GetSiblingIndex();
                    }

                    if (currentIndex      < previousIndex) navigateUp.Play();
                    else if (currentIndex > previousIndex) navigateDown.Play();
                }

                OnSelectionChanged?.Invoke(previousSelectedGameObject, eventSystem.currentSelectedGameObject);
                previousSelectedGameObject = eventSystem.currentSelectedGameObject;
            }

        if (CharacterSelectScene)
        {
            
            
            // if (input.x < 0) navigateLeft.Play();
            // if (input.x > 0) navigateRight.Play();
        }
        
        // if (CharacterSelectScene)
        // {
        //     GameObject marker = GameObject.Find($"P{playerID} Marker");
        //     marker.SetActive(true); // bug: this throws an error OnDestroy/OnDisable??
        //
        //     marker.GetComponent<RectTransform>().anchoredPosition = eventSystem.currentSelectedGameObject.name switch
        //     { "Shelby"  => new (-100, -325),
        //       "Dorathy" => new (100, -325),
        //       _         => marker.GetComponent<RectTransform>().anchoredPosition };
        //
        //     // Audio
        // }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        // cancelSFX.Play();
        
        if (context.performed)
        {
            if (MainMenuScene)
            {
                var menuManager = FindObjectOfType<MenuManager>();

                if (menuManager.IsAnyMenuActive())
                {
                    menuManager.CloseCurrentMainMenu();
                    menuManager.CloseCurrentSettingsMenu();
                    closeMenu.Play();
                }
            }
            
            // if (CharacterSelectScene)
            // {
            //     eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().CharacterSelected = false;
            //     CharacterSelector.DeselectCharacter(playerID, GameObject.Find($"Player {playerID}").transform.GetChild(0).GetComponent<Button>());
            // }
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

    public void OnMenu(InputAction.CallbackContext context) => CharacterSelectManager.ToggleCharacterSettingsMenu(playerID);

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

    static bool SceneNotSupportedForPlayer2(int sceneIndex)
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