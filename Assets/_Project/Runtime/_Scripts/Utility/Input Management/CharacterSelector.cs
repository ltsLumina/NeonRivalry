using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Selects the character to be played in the game scene.
/// </summary>
public class CharacterSelector : MonoBehaviour
{
    PlayerController player;
    string characterTag;
    bool isCharacterSelected;
    
    public void SetCharacter()
    {
        // Button is clicked.
        isCharacterSelected = !isCharacterSelected;
        
        var selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        var buttonName = selectedButton.gameObject.ToString();
        
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

        // Removes (Player X) from e.g "Shellby (Player X)"
        characterTag = TrimString(buttonName);
        
        //TODO: Can only confirm selection if both players have selected a character. (Smash Bros. Style)
        // Confirm selection.
        // if (isCharacterSelected)
        // {
        //     Debug.Log("Character selected: " + characterTag);
        //     EnableCharacterModel();
        // }
        
    }

    // TODO: this method will send a callback to the following scene or something which spawns in the correct player model.
    void EnableCharacterModel()
    {
        var obj = GameObject.FindWithTag("Character");
        Debug.Log("Found: " + obj);

        switch (characterTag)
        {
            case "Shellby":
                obj.transform.GetChild(0).gameObject.SetActive(true);
                break;
            
            case "Dorathy":
                obj.transform.GetChild(1).gameObject.SetActive(true);
                break;
            
            default:
                Debug.LogError("something went wrong.");
                return;
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
