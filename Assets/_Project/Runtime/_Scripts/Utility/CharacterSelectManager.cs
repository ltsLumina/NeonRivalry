#region
using System;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static SceneManagerExtended;
#endregion

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] Slider p1RumbleSlider;
    [SerializeField] Slider p2RumbleSlider;
    
    static Sound menuClose;
    static bool isTweening;
    
    void Awake()
    {
        // Classic EnterPlaymodeOptions fix
        isTweening = false;

        if (ActiveScene != CharacterSelect)
        {
            Destroy(gameObject);
            Debug.LogException(new InvalidOperationException("CharacterSelectManager should only exist in the Character Select scene."));
        }
    }

    void Start()
    {
        menuClose = new (SFX.MenuClose);
        menuClose.SetOutput(Output.SFX);

        // Load rumble settings
        SettingsManager.LoadRumble(p1RumbleSlider, p2RumbleSlider);
        
        // update slider text
        p1RumbleSlider.onValueChanged.AddListener(_ => UpdateSliderText(p1RumbleSlider));
        p2RumbleSlider.onValueChanged.AddListener(_ => UpdateSliderText(p2RumbleSlider));
    }

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

    /// <summary>
    /// Shows/Hides the character settings menu for the player.
    /// </summary>
    /// <param name="playerID"></param>
    /// <remarks>This method is highly volatile and should be modified with caution.</remarks>
    public static void ToggleCharacterSettingsMenu(int playerID)
    {
        if (ActiveScene != CharacterSelect) return;
        
        GameObject             playerMenu  = GetPlayerMenu(playerID);
        PlayerInput            playerInput = PlayerManager.GetMenuNavigator(playerID).GetComponent<PlayerInput>();
        MultiplayerEventSystem eventSystem = PlayerManager.GetMenuNavigator(playerID).GetComponent<MultiplayerEventSystem>();

        // Check if the menu is currently active
        bool isMenuActive = playerMenu.activeSelf;

        if (isMenuActive) CloseCharacterSettingsMenu(playerID, eventSystem);
        else OpenCharacterSettingsMenu(playerID, playerInput, eventSystem);
    }

    static void OpenCharacterSettingsMenu(int playerID, PlayerInput playerInput, MultiplayerEventSystem eventSystem)
    {
        if (isTweening) return;

        GameObject      playerMenu    = GetPlayerMenu(playerID);
        DiagramSelector playerDiagram = GetPlayerDiagram(playerID);

        const float shownY  = 50f;
        const float hiddenY = 1200f;

        // Create a sequence for the tweening animation
        Sequence sequence = DOTween.Sequence();
        isTweening = true;

        // Reset the position of the player menu
        playerMenu.transform.localPosition = new (playerMenu.transform.localPosition.x, hiddenY, playerMenu.transform.localPosition.z);
        playerMenu.SetActive(true);

        // Show the device buttons for the player's control scheme and show the diagram
        GameObject deviceButtons = ShowDeviceButtons(playerInput.currentControlScheme, playerMenu);
        deviceButtons.SetActive(true);
        playerDiagram.ShowDiagram();

        // Set the selection to the first button in the device buttons
        eventSystem.SetSelectedGameObject(deviceButtons.transform.GetChild(0)?.GetComponentInChildren<Button>().gameObject);

        // Tween the player menu into view
        sequence.AppendInterval(0.2f);
        sequence.Append(playerMenu.transform.DOLocalMoveY(shownY, 0.5f).SetEase(Ease.OutBack)).OnStart(() => isTweening = true);
        sequence.OnComplete(() =>
        {
            isTweening = false;
        });
    }

    public static void CloseCharacterSettingsMenu(int playerID, MultiplayerEventSystem eventSystem)
    {
        if (isTweening) return;

        GameObject playerMenu = GetPlayerMenu(playerID);
        const float hiddenY = 1200f;

        // Create a sequence for the tweening animation
        Sequence sequence = DOTween.Sequence();
        isTweening = true;

        // Tween the player menu out of view
        sequence.AppendInterval(0.2f);
        sequence.Append(playerMenu.transform.DOLocalMoveY(hiddenY, 0.5f).SetEase(Ease.InBack)).OnStart(() => isTweening = true).OnComplete
        (() =>
        {
            isTweening = false;
                
            // Hide the device buttons and the diagram
            playerMenu.SetActive(false);
            playerMenu.transform.localPosition = new (playerMenu.transform.localPosition.x, hiddenY, playerMenu.transform.localPosition.z);

            // Set the selection back to the player's character button.
            var characterButton = GameObject.Find($"Player {playerID}").transform.GetChild(0).gameObject.GetComponent<Button>();
            eventSystem.SetSelectedGameObject(characterButton.gameObject);
        });
    }

    #region Menu Utility Functions
    static GameObject GetPlayerMenu(int playerID) => FindObjectOfType<RebindSaveLoad>().transform.GetChild(playerID - 1).gameObject;
    static DiagramSelector GetPlayerDiagram(int playerID) => GetPlayerMenu(playerID).GetComponentInChildren<DiagramSelector>(true);
    static GameObject ShowDeviceButtons(string controlScheme, GameObject playerMenu) => controlScheme switch
    { "Gamepad"  => playerMenu.GetComponentInChildren<GamepadIconsExample>(true).gameObject,
      "Keyboard" => playerMenu.GetComponentInChildren<KeyboardIconsExample>(true).gameObject,
      _          => null };
    #endregion
}
