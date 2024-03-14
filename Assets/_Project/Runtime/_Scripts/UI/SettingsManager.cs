using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] List<Button> settingsButtons; 
    [SerializeField] GameObject settingsOptions;

    UIManager UIManager;
    
    void Start() => UIManager = FindObjectOfType<UIManager>();

    public void ToggleSettingsOptions()
    {
        Routine();
        return;

        void Routine()
        {
            var   background = settingsOptions.GetComponentInChildren<Image>();
            float duration   = 0.5f;

            if (background.color.a == 0)
            {
                ToggleMainMenuButtonsClickable();
                
                // Disable interaction with the "Close" button.
                ToggleButtonClickable(settingsButtons[2], false);
                
                // Fade in the background and buttons.
                background.DOFade(0.5f, duration).OnComplete
                (() =>
                {
                    foreach (var button in settingsButtons)
                    {
                        // Activate button and scale it up to 1.
                        button.gameObject.SetActive(true);
                        button.transform.DOScale(1, duration).SetEase(Ease.OutBack).OnComplete(() =>
                        {
                            // Buttons are active. Activate interaction with the "Close" button and select it.
                            ToggleButtonClickable(settingsButtons[2], true);
                            settingsButtons[2].Select();
                        });
                    }
                });
            }
            else
            {
                // Fade out the background and buttons.
                foreach (var button in settingsButtons)
                {
                    // Scale down button to 0 and deactivate it.
                    button.transform.DOScale(0, duration).SetEase(Ease.InBack).OnComplete(() => button.gameObject.SetActive(false));

                    // Disable interaction with the "Close" button.
                    ToggleButtonClickable(settingsButtons[2], false);
                }

                background.DOFade(0, duration).OnComplete
                (() =>
                {
                    ToggleMainMenuButtonsClickable();
                    ToggleButtonClickable(settingsButtons[2], true);
                    UIManager.MainMenuButtons[1].Select();
                });
            }
        }
    }

    public void ToggleButtonClickable(Button button) => button.interactable = !button.interactable;
    public void ToggleButtonClickable(Button button, bool clickable) => button.interactable = clickable; 

    public void ToggleMainMenuButtonsClickable()
    {
        foreach (var button in UIManager.MainMenuButtons)
        {
            button.interactable = !button.interactable;
        }
    }

    public void SelectButtonByName(string buttonName) => UIManager.SelectButtonByButtonName(buttonName);

    public void SelectButtonByReference(Button button) => UIManager.SelectButtonByReference(button);

    public void AudioButton()
    {
        
    }

    public void ControlsButton()
    {
        
    }

    public void CloseButton()
    {
        ToggleSettingsOptions();
        
        // Select the "Settings" button.
        UIManager.MainMenuButtons[1].Select();
    }
}
