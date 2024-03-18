using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class SettingsManagerNEW : MonoBehaviour
{
    [Tab("Main")]
    [Tooltip("Populated by the Start() method.")]
    [Space]
    [SerializeField] List<GameObject> mainMenus;
    [Space]
    [SerializeField] Image creditsBackground;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject creditsMenu;
    
    [Tab(" Settings ")]
    [Tooltip("Populated by the Start() method.")]
    [Space]
    [SerializeField] List<GameObject> settingsMenus;
    [SerializeField] List<Button> settingsHeaders; // The buttons at the top of the settings menu. ("Game", "Audio", "Video", etc.)
    [SerializeField] List<GameObject> settingsContents;
    [Space]
    [SerializeField] Image settingsBackground;
    
    [Foldout("Game")]
    [SerializeField] GameObject gameMenu;
    [SerializeField] Button gameHeader;
    [SerializeField] GameObject gameContent;
    [SerializeField] Slider screenshakeSlider;
    [SerializeField] Slider rumbleSlider;
    
    [Foldout("Audio")]
    [SerializeField] GameObject audioMenu;
    [SerializeField] Button audioHeader;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    
    [Foldout("Video")]
    [SerializeField] GameObject videoMenu;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Dropdown qualityDropdown;
    [SerializeField] Toggle fullscreenToggle;
    
    [Foldout("Gamepad")]
    [SerializeField] GameObject gamepadMenu;
    
    [Foldout("Keyboard")]
    [SerializeField] GameObject keyboardMenu;
    [EndFoldout]
    
    [Header("Tweening")]
    [Space]
    [SerializeField] float backgroundOpacity = 0.5f;
    [SerializeField] float backgroundTweenDuration = 0.5f;

    // === === === ===  Cached References  === === === === \\
    
    UIManager UIManager;
    EventSystemSelector player;
    
    // === === === ===  Private Variables  === === === === \\
    
    bool isCreditsActive;

    #region VInpector fix
#pragma warning disable CS0414 // Field is assigned but its value is never used
    bool disabled = true;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    #endregion

    // === === === === ===  Other  === === === === === \\
    
    #region Button Methods
    // These methods are meant to be used by the UI buttons.
    public void ToggleSettingsMenu() => ToggleSettings();
    public void ToggleGameMenu() => ToggleMenu(gameMenu);
    public void ToggleAudioMenu() => ToggleMenu(audioMenu);
    public void ToggleVideoMenu() => ToggleMenu(videoMenu);
    public void ToggleGamepadMenu() => ToggleMenu(gamepadMenu);
    public void ToggleKeyboardMenu() => ToggleMenu(keyboardMenu);
    #endregion

    void Awake()
    {
        // References
        UIManager = FindObjectOfType<UIManager>();
    }

    void Start()
    {
        // Populate the settings menus, headers, and contents lists and disable it all.
        settingsMenus.AddRange(new List<GameObject> {gameMenu, audioMenu, videoMenu, gamepadMenu, keyboardMenu});
        settingsHeaders.AddRange(new List<Button> {gameHeader, audioHeader});
        settingsContents.AddRange(new List<GameObject> {gameContent});
        settingsMenus.ForEach(menu => menu.SetActive(false));
        settingsContents.ForEach(content => content.SetActive(false));
        
        // Disable all the headers.
        gameHeader.gameObject.SetActive(false);
        audioHeader.gameObject.SetActive(false);
        
        // Disable the backgrounds.
        settingsBackground.gameObject.SetActive(false);
        
        // Disable the main menus.
        mainMenus.AddRange(new List<GameObject> {mainMenu, creditsMenu});
        mainMenus.ForEach(menu => menu.SetActive(false));
        mainMenu.SetActive(true);
        
        // Disable the credits background.
        creditsBackground.gameObject.SetActive(false);
    }

    public void ScaleUpSlider() => screenshakeSlider.transform.parent.DOScale(1.05f, 0.1f).SetEase(Ease.OutBack);

    public void ScaleDownSlider() => screenshakeSlider.transform.parent.DOScale(1, 0.1f).SetEase(Ease.InBack);

    void ToggleSettings()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        
        ToggleGameMenu();
    }
    
    void ToggleMenu(GameObject menu)
    {
        GameObject parent = menu.transform.parent.gameObject;
        
        bool isParentMenuActive = parent.activeSelf;
        bool isMenuActive = menu.activeSelf;

        #region Activate Menu
        // If the parent is inactive, enable the parent and child menu.
        if (!isParentMenuActive)
        {
            parent.SetActive(true);
            menu.SetActive(true);
        }
        // If the parent is active, enable the child menu if it's inactive.
        else if (!isMenuActive)
        {
            menu.SetActive(true);
        }
        #endregion

        if (menu == creditsMenu)
        {
            creditsBackground.gameObject.SetActive(true);
            creditsBackground.DOFade(backgroundOpacity, backgroundTweenDuration);
        }

        // Regardless of which menu is active, enable the background.
        if (menu == mainMenus.Any())
        {
            settingsBackground.gameObject.SetActive(true);
        }
        
        if (menu == gameMenu)
        {
            settingsHeaders.ForEach(header => header.gameObject.SetActive(true));
            settingsContents.ForEach(content => content.SetActive(false));

            gameMenu.SetActive(true);
            gameHeader.gameObject.SetActive(true);
            gameContent.SetActive(true);
            UIManager.SelectButtonByReference(gameHeader);
        }
        
        if (menu == audioMenu)
        {
            settingsHeaders.ForEach(header => header.gameObject.SetActive(true));
            settingsContents.ForEach(content => content.SetActive(false));
            //audioContent.SetActive(true);
            UIManager.SelectButtonByReference(audioHeader);
        }
    }

    public void ToggleCredits(Button creditsButton)
    {
        //TextMeshProUGUI creditsText = creditsMenu.GetComponentInChildren<TextMeshProUGUI>();
        GameObject parent = creditsMenu.gameObject.transform.parent.gameObject;

        // If the credits menu is inactive, fade it in,
        if (!parent.activeSelf)
        {
            creditsButton.interactable = false;
            
            parent.SetActive(true);
            creditsBackground.gameObject.SetActive(true);
        
            Sequence sequence = DOTween.Sequence();
        
            // If the credits menu is active, fade it out and shrink Y from 1 to 0. If it's inactive, fade it in and scale Y from 0 to 1.
            sequence.Append(creditsBackground.DOFade(isCreditsActive ? 0 : backgroundOpacity, backgroundTweenDuration));
            sequence.Join(creditsBackground.transform.DOScaleY(isCreditsActive ? 0 : 1, backgroundTweenDuration));
        
            sequence.OnComplete
            (() =>
            {
                // Toggle the credits menu
                creditsMenu.SetActive(true);
            });
        }
        // If it's active, fade it out.
        else
        {
            // Toggle the credits menu
            creditsMenu.SetActive(false);
            
            Sequence sequence = DOTween.Sequence();
        
            // If the settings menu is active, fade it out and shrink Y from 1 to 0. If it's inactive, fade it in and scale Y from 0 to 1.
            sequence.Append(creditsBackground.DOFade(isCreditsActive ? backgroundOpacity : 0, backgroundTweenDuration));
            sequence.Join(creditsBackground.transform.DOScaleY(isCreditsActive ? 1 : 0, backgroundTweenDuration));
        
            sequence.OnComplete
            (() =>
            {
                // Toggle the background & parent menu
                creditsBackground.gameObject.SetActive(false);
                parent.SetActive(false);
            });
        }
    }

    void CloseCredits()
    {
        GameObject parent = creditsMenu.gameObject.transform.parent.gameObject;
        
        // Toggle the credits menu
        creditsMenu.SetActive(false);

        Sequence sequence = DOTween.Sequence();

        // If the settings menu is active, fade it out and shrink Y from 1 to 0. If it's inactive, fade it in and scale Y from 0 to 1.
        sequence.Append(creditsBackground.DOFade(isCreditsActive ? backgroundOpacity : 0, backgroundTweenDuration));
        sequence.Join(creditsBackground.transform.DOScaleY(isCreditsActive ? 1 : 0, backgroundTweenDuration));

        sequence.OnComplete
        (() =>
        {
            // Toggle the background & parent menu
            creditsBackground.gameObject.SetActive(false);
            parent.SetActive(false);
            
            UIManager UIManager = FindObjectOfType<UIManager>();
            UIManager.MainMenuButtons[2].interactable = true;
            UIManager.MainMenuButtons[2].Select();
        });
    }
    
    public void CloseCurrentMainMenu()
    {
        // Find the active settings menu.
        GameObject currentMenu = mainMenus.Find(menu => menu.activeSelf && menu != mainMenu);
        
        if (currentMenu == null) return;
        Debug.Log(currentMenu, currentMenu);
        
        // The credits menu has a unique close method.
        if (currentMenu == creditsMenu)
        {
            CloseCredits();
            return;
        }

        // Close the active settings menu.
        currentMenu.SetActive(false);
    }

    public void CloseCurrentSettingsMenu()
    {
        // Find the active settings menu.
        GameObject currentMenu = settingsMenus.Find(menu => menu.activeSelf);
        
        if (currentMenu == null) return;
        
        // Close the active settings menu.
        currentMenu.SetActive(false);
        
        // Disable all settings menus
        settingsMenus.ForEach(menu => menu.SetActive(false));
        
        // Enable the main menu
        mainMenu.SetActive(true);

        // Disable the background.
        creditsBackground.gameObject.SetActive(false);
        settingsBackground.gameObject.SetActive(false);
        
        // Select the "Settings" button.
        UIManager.MainMenuButtons[1].Select();
    }
}

public class Menu
{
    public GameObject MenuObject { get; set; }
    public Button Header { get; set; }
    public GameObject Content { get; set; }
}
