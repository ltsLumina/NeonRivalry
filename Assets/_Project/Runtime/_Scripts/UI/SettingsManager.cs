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
    [SerializeField] List<Button> settingsButtons; 

    // -- Cached References --
    
    UIManager UIManager;
    Image background; // note: hard-coded value of 0.5f (half opacity) in ShowSettings().

    void Awake()
    {
        UIManager = FindObjectOfType<UIManager>(); 
        background = settingsButtons[0].transform.parent.GetComponentInChildren<Image>();
    }

    void Start()
    {
        foreach (Button button in settingsButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!Keyboard.current.EscapeKeyPressed() && !Gamepad.current.SelectButtonPressed()) return;
        if (settingsButtons.Any(button => button.gameObject.activeSelf)) CloseButton();
    }

    public void ShowSettings()
    {
        // If the background is invisible, fade it in.
        if (background.color.a == 0)
        {
            // Toggle the main menu buttons.
            ToggleMainMenuButtonsShown();
            
            // Disable interaction with the "Close" button.
            ToggleButtonClickable(settingsButtons[2], false);
            
            // Fade in the background and buttons.
            background.DOFade(backgroundOpacity, tweenDuration).OnComplete
            (() =>
            {
                foreach (Button button in settingsButtons)
                {
                    // Activate button and scale it up to 1.
                    button.gameObject.SetActive(true);
                    button.transform.DOScale(1, tweenDuration).SetEase(Ease.OutBack).OnComplete
                    (() =>
                    {
                        // Buttons are active. Activate interaction with the "Close" button and select it.
                        ToggleButtonClickable(settingsButtons[2], true);
                        settingsButtons[2].Select();
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

                // Disable interaction with the "Close" button.
                ToggleButtonClickable(settingsButtons[2], false);
            }

            background.DOFade(0, tweenDuration).OnComplete
            (() =>
            {
                // Toggle the main menu buttons.
                ToggleMainMenuButtonsShown();
                
                ToggleButtonClickable(settingsButtons[2], true);
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

    void ToggleButtonClickable(Button button) => button.interactable = !button.interactable;

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
        
    }

    public void ControlsButton()
    {
        
    }

    public void CloseButton()
    {
        ShowSettings();
        
        // Select the "Settings" button.
        UIManager.MainMenuButtons[1].Select();
    }
}
