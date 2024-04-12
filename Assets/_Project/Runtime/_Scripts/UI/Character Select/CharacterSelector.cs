using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class CharacterSelector
{
    /// <summary>
    /// Key is the player index, value is the <see cref="Character"/>
    /// </summary>
    public static IReadOnlyDictionary<int, Character> SelectedCharacters => new ReadOnlyDictionary<int, Character>(selectedCharacters);

    readonly static Dictionary<int, Character> selectedCharacters = new ();

    public static void Reset() => selectedCharacters.Clear();

    public static bool SelectCharacter(Button button)
    {
        if (button == null) return false;
        var characterButton = button.GetComponent<CharacterButton>();

        // Get the player index from the button.
        int playerIndex = characterButton.PlayerIndex;

        // Check if a character is already selected for this player index.
        if (selectedCharacters.ContainsKey(playerIndex)) return false;

        // Get the character from the button.
        Character character = characterButton.GetCharacter();

        // Add the character to the dictionary.
        selectedCharacters[playerIndex] = character;

        LogSelectedCharacters(playerIndex, "selected", character);

        // -- Disable navigation -- \\

        // Disable the navigation.
        button.navigation = new ()
        { mode = Navigation.Mode.None };

        return true;
    }
    
    public static bool DeselectCharacter(int playerIndex, MultiplayerEventSystem eventSystem)
    {
        // return if there is no character to deselect
        if (!selectedCharacters.Remove(playerIndex, out Character previousCharacter)) return false;

        LogSelectedCharacters(playerIndex, "deselected", previousCharacter); 
        
        // -- Enable navigation -- \\
        
        var selectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        ResetNavigation(selectedButton);

        return true;
    }
    
    public static void ResetNavigation(Button button)
    {
        var characterButton = button.GetComponent<CharacterButton>();
        if (characterButton == null) return;
        
        button.navigation = new() { mode = Navigation.Mode.Explicit };
        
        // Restore the previous navigation.
        Navigation navigation = button.navigation;
        navigation.selectOnLeft  = characterButton.LeftButton;
        navigation.selectOnRight = characterButton.RightButton;
        button.navigation        = navigation;
    }
    
    public static void ResetAllNavigation(CharacterButton[] characterButtons)
    {
        foreach (CharacterButton characterButton in characterButtons)
        {
            ResetNavigation(characterButton.GetComponent<Button>());
        }
    }

    /// <summary>
    /// Logs the selected characters along with the action performed on them (selected or deselected).
    /// </summary>
    /// <param name="playerIndex">The index of the player performing the action.</param>
    /// <param name="action">The action performed on the character (selected or deselected).</param>
    /// <param name="character">The character on which the action was performed. This parameter is optional.</param>
    /// <param name="log">A boolean value indicating whether to log the action or not. This parameter is optional and defaults to true.</param>
    static void LogSelectedCharacters(int playerIndex, string action, Character character = null, bool log = true)
    {
        if (!log) return;

        // Create a StringBuilder to hold the selected characters and their player indices.
        var selectedCharactersBuilder = new StringBuilder();
        selectedCharactersBuilder.Append(selectedCharacters.Count > 0 ? "Selected characters: " : "No characters selected.");

        // Iterate over the SelectedCharacters dictionary.
        foreach (KeyValuePair<int, Character> entry in selectedCharacters)
        {
            // Append each player index and character name to the StringBuilder.
            selectedCharactersBuilder.Append($"(Player {entry.Key}) {entry.Value.name}, ");
        }

        // Remove the trailing comma and space.
        if (selectedCharacters.Count > 0) selectedCharactersBuilder.Length -= 2;

        // Log the selected characters and their player indices.
        Debug.Log($"Player {playerIndex} {action} {character?.name}." + $"\n{selectedCharactersBuilder}");
    }
}