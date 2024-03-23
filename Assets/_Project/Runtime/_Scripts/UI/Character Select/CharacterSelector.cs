using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class CharacterSelector
{
    /// <summary>
    /// Key is the player index, value is the <see cref="Character"/>
    /// </summary>
    public static IReadOnlyDictionary<int, Character> SelectedCharacters => new ReadOnlyDictionary<int, Character>(selectedCharacters);

    readonly static Dictionary<int, Character> selectedCharacters = new ();

    static Button previousNavigation;
    static Navigation previousNavigationMode;

    public static void Reset() => selectedCharacters.Clear();

    public static void SelectCharacter(Button button)
    {
        var characterButton = button.GetComponent<CharacterButton>();
        
        // Get the player index from the button.
        int playerIndex = characterButton.PlayerIndex;
        
        // Get the character from the button.
        Character character = characterButton.GetCharacter();

        // Add the character to the dictionary.
        selectedCharacters[playerIndex] = character;

        LogSelectedCharacters(playerIndex, "selected", character);

        // -- Disable navigation -- \\

        // Store the previous navigation.
        previousNavigationMode.selectOnLeft = button.navigation.selectOnLeft;
        previousNavigationMode.selectOnRight = button.navigation.selectOnRight;
        
        // Disable the navigation.
        button.navigation  = new() { mode = Navigation.Mode.None };
    }
    
    public static void DeselectCharacter(int playerIndex, Button button)
    {
        Character previousCharacter = selectedCharacters[playerIndex];
        
        // Remove the character from the dictionary.
        selectedCharacters.Remove(playerIndex);

        LogSelectedCharacters(playerIndex, "deselected", previousCharacter);
        
        // -- Enable navigation -- \\
        
        button.navigation = new() { mode = Navigation.Mode.Explicit };
        
        // Restore the previous navigation.
        Navigation navigation = button.navigation;
        navigation.selectOnLeft = previousNavigationMode.selectOnLeft;
        button.navigation       = navigation;
        
        Navigation buttonNavigation = button.navigation;
        buttonNavigation.selectOnRight = previousNavigationMode.selectOnRight;
        button.navigation              = buttonNavigation;
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