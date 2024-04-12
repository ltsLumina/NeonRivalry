#region
using System.Collections;
using System.Linq;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using VInspector;
using static SceneManagerExtended;
#endregion

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] Slider p1RumbleSlider;
    [SerializeField] Slider p2RumbleSlider;

    [Tab("Player 1")]
    [SerializeField] public GameObject player1_shelbyButton;
    [SerializeField] public GameObject player1_morpheButton;

    [SerializeField] public GameObject player1_shelbyModel;
    [SerializeField] public GameObject player1_morpheModel;

    [SerializeField] public Image player1_shelbyText;
    [SerializeField] public Image player1_morpheText;

    [SerializeField] public Image player1_SettingsIcon;
    
    [Tab("Player 2")]
    [SerializeField] public GameObject player2_shelbyButton;
    [SerializeField] public GameObject player2_morpheButton;
    
    [SerializeField] public GameObject player2_shelbyModel;
    [SerializeField] public GameObject player2_morpheModel;
    
    [SerializeField] public Image player2_shelbyText;
    [SerializeField] public Image player2_morpheText;
    
    [SerializeField] public Image player2_SettingsIcon;

    static Sound menuClose;
    static bool isTweening;
    
    void Awake()
    {
        // Classic EnterPlaymodeOptions fix
        isTweening = false;

        Debug.Assert(ActiveScene == CharacterSelect, "CharacterSelectManager should only exist in the Character Select scene.");
    }

    IEnumerator Start()
    {
        menuClose = new (SFX.MenuClose);
        menuClose.SetOutput(Output.SFX);

        // Load rumble settings
        SettingsManager.LoadRumble(p1RumbleSlider, p2RumbleSlider);
        
        // update slider text
        p1RumbleSlider.onValueChanged.AddListener(_ => UpdateSliderText(p1RumbleSlider));
        p2RumbleSlider.onValueChanged.AddListener(_ => UpdateSliderText(p2RumbleSlider));

        
        var promptManager = FindObjectOfType<ButtonPromptsManager>();
        promptManager.ShowGamepadPrompts("CharacterSelect", true);
        promptManager.ShowKeyboardPrompts("CharacterSelect", true);

        yield return new WaitUntil(() => PlayerManager.MenuNavigators.Count == 2);
        promptManager.CurrentPrompts.First().gameObject.transform.parent.DOLocalMoveY(-600, 0.5f).SetEase(Ease.InBack);
        
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

    public static bool IsMenuActive(int playerID) => GetPlayerMenu(playerID).activeSelf;
    public static bool IsAnyMenuActive() => GetPlayerMenu(1).activeSelf || GetPlayerMenu(2).activeSelf;
    
    /// <summary>
    /// Shows/Hides the character settings menu for the player.
    /// </summary>
    /// <param name="playerID"></param>
    /// <remarks>This method is highly volatile and should be modified with caution.</remarks>
    public void ToggleCharacterSettingsMenu(int playerID)
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

    void OpenCharacterSettingsMenu(int playerID, PlayerInput playerInput, MultiplayerEventSystem eventSystem)
    {
        if (isTweening) return;

        GameObject      playerMenu    = GetPlayerMenu(playerID);
        DiagramSelector playerDiagram = GetPlayerDiagram(playerID);

        const float shownY  = 90f;
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

        // Tween the settings icon out of the way.
        sequence.Append(playerID == 1 ? player1_SettingsIcon.transform.DOLocalMoveY(785, 1f) : player2_SettingsIcon.transform.DOLocalMoveY(785, 1f));

        // Tween the player menu into view
        sequence.Append(playerMenu.transform.DOLocalMoveY(shownY, 0.5f).SetEase(Ease.OutBack)).OnStart(() => isTweening = true);
        sequence.OnComplete(() => { isTweening = false; });
    }

    void CloseCharacterSettingsMenu(int playerID, MultiplayerEventSystem eventSystem)
    {
        if (isTweening) return;

        GameObject playerMenu = GetPlayerMenu(playerID);
        const float hiddenY = 1200f;

        // Create a sequence for the tweening animation
        Sequence sequence = DOTween.Sequence();
        isTweening = true;

        // Tween the player menu out of view
        sequence.Append(playerMenu.transform.DOLocalMoveY(hiddenY, 0.5f).SetEase(Ease.InBack)).OnStart(() => isTweening = true).OnComplete
        (() =>
        {
            isTweening = false;
            
            // Tween the settings icon back into view.
            if (playerID == 1) player1_SettingsIcon.transform.DOLocalMoveY(485, 1f);
            else player2_SettingsIcon.transform.DOLocalMoveY(485, 1f);
                
            // Hide the device buttons and the diagram
            playerMenu.SetActive(false);
            playerMenu.transform.localPosition = new (playerMenu.transform.localPosition.x, hiddenY, playerMenu.transform.localPosition.z);

            // Set the selection back to the player's character button.
            var characterButton = GameObject.Find($"Player {playerID} CB").transform.GetChild(0).gameObject.GetComponent<Button>();
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
