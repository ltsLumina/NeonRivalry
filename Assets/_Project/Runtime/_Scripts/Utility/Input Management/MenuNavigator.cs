using System.Linq;
using Lumina.Essentials.Attributes;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
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
    
    public delegate void SelectionChanged(GameObject previousSelectedGameObject, GameObject currentSelectedGameObject);
    public static event SelectionChanged OnSelectionChanged;

    // In the case of the Menu Navigator game object, the PlayerInput is tied to this game object.
    // The Player's UI Navigator, however, is childed to the Player's game object.
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
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
        
        previousSelectedGameObject = eventSystem.currentSelectedGameObject;
    }
    
    public void OnNavigate(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();

        // Check for selection
        if (previousSelectedGameObject != null && eventSystem.currentSelectedGameObject != null)
            if (eventSystem.currentSelectedGameObject != previousSelectedGameObject)
            {
                int previousIndex = previousSelectedGameObject.transform.parent.GetSiblingIndex();
                int currentIndex  = eventSystem.currentSelectedGameObject.transform.parent.GetSiblingIndex();

                if (MainMenuScene)
                {
                    previousIndex = previousSelectedGameObject.transform.GetSiblingIndex();
                    currentIndex  = eventSystem.currentSelectedGameObject.transform.GetSiblingIndex();

                    if (MenuManager.IsAnySettingsMenuActive())
                    {
                        previousIndex = previousSelectedGameObject.transform.parent.GetSiblingIndex();
                        currentIndex  = eventSystem.currentSelectedGameObject.transform.parent.GetSiblingIndex();
                    }
                }

                if (currentIndex      < previousIndex) navigateUp.Play();
                else if (currentIndex > previousIndex) navigateDown.Play();

                OnSelectionChanged?.Invoke(previousSelectedGameObject, eventSystem.currentSelectedGameObject);
                previousSelectedGameObject = eventSystem.currentSelectedGameObject;
            }

        if (MainMenuScene || CharacterSelectScene)
        {
            if (input.x < 0) navigateLeft.Play();
            if (input.x > 0) navigateRight.Play();
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

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (CharacterSelectScene)
        {
            bool isMenuOpen = false;
            
            switch (playerID) 
            {
                case 1:
                    GameObject player1Menu = FindObjectOfType<RebindSaveLoad>().transform.GetChild(0).gameObject;
                    var        player1Diagram     = player1Menu.GetComponentInChildren<DiagramSelector>(true);
                    player1Menu.SetActive(!player1Menu.activeSelf);
                    isMenuOpen = player1Menu.activeSelf;

                    switch (playerInput.currentControlScheme)
                    {
                        case "Gamepad": 
                        {
                            GameObject gamepad = player1Menu.GetComponentInChildren<GamepadIconsExample>(true).gameObject;
                            gamepad.SetActive(!gamepad.activeSelf);
                            
                            // Enable the diagram
                            player1Diagram.ShowDiagram();

                            eventSystem.SetSelectedGameObject(gamepad.transform.GetChild(0)?.GetComponentInChildren<Button>().gameObject);
                            break;
                        }

                        case "Keyboard": 
                        {
                            GameObject keyboard = player1Menu.GetComponentInChildren<KeyboardIconsExample>(true).gameObject;
                            keyboard.SetActive(!keyboard.activeSelf);

                            // Enable the diagram
                            player1Diagram.ShowDiagram();

                            eventSystem.SetSelectedGameObject(keyboard.transform.GetChild(0)?.GetComponentInChildren<Button>().gameObject);
                            break;
                        }
                    }
                    break;

                case 2:
                    GameObject player2Menu    = FindObjectOfType<RebindSaveLoad>().transform.GetChild(1).gameObject;
                    var        player2Diagram = player2Menu.GetComponentInChildren<DiagramSelector>(true);
                    player2Menu.SetActive(!player2Menu.activeSelf);
                    isMenuOpen = player2Menu.activeSelf;
                    
                    switch (playerInput.currentControlScheme)
                    {
                        case "Gamepad": 
                        {
                            GameObject gamepad = player2Menu.GetComponentInChildren<GamepadIconsExample>(true).gameObject;
                            gamepad.SetActive(!gamepad.activeSelf);
                            
                            // Enable the diagram
                            player2Diagram.ShowDiagram();

                            eventSystem.SetSelectedGameObject(gamepad.transform.GetChild(0)?.GetComponentInChildren<Button>().gameObject);
                            break;
                        }

                        case "Keyboard": 
                        {
                            GameObject keyboard = player2Menu.GetComponentInChildren<KeyboardIconsExample>(true).gameObject;
                            keyboard.SetActive(!keyboard.activeSelf);
                            
                            // Enable the diagram
                            player2Diagram.ShowDiagram();

                            eventSystem.SetSelectedGameObject(keyboard.transform.GetChild(0)?.GetComponentInChildren<Button>().gameObject);
                            break;
                        }
                    }
                    break;
            }

            if (!isMenuOpen)
            {
                closeMenu.Play();

                // Set the selection back to the player's character button.
                var characterButton = GameObject.Find($"Player {playerID}").transform.GetChild(0).gameObject.GetComponent<Button>();
                eventSystem.SetSelectedGameObject(characterButton.gameObject);
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