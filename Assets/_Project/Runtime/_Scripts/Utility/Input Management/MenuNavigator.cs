using System.Linq;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
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
        PlayerManager.AddMenuNavigator(this);

        Initialize();
        InitializeAudio();

        // -- other --
        
        string sceneName  = ActiveSceneName;
        int sceneIndex = ActiveScene;
        
        // If the scene is the Intro or Game scene, return, as we don't want to select a button in these scenes.
        if (sceneIndex == Intro || sceneIndex == Game || sceneIndex == Bar || sceneIndex == Street) return;

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

    public bool PlayNavigationSounds { get; set; } = true;
    
    public void OnNavigate(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        
        if (!PlayNavigationSounds) return;

        // Checks the current selected game object and the previous selected game object.
        // If the current selected game object is not the same as the previous selected game object,
        // then play the navigation sound.
        if (previousSelectedGameObject == null || eventSystem.currentSelectedGameObject == null) return;
        if (eventSystem.currentSelectedGameObject == previousSelectedGameObject) return;

        int previousIndex = previousSelectedGameObject.transform.GetSiblingIndex();
        int currentIndex  = eventSystem.currentSelectedGameObject.transform.GetSiblingIndex();

        if (MainMenuScene && MenuManager.IsAnySettingsMenuActive())
        {
            previousIndex = previousSelectedGameObject.transform.parent.GetSiblingIndex();
            currentIndex  = eventSystem.currentSelectedGameObject.transform.parent.GetSiblingIndex();
        }

        if (CharacterSelectScene)
        {
            previousIndex = previousSelectedGameObject.transform.parent.GetSiblingIndex();
            currentIndex  = eventSystem.currentSelectedGameObject.transform.parent.GetSiblingIndex();
        }

        if (CharacterSelectScene)
        {
            ShowCharacterModel(PlayerID);

            // int selectedCharacterIndex = selectedCharacter.transform.GetSiblingIndex();
            // int shelbyIndex            = manager.ShelbyButton.transform.GetSiblingIndex();
            // int morpheIndex            = manager.MorpheButton.transform.GetSiblingIndex();
            //
            // if (selectedCharacterIndex      < shelbyIndex) navigateLeft.Play();
            // else if (selectedCharacterIndex > morpheIndex) navigateRight.Play();
        }

        if (currentIndex      < previousIndex) navigateUp.Play();
        else if (currentIndex > previousIndex) navigateDown.Play();

        OnSelectionChanged?.Invoke(previousSelectedGameObject, eventSystem.currentSelectedGameObject);
        previousSelectedGameObject = eventSystem.currentSelectedGameObject;
    }

    public void ShowCharacterModel(int playerID)
    {
        var manager = FindObjectOfType<CharacterSelectManager>();
        GameObject selectedCharacter;

        GameObject shelbyButton = playerID == 1 ? manager.player1_shelbyButton : manager.player2_shelbyButton;
        GameObject morpheButton = playerID == 1 ? manager.player1_morpheButton : manager.player2_morpheButton;
        
        if (eventSystem.currentSelectedGameObject      == shelbyButton.gameObject) selectedCharacter = shelbyButton.gameObject;
        else if (eventSystem.currentSelectedGameObject == morpheButton.gameObject) selectedCharacter = morpheButton.gameObject;
        else selectedCharacter = null; 
            
        if (selectedCharacter != null)
        {
            //Show the respective character object depending on the selected character.
            // If the selected character is Shelby, show Shelby's model, otherwise show Morphe's model.
            if (selectedCharacter == shelbyButton.gameObject)
            {
                if (playerID == 1)
                {
                    manager.player1_shelbyModel.SetActive(true);
                    manager.player1_morpheModel.SetActive(false);
                    
                    manager.player1_shelbyText.gameObject.SetActive(true);
                    manager.player1_morpheText.gameObject.SetActive(false);
                }
                else
                {
                    manager.player2_shelbyModel.SetActive(true);
                    manager.player2_morpheModel.SetActive(false);
                    
                    manager.player2_shelbyText.gameObject.SetActive(true);
                    manager.player2_morpheText.gameObject.SetActive(false);
                }
            }
            else if (selectedCharacter == morpheButton.gameObject)
            {
                if (playerID == 1)
                {
                    manager.player1_shelbyModel.SetActive(false);
                    manager.player1_morpheModel.SetActive(true);
                    
                    manager.player1_shelbyText.gameObject.SetActive(false);
                    manager.player1_morpheText.gameObject.SetActive(true);
                }
                else
                {
                    manager.player2_shelbyModel.SetActive(false);
                    manager.player2_morpheModel.SetActive(true);
                    
                    manager.player2_shelbyText.gameObject.SetActive(false);
                    manager.player2_morpheText.gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        PlayNavigationSounds = true;
        
        if (context.performed)
        {
            if (MainMenuScene)
            {
                var menuManager = FindObjectOfType<MenuManager>();

                if (menuManager.IsAnyMenuActive())
                {
                    menuManager.CloseCurrentMainMenu(); // Only closes the credits menu.
                }
            }
            
            if (CharacterSelectScene)
            {
                if (CharacterSelectManager.IsAnyMenuActive()) return;

                #region Stop Selecting Map
                var mapSelector = FindObjectOfType<MapSelector>();

                if (mapSelector != null && mapSelector.IsSelecting())
                {
                    mapSelector.gameObject.SetActive(false);

                    // Set selection back to the character button.
                    foreach (var player in PlayerManager.MenuNavigators)
                    {
                        var characterButtons = FindObjectsOfType<CharacterButton>();
                        CharacterSelector.ResetAllNavigation(characterButtons);

                        // Deselect the confirmation button, and reselect the character button.

                        int index = 0; // Default to Shelby.

                        if (CharacterSelector.SelectedCharacters.TryGetValue(player.PlayerID, out Character character))
                        {
                            if (character.characterName      == "Shelby") { index = 0; }
                            else if (character.characterName == "Morphe") { index = 1; }
                        }

                        var buttonToReturnTo = GameObject.Find($"Player {player.PlayerID} CB").transform.GetChild(index).gameObject;
                        player.eventSystem.SetSelectedGameObject(buttonToReturnTo);
                    }
                }
                #endregion
                
                #region Deselect Character
                // Deselect the current character.
                var characterButton = GameObject.Find($"Player {PlayerID} CB").transform.GetChild(0).GetComponent<CharacterButton>();
                
                // If there is a character selected, deselect it.
                if (CharacterSelector.DeselectCharacter(characterButton.PlayerIndex, eventSystem))
                {
                    cancelSFX.Play();

                    // Check if the confirmation button is showing.
                    // If it is, hide it.
                    var    confirmCharacters       = FindObjectOfType<ConfirmCharactersButton>();
                    Button confirmCharactersButton = confirmCharacters.transform.GetChild(0).GetComponent<Button>();

                    if (confirmCharacters != null && confirmCharactersButton.gameObject.activeSelf)
                    {
                        // Hide the button.
                        confirmCharactersButton.gameObject.SetActive(false);

                        // Play the cancel sound.
                        cancelSFX.Play();

                        foreach (var player in PlayerManager.MenuNavigators)
                        {
                            // Deselect the confirmation button, and reselect the character button.
                            var buttonToReturnTo = GameObject.Find($"Player {player.PlayerID} CB").transform.GetChild(0).gameObject;
                            player.eventSystem.SetSelectedGameObject(buttonToReturnTo);
                        }
                    }
                }
                #endregion
            }
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (MainMenuScene)
        {
            var menuManager = FindObjectOfType<MenuManager>();
            if (menuManager == null) return;
            
            menuManager.ResetDefaults();
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
        if (MainMenuScene)
        {
            var menuManager = FindObjectOfType<MenuManager>();
            if (menuManager == null) return;
        
            menuManager.CloseCurrentSettingsMenu();
        }

        if (CharacterSelectScene)
        {
            var mapSelector = FindObjectOfType<MapSelector>();
            if (mapSelector.IsSelecting()) return;
            
            var characterSelectManager = FindObjectOfType<CharacterSelectManager>();
            characterSelectManager.ToggleCharacterSettingsMenu(playerID);
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
                return GameObject.Find($"Player {PlayerID} CB").transform.GetChild(0).gameObject;

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