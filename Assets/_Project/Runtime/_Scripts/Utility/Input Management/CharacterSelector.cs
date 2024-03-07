using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// Selects the character to be played in the game scene.
/// </summary>
public class CharacterSelector : MonoBehaviour
{
    [SerializeField] Button confirmationButton;
    [SerializeField, ReadOnly] bool isCharacterSelected;
    [SerializeField, ReadOnly] bool allCharactersSelected;
    [SerializeField, ReadOnly] bool confirmSelection;


    public static string GetSelectedCharacter(int playerIndex)
    {
        switch (playerIndex) //TODO: On the right track, but this is not the right way to do it.
        {
            case 0:
                return "Shellby";
                break;
            
            case 1:
                return "Dorathy";
                break;
        }
        
        return null;
    }
    
    public void SelectCharacter()
    {
        // Button is clicked.
        isCharacterSelected = !isCharacterSelected;
        
        var selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        
        // Toggle the checkmark.
        var checkmark = selectedButton.gameObject.transform.GetComponentInChildren<RawImage>();
        checkmark.enabled = isCharacterSelected;
        
        // Prevent the player from selecting multiple characters
        ToggleNavigationMode(selectedButton);

        selectedButton.navigation = isCharacterSelected
            ? new ()
            { mode = Navigation.Mode.None }
            : new ()
            { mode = Navigation.Mode.Automatic };
        
        var characterSelectors = FindObjectsOfType<CharacterSelector>();
        foreach (var selector in characterSelectors)
        {
            if (!selector.isCharacterSelected)
            {
                allCharactersSelected = false;
                break;
            }
            
            allCharactersSelected = true;
        }
        
        // Toggle the confirmation button. If all characters are selected, enable the confirmation button, else disable it.
        confirmationButton.gameObject.SetActive(allCharactersSelected);

        if (allCharactersSelected)
        {
            foreach (var eventSystem in FindObjectsOfType<MultiplayerEventSystem>())
            {
                // set the first selected gameobject to the first character selector.
                eventSystem.SetSelectedGameObject(confirmationButton.gameObject);
            }
        }
    }

    public void ConfirmSelection()
    {
        confirmSelection = !confirmSelection;

        // Confirm selection.
        if (allCharactersSelected && confirmSelection)
        {
            confirmationButton.interactable = false;
        }

    }

    #region Utility
    void ToggleNavigationMode(Button selectedButton) => selectedButton.navigation = isCharacterSelected
        ? new ()
        { mode = Navigation.Mode.None }
        : new ()
        { mode = Navigation.Mode.Automatic };

    static string TrimString(string input)
    {
        // Split the input string by space and take the first part
        return input.Split(' ')[0];
    }
    #endregion
    
}
