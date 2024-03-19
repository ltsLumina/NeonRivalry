using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class MenuManager : MonoBehaviour
{
    [Tab("Main")]
    [Foldout("Main")]
      [Tooltip(tooltip)]
    [SerializeField] List<GameObject> mainMenus;
      [Tooltip(tooltip)]
    [SerializeField] List<Button> mainMenuButtons;
      [Tooltip(tooltip)]
    [SerializeField] List<GameObject> mainContents;
    [Space]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainContent;
    
    [Header("Buttons")]
    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button creditsButton;

    [Foldout("Credits")]
    [SerializeField] GameObject creditsMenu;
    [Space]
    [SerializeField] GameObject creditsContent;
    [SerializeField] Image creditsBackground;
    
    [Tab(" Settings ")]
    [SerializeField] Image settingsBackground;
    [SerializeField] TextMeshProUGUI settingsTitle;
    [Space]
    [SerializeField] List<GameObject> settingsMenus;
      [Tooltip(tooltip)]
    [SerializeField] List<Button> settingsHeaders; // The buttons at the top of the settings menu. ("Game", "Audio", "Video", etc.)
      [Tooltip(tooltip)]
    [SerializeField] List<GameObject> settingsContents;

    [SerializeField] GameObject settingsHeader;
    
    [Foldout("Game")]
    [SerializeField] GameObject gameMenu;
    [SerializeField] Button gameHeader;
    
    [Header("Content")]
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
    
    [Tab("Tweening")]
    [Space]
    [SerializeField] float backgroundOpacity = 0.5f;
    [SerializeField] float backgroundTweenDuration = 0.5f;

    // === === === ===  Cached References  === === === === \\
    
    UIManager UIManager;
    EventSystemSelector player;
    CreditsManager creditsManager;
    
    // === === === ===  Private Variables  === === === === \\
    
    const string tooltip = "Populated by the Start() method. This list should not be populated manually.";
    bool isCreditsActive => creditsMenu.activeSelf;

    #region VInpector fix
#pragma warning disable CS0414 // Field is assigned but its value is never used
    bool disabled = true;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    #endregion

    // === === === === ===  Other  === === === === === \\
    
    #region Button Methods
    // These methods are meant to be used by the UI buttons.
    public void ToggleGameMenu() => ToggleSettingsMenu(gameMenu);
    public void ToggleAudioMenu() => ToggleSettingsMenu(audioMenu);
    public void ToggleVideoMenu() => ToggleSettingsMenu(videoMenu);
    public void ToggleGamepadMenu() => ToggleSettingsMenu(gamepadMenu);
    public void ToggleKeyboardMenu() => ToggleSettingsMenu(keyboardMenu);
    #endregion

    void Awake()
    {
        // References
        UIManager = FindObjectOfType<UIManager>();
        creditsManager = FindObjectOfType<CreditsManager>();
    }

    void OnEnable()
    {
        creditsManager.onCreditsEnd.AddListener(CloseCredits);
    }

    void Start()
    {
        // Populate the main lists, disable all menus, and enable the main menu.
        mainMenus.AddRange(new List<GameObject> {mainMenu, creditsMenu});
        mainContents.AddRange(new List<GameObject> {mainContent, creditsContent});
        mainMenus.ForEach(menu => menu.SetActive(false));
        mainMenu.SetActive(true);
        
        // Populate and enable the main menu buttons.
        mainMenuButtons.AddRange(new List<Button> {playButton, settingsButton, quitButton, creditsButton});
        mainMenuButtons.ForEach(button => button.gameObject.SetActive(true));
        
        // Populate the settings menus, headers, and contents lists and disable it all.
        settingsMenus.AddRange(new List<GameObject> {gameMenu, audioMenu, videoMenu, gamepadMenu, keyboardMenu});
        settingsHeaders.AddRange(new List<Button> {gameHeader, audioHeader});
        settingsHeader.SetActive(false);
        settingsTitle.gameObject.SetActive(false);
        settingsContents.AddRange(new List<GameObject> {gameContent});
        settingsContents.ForEach(content => content.SetActive(false));
        
        // Disable the settings background.
        settingsBackground.gameObject.SetActive(false);
    }

    public void ScaleUpSlider() => screenshakeSlider.transform.parent.DOScale(1.05f, 0.1f).SetEase(Ease.OutBack);

    public void ScaleDownSlider() => screenshakeSlider.transform.parent.DOScale(1, 0.1f).SetEase(Ease.InBack);

    public void ToggleSettings()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        settingsHeader.SetActive(!settingsHeader.activeSelf);
        
        ToggleGameMenu();
    }

    void ToggleMainMenu(GameObject menu)
    {
        bool isMenuActive = menu.activeSelf;

        menu.SetActive(!isMenuActive);

        if (menu == creditsMenu) { ToggleCredits(); }
    }
    
    void ToggleSettingsMenu(GameObject menu)
    {
        bool isMenuActive = menu.activeSelf;
        
        menu.SetActive(!isMenuActive);

        // Regardless of which menu is active, enable the background. (other than the credits menu)
        if (menu == mainMenus.Any(m => m != creditsMenu))
        {
            settingsBackground.gameObject.SetActive(true);
            
            settingsMenus.ForEach(m => m.SetActive(false));
        }
        
        if (menu == gameMenu)
        {
            settingsBackground.gameObject.SetActive(true);
            settingsTitle.gameObject.SetActive(true);
            
            settingsHeaders.ForEach(header => header.gameObject.SetActive(true));
            settingsContents.ForEach(content => content.SetActive(false));

            gameMenu.SetActive(true);
            gameHeader.gameObject.SetActive(true);
            gameContent.SetActive(true);
            UIManager.SelectButtonByReference(gameHeader);
        }
        
        if (menu == audioMenu)
        {
            settingsContents.ForEach(content => content.SetActive(false));
            settingsMenus.ForEach(m => m.SetActive(false));
            audioMenu.SetActive(true);
            //audioContent.SetActive(true);
            UIManager.SelectButtonByReference(audioHeader);
        }
        
        if (menu == videoMenu)
        {
            settingsContents.ForEach(content => content.SetActive(false));
            settingsMenus.ForEach(m => m.SetActive(false));
            videoMenu.SetActive(true);
            //videoContent.SetActive(true);
            UIManager.SelectButtonByReference(audioHeader);
        }
        
        if (menu == gamepadMenu)
        {
            settingsContents.ForEach(content => content.SetActive(false));
            settingsMenus.ForEach(m => m.SetActive(false));
            gamepadMenu.SetActive(true);
            //gamepadContent.SetActive(true);
            UIManager.SelectButtonByReference(audioHeader);
        }
        
        if (menu == keyboardMenu)
        {
            settingsContents.ForEach(content => content.SetActive(false));
            settingsMenus.ForEach(m => m.SetActive(false));
            keyboardMenu.SetActive(true);
            //keyboardContent.SetActive(true);
            UIManager.SelectButtonByReference(audioHeader);
        }
    }

    public void ToggleCredits()
    {
        creditsManager.ResetCredits();
        
        // If the credits menu is inactive, fade it in,
        if (!creditsMenu.activeSelf)
        {
            creditsButton.interactable = false;
            
            creditsMenu.SetActive(true);
            creditsContent.SetActive(true);

            creditsContent.GetComponent<CanvasGroup>().alpha = 0;
        
            Sequence sequence = DOTween.Sequence();
            
            // Fade in the background and scale Y from 0 to 1.
            sequence.Append(creditsBackground.DOFade(isCreditsActive ? backgroundOpacity : 0, backgroundTweenDuration));
            sequence.Join(creditsBackground.transform.DOScaleY(isCreditsActive ? 1 : 0, backgroundTweenDuration));

            // Fade in the texts
            sequence.AppendInterval(backgroundTweenDuration);
            sequence.Append(creditsContent.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
        
            sequence.OnComplete
            (() =>
            {
                // do something
            });
        }
        // If it's active, fade it out.
        else
        {
            Sequence sequence = DOTween.Sequence();
        
            // If the settings menu is active, fade it out and shrink Y from 1 to 0. If it's inactive, fade it in and scale Y from 0 to 1.
            sequence.Append(creditsBackground.DOFade(isCreditsActive ? backgroundOpacity : 0, backgroundTweenDuration));
            sequence.Join(creditsBackground.transform.DOScaleY(isCreditsActive ? 1 : 0, backgroundTweenDuration));
        
            sequence.OnComplete
            (() =>
            {
                // Toggle the credits menu
                creditsMenu.SetActive(false);
                
                creditsButton.interactable = true;
                UIManager.SelectButtonByReference(creditsButton);
            });
        }
    }

    void CloseCredits()
    {
        Sequence sequence = DOTween.Sequence();

        // Fade out the canvasgroup
        sequence.Append(creditsContent.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
        sequence.AppendInterval(1f);
        
        // Fade out the background and scale Y from 1 to 0.
        sequence.Append(creditsBackground.DOFade(0, backgroundTweenDuration));
        sequence.Join(creditsBackground.transform.DOScaleY(0, backgroundTweenDuration));
        creditsButton.interactable = true;

        sequence.OnComplete
        (() =>
        {
            // Toggle the credits menu
            creditsMenu.SetActive(false);

            creditsButton.interactable = true;
            UIManager.SelectButtonByReference(creditsButton);
        });
    }
    
    public void CloseCurrentMainMenu()
    {
        // Find the active settings menu.
        GameObject currentMenu = mainMenus.Find(menu => menu.activeSelf && menu != mainMenu);
        
        if (currentMenu == null) return;
        
        // The credits menu has a unique close method.
        if (currentMenu == creditsMenu)
        {
            ToggleCredits();
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
        
        // Disable all settings menus
        settingsMenus.ForEach(menu => menu.SetActive(false));
        
        // Disable the header and title
        settingsHeader.SetActive(false);
        settingsTitle.gameObject.SetActive(false);
        
        // Enable the main menu
        mainMenu.SetActive(true);

        // Disable the background.
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
