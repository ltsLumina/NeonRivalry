using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] TextMeshProUGUI settingsInfo;
    
    [Foldout("On-Screen Keys")]
    [Header("On-Screen Keys")]
    [SerializeField] Image QKeyIcon;
    [SerializeField] Image EKeyIcon;
    [SerializeField] GameObject keyboardBottomLeftIcons;
    [SerializeField] Image LBKeyIcon;
    [SerializeField] Image RBKeyIcon;
    [SerializeField] GameObject gamepadBottomLeftIcons;
    [EndFoldout]
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
    [SerializeField] Slider player1RumbleSlider;
    [SerializeField] Slider player2RumbleSlider;
    
    [Foldout("Audio")]
    [SerializeField] GameObject audioMenu;
    [SerializeField] Button audioHeader;

    [Header("Content")]
    [SerializeField] GameObject audioContent;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    
    [Foldout("Video")]
    [SerializeField] GameObject videoMenu;
    [SerializeField] Button videoHeader;
    
    [Header("Content")]
    [SerializeField] GameObject videoContent;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Toggle vSyncToggle;
    
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
    
    CreditsManager creditsManager;
    
    // === === === ===  Private Variables  === === === === \\
    
    const string tooltip = "Populated at runtime. This list should not be populated manually.";
    
    List<string> resolutions;
    Sound openMenu;
    Sound closeMenu;
    Music music;

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
        creditsManager = FindObjectOfType<CreditsManager>();
    }

    void OnEnable()
    {
        creditsManager.onCreditsEnd.AddListener(CloseCredits);
    }

    void Start()
    {
        // Load PlayerPrefs settings.
        SettingsManager.LoadRumble(player1RumbleSlider, player2RumbleSlider);
        SettingsManager.LoadVolume(masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider);
        
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
        settingsMenus.ForEach(menu => menu.SetActive(false));
        settingsTitle.gameObject.SetActive(false);
        settingsInfo.gameObject.SetActive(false);
        settingsHeaders.AddRange(new List<Button> {gameHeader, audioHeader, videoHeader});
        settingsHeader.SetActive(false);
        settingsContents.AddRange(new List<GameObject> {gameContent, audioContent, videoContent});
        settingsContents.ForEach(content => content.SetActive(false));
        
        // Disable the settings background.
        settingsBackground.gameObject.SetActive(false);
        
        // update slider text
        player1RumbleSlider.onValueChanged.AddListener(_ => UpdateSliderText(player1RumbleSlider));
        player2RumbleSlider.onValueChanged.AddListener(_ => UpdateSliderText(player2RumbleSlider));
        
        // Initialize sounds
        openMenu = new (SFX.MenuOpen);
        closeMenu = new (SFX.MenuClose);

        openMenu.SetOutput(Output.SFX);
        closeMenu.SetOutput(Output.SFX);
        
        // Populate the resolution dropdown with the most common resolutions.
        resolutions = new() { "3840x2160", "2560x1440", "1920x1080", "1366x768", "1280x720", };
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions);
        
        // Adjust the quality dropdown to match the current quality setting.
        resolutionDropdown.value = resolutions.IndexOf($"{Screen.currentResolution.width}x{Screen.currentResolution.height}");
        qualityDropdown.value    = PlayerPrefs.GetInt("Quality", 0);
        fullscreenToggle.isOn    = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        vSyncToggle.isOn         = PlayerPrefs.GetInt("VSync", 0)      == 1;
    }

    public void ScaleUpSliderButton(Selectable parent) => parent.transform.DOScale(1.05f, 0.1f).SetEase(Ease.OutBack);
    public void ScaleDownSliderButton(Selectable parent) => parent.transform.DOScale(1, 0.1f).SetEase(Ease.InBack);

    static void UpdateSliderText(Slider slider)
    {
        // iterate over all the children with the TMP component
        foreach (var text in slider.GetComponentsInChildren<TextMeshProUGUI>())
        {
            // Get the one with the "Current Value Text" tag.
            if (text.CompareTag("Current Value Text"))
            {
                // Set the text to the slider's value.
                text.text = slider.value.ToString("0.00");
            }
        }
    }

    void Update()
    {
        ChangeHeaderColour();

        // Get the player's playerinput
        if (PlayerInput.all.Count == 0) return;
        var action = PlayerInput.all[0].actions["SwitchSettingsMenu"];
        var input       = action.ReadValue<Vector2>();

        if (!action.triggered) return;
        
        GameObject currentMenu = settingsMenus.Find(menu => menu.activeSelf);
        GameObject newMenu;
        
        if (currentMenu == null) return;

        // Q to go left.
        if (input.x < 0)
        {
            currentMenu.SetActive(false);
            newMenu = settingsMenus[Mathf.Clamp(settingsMenus.IndexOf(currentMenu) - 1, 0, settingsMenus.Count - 1)];
            ToggleSettingsMenu(newMenu);
        }
        // E to go right.
        else
        {
            currentMenu.SetActive(false);
            newMenu = settingsMenus[Mathf.Clamp(settingsMenus.IndexOf(currentMenu) + 1, 0, settingsMenus.Count - 1)];
            ToggleSettingsMenu(newMenu);
        }
    }

    void ChangeHeaderColour()
    {
        var currentMenu = settingsMenus.Find(menu => menu.activeSelf);
        if (currentMenu == null) return;

        if (currentMenu == gameMenu)
        {
            // Change the colour of the header to match the selected menu.
            var color = gameHeader.GetComponent<Button>().colors;
            color.normalColor                      = Color.red;
            gameHeader.GetComponent<Button>().colors = color;
        }
        else
        {
            ResetHeaderColour(gameHeader);
        }
        
        if (currentMenu == audioMenu)
        {
            // Change the colour of the header to match the selected menu.
            var color = audioHeader.GetComponent<Button>().colors;
            color.normalColor                       = Color.red;
            audioHeader.GetComponent<Button>().colors = color;
        }
        else
        {
            ResetHeaderColour(audioHeader);
        }
        
        if (currentMenu == videoMenu)
        {
            // Change the colour of the header to match the selected menu.
            var color = videoHeader.GetComponent<Button>().colors;
            color.normalColor                       = Color.red;
            videoHeader.GetComponent<Button>().colors = color;
        }
        else
        {
            ResetHeaderColour(videoHeader);
        }
    }

     void ResetHeaderColour(Button header)
    {
        // Change the colour of the header to match the selected menu.
        var color = header.colors;
        color.normalColor = Color.white;
        header.colors = color;
    }

    public void ToggleSettings()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        settingsHeader.SetActive(!settingsHeader.activeSelf);
        
        ToggleGameMenu();
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
            switch (InputDeviceManager.PlayerOneDevice)
            {
                case Gamepad:
                    LBKeyIcon.gameObject.SetActive(true);
                    RBKeyIcon.gameObject.SetActive(true);
                    gamepadBottomLeftIcons.SetActive(true);

                    QKeyIcon.gameObject.SetActive(false);
                    EKeyIcon.gameObject.SetActive(false);
                    keyboardBottomLeftIcons.SetActive(false);
                    break;

                case Keyboard:
                    QKeyIcon.gameObject.SetActive(true);
                    EKeyIcon.gameObject.SetActive(true);
                    keyboardBottomLeftIcons.SetActive(true);

                    LBKeyIcon.gameObject.SetActive(false);
                    RBKeyIcon.gameObject.SetActive(false);
                    gamepadBottomLeftIcons.SetActive(false);
                    break;
            }
            
            settingsBackground.gameObject.SetActive(true);
            settingsTitle.gameObject.SetActive(true);
            settingsInfo.gameObject.SetActive(true);
            
            settingsHeaders.ForEach(header => header.gameObject.SetActive(true));
            settingsContents.ForEach(content => content.SetActive(false));

            gameMenu.SetActive(true);
            gameHeader.gameObject.SetActive(true);
            gameContent.SetActive(true);
            
            // Select the first thing in the gameContent
            player1RumbleSlider.Select();
        }
        
        if (menu == audioMenu)
        {
            settingsContents.ForEach(content => content.SetActive(false));
            settingsMenus.ForEach(m => m.SetActive(false));
            audioMenu.SetActive(true);
            audioContent.SetActive(true);
            
            // Select the first thing in the audioContent
            masterVolumeSlider.Select();
        }
        
        if (menu == videoMenu)
        {
            settingsContents.ForEach(content => content.SetActive(false));
            settingsMenus.ForEach(m => m.SetActive(false));
            videoMenu.SetActive(true);
            videoContent.SetActive(true);
            
            // Select the first thing in the videoContent
            UIManager.SelectSelectableByReference(resolutionDropdown);
        }
        
        if (menu == gamepadMenu)
        {
            settingsContents.ForEach(content => content.SetActive(false));
            settingsMenus.ForEach(m => m.SetActive(false));
            gamepadMenu.SetActive(true);
            //gamepadContent.SetActive(true);
            
            // Select the first thing in the gamepadContent
            UIManager.SelectSelectableByReference(null);
        }
        
        if (menu == keyboardMenu)
        {
            settingsContents.ForEach(content => content.SetActive(false));
            settingsMenus.ForEach(m => m.SetActive(false));
            keyboardMenu.SetActive(true);
            //keyboardContent.SetActive(true);
            
            // Select the first thing in the keyboardContent
            UIManager.SelectSelectableByReference(null);
        }
    }

    public void ShowCredits()
    {
        creditsManager.ResetCredits();

        creditsButton.interactable = false;

        creditsMenu.SetActive(true);
        creditsContent.SetActive(true);

        creditsContent.GetComponent<CanvasGroup>().alpha = 0;

        Sequence sequence = DOTween.Sequence();

        // Fade in the background and scale Y from 0 to 1.
        sequence.Append(creditsBackground.DOFade(backgroundOpacity, backgroundTweenDuration));
        sequence.Join(creditsBackground.transform.DOScaleY(1, backgroundTweenDuration));

        // Fade in the canvas group
        sequence.Append(creditsContent.GetComponent<CanvasGroup>().DOFade(1, backgroundTweenDuration));

        sequence.OnComplete
        (() =>
        {
            // do something
        });
    }

    void CloseCredits()
    {
        Sequence sequence = DOTween.Sequence();
        
        // Fade out the canvas group
        sequence.Append(creditsContent.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
        
        // Fade out the background and scale Y from 1 to 0.
        sequence.Append(creditsBackground.DOFade(0, backgroundTweenDuration));
        sequence.Join(creditsBackground.transform.DOScaleY(0, backgroundTweenDuration));

        sequence.OnComplete
        (() =>
        {
            // Toggle the credits menu
            creditsMenu.SetActive(false);

            creditsButton.interactable = true;
            creditsButton.Select();

            closeMenu.Play();
        });
    }

    public bool IsAnyMainMenuActive() => mainMenus.Any(menu => menu.activeSelf);
    public bool IsAnySettingsMenuActive() => settingsMenus.Any(menu => menu.activeSelf);
    public bool IsAnyMenuActive() => IsAnyMainMenuActive() || IsAnySettingsMenuActive();

    public void SetResolution(int resolutionIndex)
    {
        // return if the resolution index equals the current resolution value
        
        string[] resolution = resolutionDropdown.options.ElementAt(resolutionIndex).text.Split('x');
        Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), Screen.fullScreenMode, new RefreshRate { numerator = 60, denominator = 1 });
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);

        // Set the resolution to 1920x1080 resolution if fullscreen is toggled.
        if (isFullscreen)
        {
            Screen.SetResolution
            (1920, 1080, FullScreenMode.FullScreenWindow, new RefreshRate
             { numerator = 60, denominator = 1 });

            resolutionDropdown.value = resolutions.IndexOf("1920x1080");
        }
        else
        {
            SetResolution(resolutionDropdown.value);
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        Screen.fullScreen = isFullscreen;
    }

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isVSync ? 1 : 0);
    }

    public void SetShadowQuality(int shadowQuality) => QualitySettings.shadows = (ShadowQuality) shadowQuality;
    public void SetAntiAliasing(int antiAliasing) => QualitySettings.antiAliasing = antiAliasing;
    public void SetTextureQuality(int textureQuality) => QualitySettings.globalTextureMipmapLimit = textureQuality;
    public void SetAnisotropicFiltering(int anisotropicFiltering) => QualitySettings.anisotropicFiltering = (AnisotropicFiltering) anisotropicFiltering;

    public static void LoadSettings()
    {
        // Load the volume
        SettingsManager.LoadVolume();
        
        // Load the video settings
        int  qualityIndex = PlayerPrefs.GetInt("Quality", 0);         // Default to 0 if not set
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1; // Default to true if not set
        bool isVSync      = PlayerPrefs.GetInt("VSync", 0)      == 1; // Default to false if not set
        
        // Apply the settings
        QualitySettings.SetQualityLevel(qualityIndex);
        Screen.fullScreen          = isFullscreen;
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
    }
    
    public void CloseCurrentMainMenu()
    {
        // Find the active settings menu.
        GameObject currentMenu = mainMenus.Find(menu => menu.activeSelf && menu != mainMenu);
        
        if (currentMenu == null) return;
        
        // The credits menu has a unique close method.
        if (currentMenu == creditsMenu)
        {
            CloseCredits();
            return;
        }

        closeMenu.Play();
        
        // Close the active settings menu.
        currentMenu.SetActive(false);
    }

    public void CloseCurrentSettingsMenu()
    {
        // Find the active settings menu.
        GameObject currentMenu = settingsMenus.Find(menu => menu.activeSelf);
        if (currentMenu == null) return;

        closeMenu.Play();
        
        // Disable all settings menus
        settingsMenus.ForEach(menu => menu.SetActive(false));
        
        // Disable the header and title
        settingsHeader.SetActive(false);
        settingsTitle.gameObject.SetActive(false);
        settingsInfo.gameObject.SetActive(false);
        
        // Disable all on-screen keys
        ToggleOnScreenKeysOff();
        
        // Enable the main menu
        mainMenu.SetActive(true);

        // Disable the background.
        settingsBackground.gameObject.SetActive(false);
        
        // Select the "Settings" button.
        mainMenuButtons[1].Select();
    }

    void ToggleOnScreenKeysOff()
    {
        QKeyIcon.gameObject.SetActive(false);
        EKeyIcon.gameObject.SetActive(false);
        keyboardBottomLeftIcons.SetActive(false);
        LBKeyIcon.gameObject.SetActive(false);
        RBKeyIcon.gameObject.SetActive(false);
        gamepadBottomLeftIcons.SetActive(false);
    }
}
