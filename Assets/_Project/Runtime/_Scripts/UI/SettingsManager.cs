using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;

public class SettingsManager : MonoBehaviour
{
    [Tab("Tweening"), Range(0,1)]
    [SerializeField] float backgroundOpacity = 0.5f;
    [SerializeField] float tweenDuration = 0.5f;

    [Tab("References")]
    [SerializeField] List<Image> backgrounds;
    [SerializeField] List<Button> settingsButtons;
    [SerializeField] List<Selectable> audioSelectables; // Buttons, sliders, etc.

    // -- Cached References --
    
    UIManager UIManager;

    void Awake()
    {
        UIManager = FindObjectOfType<UIManager>(); 
    }

    void Start()
    {
        foreach (Button button in settingsButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        foreach (Selectable selectable in audioSelectables)
        {
            selectable.gameObject.SetActive(false);
        }
        
        foreach (Image background in backgrounds)
        {
            background.gameObject.SetActive(true);
            background.color = new (background.color.r, background.color.g, background.color.b, 0);
        }
    }

    void Update()
    {
        if (!Keyboard.current.EscapeKeyPressed() && !Gamepad.current.SelectButtonPressed()) return;
        if (settingsButtons.Any(button => button.gameObject.activeSelf)) CloseButton();
    }

    public void ToggleSettingsShown()
    {
        // If audio settings are open, close them.
        if (audioSelectables.Any(selectable => selectable.gameObject.activeSelf))
        {
            var masterVolumeSlider = audioSelectables[0];
            masterVolumeSlider.gameObject.transform.DOScale(0, tweenDuration).SetEase(Ease.InBack).OnComplete(() =>
            {
                masterVolumeSlider.gameObject.SetActive(false);
            });
            
            var closeButton = audioSelectables[1].GetComponent<Button>();
            closeButton.gameObject.transform.DOScale(0, tweenDuration).SetEase(Ease.InBack).OnComplete(() =>
            {
                closeButton.gameObject.SetActive(false);
            });
            
            backgrounds[1].DOFade(0, tweenDuration).OnComplete(() =>
            {
                foreach (Button button in settingsButtons) { ToggleButtonClickable(button, true); }
                settingsButtons[0].Select();
            });
        }
        
        // If the background is invisible, fade it in.
        if (backgrounds[0].color.a == 0)
        {
            // Toggle the main menu buttons.
            if (backgrounds[1].color.a == 0) // If the audio settings aren't shown.
            {
                ToggleMainMenuButtonsShown();
            }
            
            // Disable interaction with the buttons.
            foreach (Button button in settingsButtons) { ToggleButtonClickable(button, false); }

            // Fade in the background and buttons.
            backgrounds[0].DOFade(backgroundOpacity, tweenDuration).OnComplete
            (() =>
            {
                foreach (Button button in settingsButtons)
                {
                    ToggleButtonClickable(button, false);
                    
                    // Activate button and scale it up to 1.
                    button.gameObject.SetActive(true);
                    button.transform.DOScale(1, tweenDuration).SetEase(Ease.OutBack).OnComplete
                    (() =>
                    {
                        // Buttons are active. Activate interaction with the buttons.
                        ToggleButtonClickable(button, true);
                        
                        // Select the "Audio" button.
                        settingsButtons[0].Select();
                    });
                }
            });
        }

        // If the background is visible, fade it out.
        else
        {
            // Fade out the background and buttons.
            foreach (Button button in settingsButtons)
            {
                // Scale down button to 0 and deactivate it.
                button.transform.DOScale(0, tweenDuration).SetEase(Ease.InBack).OnComplete(() => button.gameObject.SetActive(false));

                // Disable interaction with all buttons.
                ToggleButtonClickable(button, false);
            }

            backgrounds[0].DOFade(0, tweenDuration).OnComplete
            (() =>
            {
                // Toggle the main menu buttons.
                if (backgrounds[1].color.a == 0) // If the audio settings aren't shown.
                {
                    ToggleMainMenuButtonsShown();
                }

                foreach (Button button in settingsButtons)
                {
                    // Enable interaction with all buttons.
                    ToggleButtonClickable(button, true);
                }
                UIManager.MainMenuButtons[1].Select();
            });
        }
    }
    
    void ToggleMainMenuButtonsShown()
    {
        foreach (Button button in UIManager.MainMenuButtons)
        {
            if (button.gameObject.activeSelf)
            {
                ToggleMainMenuButtonsClickable();
                
                // Scale down button to 0 and deactivate it.
                button.transform.DOScale(0, tweenDuration).SetEase(Ease.InBack).OnComplete(() => button.gameObject.SetActive(false));
            }
            else
            {
                // Activate button and scale it up to 1.
                button.gameObject.SetActive(true);
                button.transform.DOScale(1, tweenDuration).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    ToggleMainMenuButtonsClickable();
                    SelectButtonByReference(UIManager.MainMenuButtons[1]);
                });
            }
        }
    }

    void ToggleButtonClickable(Button button, bool clickable) => button.interactable = clickable;

    void ToggleMainMenuButtonsClickable()
    {
        foreach (Button button in UIManager.MainMenuButtons)
        {
            button.interactable = !button.interactable;
        }
    }

    void SelectButtonByReference(Button button) => UIManager.SelectButtonByReference(button);

    public void AudioButton()
    {
        ToggleSettingsShown();

        // Disable interaction with the "Audio" button.
        ToggleButtonClickable(settingsButtons[0], false);
        
        backgrounds[1].DOFade(backgroundOpacity, tweenDuration).OnComplete(() =>
        {
            // Select the "Audio" Slider.
            var masterVolumeSlider = audioSelectables[0];
            masterVolumeSlider.gameObject.SetActive(!masterVolumeSlider.gameObject.activeSelf);
            masterVolumeSlider.gameObject.transform.DOScale(4, tweenDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                masterVolumeSlider.Select();
            });
            
            var closeButton = audioSelectables[1].GetComponent<Button>();
            closeButton.gameObject.SetActive(!closeButton.gameObject.activeSelf);
            closeButton.gameObject.transform.DOScale(1, tweenDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                // Enable interaction with the "Audio" button.
                ToggleButtonClickable(settingsButtons[0], true);
            });
            
        });
        
    }

    public void ControlsButton()
    {
        
    }

    public void CloseButton()
    {
        ToggleSettingsShown();
        
        // Select the "Settings" button.
        UIManager.MainMenuButtons[1].Select();
    }
}
