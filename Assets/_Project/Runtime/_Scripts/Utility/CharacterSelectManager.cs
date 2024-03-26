#region
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static SceneManagerExtended;
#endregion

public class CharacterSelectManager : MonoBehaviour
{
    static Sound closeMenu;
    
    void Start()
    {
        closeMenu = new (SFX.MenuClose);
        closeMenu.SetOutput(Output.SFX);
    }

    /// <summary>
    /// Shows/Hides the character settings menu for the player.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="playerInput"></param>
    /// <param name="eventSystem"></param>
    /// <remarks>This method is highly volatile and should be modified with caution.</remarks>
    public static void ToggleCharacterSettingsMenu(int playerID, PlayerInput playerInput, MultiplayerEventSystem eventSystem)
    {
        if (CharacterSelectScene)
        {
            GameObject      playerMenu    = GetPlayerMenu(playerID);
            DiagramSelector playerDiagram = GetPlayerDiagram(playerID);
            
            playerMenu.SetActive(!playerMenu.activeSelf);
            bool isMenuOpen = playerMenu.activeSelf;

            // Show the appropriate menu.
            GameObject controlSchemeObject = ShowDeviceMenu(playerInput.currentControlScheme, playerMenu);
            controlSchemeObject.SetActive(!controlSchemeObject.activeSelf);

            // Enable the diagram
            playerDiagram.ShowDiagram();

            // Select the first button in the control scheme object.
            eventSystem.SetSelectedGameObject(controlSchemeObject.transform.GetChild(0)?.GetComponentInChildren<Button>().gameObject);

            if (!isMenuOpen)
            {
                closeMenu.Play();

                // Set the selection back to the player's character button.
                var characterButton = GameObject.Find($"Player {playerID}").transform.GetChild(0).gameObject.GetComponent<Button>();
                eventSystem.SetSelectedGameObject(characterButton.gameObject);
            }
        }
    }

    #region Menu Utility Functions
    static GameObject GetPlayerMenu(int playerID) => FindObjectOfType<RebindSaveLoad>().transform.GetChild(playerID - 1).gameObject;
    static DiagramSelector GetPlayerDiagram(int playerID) => GetPlayerMenu(playerID).GetComponentInChildren<DiagramSelector>(true);

    /// <summary>
    /// Shows the appropriate menu based on the control scheme.
    /// </summary>
    /// <param name="controlScheme"></param>
    /// <param name="playerMenu"></param>
    /// <returns></returns>
    static GameObject ShowDeviceMenu(string controlScheme, GameObject playerMenu) => controlScheme switch
    { "Gamepad"  => playerMenu.GetComponentInChildren<GamepadIconsExample>(true).gameObject,
      "Keyboard" => playerMenu.GetComponentInChildren<KeyboardIconsExample>(true).gameObject,
      _          => null };
    #endregion
    
    public void CloseSettingsMenu(int playerID)
    {
        var players = FindObjectsOfType<MenuNavigator>();
        foreach (var player in players)
        {
            if (player.PlayerID == playerID)
            {
                player.OnMenu(default);
            }
        }
    }
}
