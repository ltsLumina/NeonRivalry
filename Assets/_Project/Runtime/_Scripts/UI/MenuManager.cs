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
    #region Lists
    [Tooltip(tooltip)]
    readonly static List<GameObject> mainMenus = new ();
    [Tooltip(tooltip)]
    readonly static List<Button> mainMenuButtons = new ();
    [Tooltip(tooltip)]
    readonly static List<GameObject> mainContents = new ();

    // -- Settings -- \\
    
    readonly static List<GameObject> settingsMenus = new ();
    [Tooltip(tooltip)]
    readonly static List<Button> settingsHeaders = new (); // The buttons at the top of the settings menu. ("Game", "Audio", "Video", etc.)
    [Tooltip(tooltip)]
    readonly static List<GameObject> settingsContents = new ();
    #endregion
    
    [Tab("Main")]
    [Foldout("Main")]
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
    [EndFoldout]
    
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

    [SerializeField] GameObject settingsHeader;
    
    [Foldout("Game")]
    [SerializeField] GameObject gameMenu;
    [SerializeField] Button gameHeader;
    
    [Header("Content")]
    [SerializeField] GameObject gameContent;
    [SerializeField] Toggle showEffectsToggle;
    [SerializeField] Toggle showParticlesToggle;
    
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

    // === === === ===  Cached References  === === === === \\
    
    CreditsManager creditsManager;
    ButtonPromptsManager promptManager;
    
    // === === === ===  Private Variables  === === === === \\
    
    const string tooltip = "Populated at runtime. This list should not be populated manually.";
    
    List<string> resolutions;
    Sound openMenu;
    Sound closeMenu;

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
    #endregion

    void Awake()
    {
        // References
        creditsManager = FindObjectOfType<CreditsManager>();
        promptManager = FindObjectOfType<ButtonPromptsManager>();

        // Due to EnterPlaymodeOptions, we must clear all lists in Awake.
        mainMenus.Clear();
        mainMenuButtons.Clear();
        mainContents.Clear();
        
        settingsMenus.Clear();
        settingsHeaders.Clear();
        settingsContents.Clear();
    }

    void OnEnable()
    {
        creditsManager.onCreditsEnd.AddListener(CloseCredits);
    }

    void Start()
    {
        // Load PlayerPrefs settings.
        SettingsManager.LoadVolume(masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider);
        
        // Populate the main lists, disable all menus, and enable the main menu.
        mainMenus.AddRange(new List<GameObject> {mainMenu, creditsMenu});
        mainContents.AddRange(new List<GameObject> {mainContent, creditsContent});
        mainMenus.ForEach(menu => menu.SetActive(false));
        mainMenu.SetActive(true);
        creditsManager.gameObject.SetActive(false);
        
        // Populate and enable the main menu buttons.
        mainMenuButtons.AddRange(new List<Button> {playButton, settingsButton, quitButton});
        mainMenuButtons.ForEach(button => button.gameObject.SetActive(true));
        
        // Populate the settings menus, headers, and contents lists and disable it all.
        settingsMenus.AddRange(new List<GameObject> {gameMenu, audioMenu, videoMenu});
        settingsMenus.ForEach(menu => menu.SetActive(false));
        settingsTitle.gameObject.SetActive(false);
        settingsInfo.gameObject.SetActive(false);
        settingsInfo.enabled = false; // Settings info is treated differently, as the text is dynamic.
        settingsHeaders.AddRange(new List<Button> {gameHeader, audioHeader, videoHeader});
        settingsHeader.SetActive(false);
        settingsContents.AddRange(new List<GameObject> {gameContent, audioContent, videoContent});
        settingsContents.ForEach(content => content.SetActive(false));
        
        // Disable the settings background.
        settingsBackground.gameObject.SetActive(false);
        
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

    public void ScaleUpButton(Button button) => button.transform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack);
    public void ScaleDownButton(Button button) => button.transform.DOScale(1, 0.1f).SetEase(Ease.InBack);
    
    public void ScaleUpSliderButton(Selectable parent) => parent.transform.DOScale(1.05f, 0.1f).SetEase(Ease.OutBack);
    public void ScaleDownSliderButton(Selectable parent) => parent.transform.DOScale(1, 0.1f).SetEase(Ease.InBack);

    public void OnClickButton(Button button)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(button.transform.DOScale(4, 0.1f).SetEase(Ease.InBack));
        sequence.Append(button.transform.DOScale(4.2f, 0.1f).SetEase(Ease.OutBack));
    }
    
    void Update()
    {
        ChangeHeaderColour();

        // Toggle between the settings menus. (Game, Audio, Video, etc.)
        SwitchBetweenHeaderMenus();

        return;
        void SwitchBetweenHeaderMenus()
        {
            // Get the player's player input
            if (PlayerInput.all.Count == 0) return;
            var action = PlayerInput.all[0].actions["SwitchSettingsMenu"];
            var input  = action.ReadValue<Vector2>();

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
    }

    void ChangeHeaderColour()
    {
        // Define the new color for the active header
        var newColor = new Color(0.36f, 0.87f, 0.96f);

        // Iterate over all settings menus and headers
        for (int i = 0; i < settingsMenus.Count; i++)
        {
            var currentMenu   = settingsMenus[i];
            var currentHeader = settingsHeaders[i];

            // If the current menu is active, change the header color
            if (currentMenu.activeSelf)
            {
                var headerColor = currentHeader.colors;
                headerColor.normalColor = newColor;
                currentHeader.colors    = headerColor;
            }
            else
            {
                // If the current menu is not active, reset the header color
                ResetHeaderColour(currentHeader);
            }
        }
    }

    void ResetHeaderColour(Button header)
    {
        // Change the colour of the header to match the selected menu.
        ColorBlock color = header.colors;
        color.normalColor = Color.white;
        header.colors     = color;
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
            settingsBackground.gameObject.SetActive(true);
            settingsTitle.gameObject.SetActive(true);
            settingsInfo.gameObject.SetActive(true);
            settingsInfo.enabled = true;
            
            settingsHeaders.ForEach(header => header.gameObject.SetActive(true));
            settingsContents.ForEach(content => content.SetActive(false));

            gameMenu.SetActive(true);
            gameHeader.gameObject.SetActive(true);
            gameContent.SetActive(true);
            
            // Select the first thing in the gameContent
            showEffectsToggle.Select();
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

        switch (InputDeviceManager.PlayerOneDevice)
        {
            case Gamepad:
                promptManager.ShowGamepadPrompts("MainMenu", true);
                break;

            case Keyboard:
                promptManager.ShowKeyboardPrompts("MainMenu", true);
                break;
        }
    }

    public void ShowCredits()
    {
        // Close all settings menus.
        settingsBackground.gameObject.SetActive(false);
        settingsTitle.gameObject.SetActive(false);
        settingsInfo.gameObject.SetActive(false);
        promptManager.HideKeyboardPrompts();
        promptManager.HideGamepadPrompts();
        settingsMenus.ForEach(menu => menu.SetActive(false));
        settingsHeader.SetActive(false);
        
        // Enable the Credits Manager.
        creditsManager.gameObject.SetActive(true);
        
        creditsManager.ResetCredits();

        creditsButton.interactable = false;
        
        creditsMenu.SetActive(true);
        creditsContent.SetActive(true);

        creditsContent.GetComponent<CanvasGroup>().alpha = 0;

        Sequence sequence = DOTween.Sequence();

        // Fade in the background and scale Y from 0 to 1.
        sequence.Append(creditsBackground.DOFade(1, 0.5f));
        sequence.Join(creditsBackground.transform.DOScaleY(1, 0.5f));

        // Fade in the canvas group
        sequence.Append(creditsContent.GetComponent<CanvasGroup>().DOFade(1, 0.5f));

        sequence.OnComplete
        (() =>
        {
            // do something
        });
    }

    void CloseCredits()
    {
        // Enable the main menu
        mainMenu.SetActive(true);
        
        Sequence sequence = DOTween.Sequence();
        
        // Fade out the canvas group
        sequence.Append(creditsContent.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
        
        // Fade out the background and scale Y from 1 to 0.
        sequence.Append(creditsBackground.DOFade(0, 0.5f));
        sequence.Join(creditsBackground.transform.DOScaleY(0, 0.5f));

        sequence.OnComplete
        (() =>
        {
            // Toggle the credits menu
            creditsMenu.SetActive(false);

            creditsButton.interactable = true;
            settingsButton.Select();
        });
    }

    public bool IsAnyMainMenuActive() => mainMenus.Any(menu => menu.activeSelf);

    public static bool IsAnySettingsMenuActive() => settingsMenus.Any(menu => menu.activeSelf);

    public bool IsAnyMenuActive() => IsAnyMainMenuActive() || IsAnySettingsMenuActive();

    void SetEffects(bool showEffects) => SettingsManager.ShowEffects = showEffects;
    void SetParticles(bool showParticles) => SettingsManager.ShowParticles = showParticles;

    public void SetResolution(int resolutionIndex)
    {
        // return if the resolution index equals the current resolution value
        
        string[] resolution = resolutionDropdown.options.ElementAt(resolutionIndex).text.Split('x');
        Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), Screen.fullScreenMode, new RefreshRate { numerator = 60, denominator = 1 });
        
        // Update the resolution dropdown value
        resolutionDropdown.value = resolutionIndex;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
        
        // Update the quality dropdown value
        qualityDropdown.value = qualityIndex;
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
        
        // Update the fullscreen toggle value
        fullscreenToggle.isOn = isFullscreen;
    }

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isVSync ? 1 : 0);
        
        // Update the VSync toggle value
        vSyncToggle.isOn = isVSync;
    }
    
    public void ResetDefaults()
    {
        // Default: Show effects.
        SetEffects(true);
        
        // Default: Show effects.
        SetParticles(true);
        
        // Default: 1920x1080. Index 2 in the dropdown.
        SetResolution(2);
        
        // Default: High. Index 0 in the dropdown.
        SetQuality(0);
        
        // Reset fullscreen to the current fullscreen setting.
        SetFullscreen(true);
        
        // Reset VSync to the current VSync setting.
        SetVSync(true);
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
            closeMenu.Play();
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

        closeMenu.Play();
        
        // Disable all settings menus
        settingsMenus.ForEach(menu => menu.SetActive(false));
        
        // Disable prompts
        promptManager.HideGamepadPrompts();
        promptManager.HideKeyboardPrompts();
        
        // Disable the header and title
        settingsHeader.SetActive(false);
        settingsTitle.gameObject.SetActive(false);
        settingsInfo.gameObject.SetActive(false);
        
        // Enable the main menu
        mainMenu.SetActive(true);

        // Disable the background.
        settingsBackground.gameObject.SetActive(false);
        
        // Select the "Settings" button.
        mainMenuButtons[1].Select();
    }
}
