using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using VInspector;
using Debug = UnityEngine.Debug;

/// <summary>
/// This class is a singleton that keeps track of which button prompts should be shown when and where.
/// The prompts are switched out dynamically depending on which input devices are currently connected.
/// <para></para>
/// For instance, if a gamepad is connected, show gamepad prompts.
/// If both a keyboard and gamepad is connected, the prompts will correspond to the input device of player 1,
/// besides in the 'Character Select' scene, where both prompts are shown per-player.  
/// <remarks> This only changes the prompts for the UI. </remarks>
/// </summary>
public class ButtonPromptsManager : MonoBehaviour
{
    [Tab("Gamepad Prompts")]
    [Foldout("Intro")]
    [SerializeField] ButtonPrompt GP_mainMenuPrompt;
    [SerializeField] ButtonPrompt GP_quitGamePrompt;
    
    [Foldout("Main Menu")]
    [SerializeField] ButtonPrompt GP_confirmPrompt;
    [SerializeField] ButtonPrompt GP_toggleMenuLeftPrompt;
    [SerializeField] ButtonPrompt GP_toggleMenuRightPrompt;
    [SerializeField] ButtonPrompt GP_resetDefaultsPrompt;
    [SerializeField] ButtonPrompt GP_movePrompt;
    [SerializeField] ButtonPrompt GP_cancelPrompt;
        
    [Tab("Keyboard Prompts")]
    [Foldout("Intro")]
    [SerializeField] ButtonPrompt KB_mainMenuPrompt;
    [SerializeField] ButtonPrompt KB_quitGamePrompt;
    
    [Foldout("Main Menu")]
    [SerializeField] ButtonPrompt KB_confirmPrompt;
    [SerializeField] ButtonPrompt KB_toggleMenuLeftPrompt;
    [SerializeField] ButtonPrompt KB_toggleMenuRightPrompt;
    [SerializeField] ButtonPrompt KB_resetDefaultsPrompt;
    [SerializeField] GameObject KB_movePrompt;
    [SerializeField] ButtonPrompt KB_cancelPrompt;
    
    public List<ButtonPrompt> currentPrompts = new();

    public List<ButtonPrompt> CurrentPrompts
    {
        get => currentPrompts;
        set => currentPrompts = value;
    }

    #region VInspector Fix
#pragma warning disable CS0414 // Field is assigned but its value is never used
    bool disabled = true;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    #endregion
    
    #region VInspector Data
    // Makes sure the inspector data is saved when recompiling. (Foldouts keep their open/close state)
    // ReSharper disable once UnusedMember.Global
    public VInspectorData VInspectorData;
    #endregion
    
    string intro;
    string mainMenu;
    string characterSelect;
    string game;

    void Awake()
    {
        intro           = SceneManager.GetSceneByBuildIndex(0).name;
        mainMenu        = SceneManager.GetSceneByBuildIndex(1).name;
        characterSelect = SceneManager.GetSceneByBuildIndex(2).name;
        game            = SceneManager.GetSceneByBuildIndex(3).name;
        
         // Disable all prompts by default.
         HideAllPrompts();
    }

    void HideAllPrompts()
    {
        // Get all fields of type ButtonPrompt
        var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.FieldType == typeof(ButtonPrompt));

        // Iterate over each field and toggle it off if it's not null
        foreach (var field in fields)
        {
            var prompt = field.GetValue(this) as ButtonPrompt;
            if (prompt != null) prompt.Toggle(false);
        }
        
        // Fun fact: The time in milliseconds it takes to hide all prompts is about 0.65 ms or 6500 ticks using reflection,
        // while it only takes about 0.1 ms or 1000 ticks without reflection.

        // Due to special circumstances with the Keyboard move prompt, we need to disable it differently.
        if (KB_movePrompt != null) KB_movePrompt.SetActive(false);
    }

    void HidePrompts(InputDevice deviceType)
    {
        if (deviceType == null)
        {
            Debug.LogWarning("The device type is null. Please assign a valid device type.");
            return;
        }

        // Get all fields of type ButtonPrompt
        var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.FieldType == typeof(ButtonPrompt));

        // Determine the prefix based on the device type
        string prefix = deviceType is Keyboard ? "KB_" : "GP_";

        // Iterate over each field and toggle it off if it's not null and has the correct prefix
        foreach (var field in fields)
        {
            if (field.Name.StartsWith(prefix))
            {
                var prompt = field.GetValue(this) as ButtonPrompt;
                if (prompt != null) prompt.Toggle(false);
            }
        }

        // Due to special circumstances with the Keyboard move prompt, we need to disable it differently.
        if (deviceType is Keyboard && KB_movePrompt != null) KB_movePrompt.SetActive(false);
    }

    public void ShowGamepadPrompts(string scene, bool show) => ShowByScene(Gamepad.current, scene, show);
    public void ShowKeyboardPrompts(string scene, bool show) => ShowByScene(Keyboard.current, scene, show);
    
    public void HideGamepadPrompts() => HidePrompts(Gamepad.current);
    public void HideKeyboardPrompts() => HidePrompts(Keyboard.current);

    void ShowByScene(InputDevice deviceType, string scene, bool show)
    {
        deviceType ??= Keyboard.current;

        switch (deviceType)
        {
            case Keyboard:
                switch (scene)
                {
                    case var _ when scene == intro:
                        KB_mainMenuPrompt.Toggle(show);
                        KB_quitGamePrompt.Toggle(show);
                        break;

                    case var _ when scene == mainMenu:
                        if (MenuManager.IsAnySettingsMenuActive())
                        {
                            // Show the settings prompts
                            //KB_confirmPrompt.Toggle(show);
                            KB_toggleMenuLeftPrompt.Toggle(show);
                            KB_toggleMenuRightPrompt.Toggle(show);
                            KB_resetDefaultsPrompt.Toggle(show);
                            KB_movePrompt.SetActive(show);
                            KB_cancelPrompt.Toggle(show);
                        }

                        break;

                    case var _ when scene == characterSelect:
                        // not implemented yet
                        break;

                    case var _ when scene == game:
                        // not implemented yet
                        break;
                }

                break;

            case Gamepad:
                switch (scene)
                {
                    case var _ when scene == intro:
                        GP_mainMenuPrompt.Toggle(show);
                        GP_quitGamePrompt.Toggle(show);
                        break;

                    case var _ when scene == mainMenu:
                        if (MenuManager.IsAnySettingsMenuActive())
                        {
                            // Show the settings prompts
                            //GP_confirmPrompt.Toggle(show);
                            GP_toggleMenuLeftPrompt.Toggle(show);
                            GP_toggleMenuRightPrompt.Toggle(show);
                            GP_resetDefaultsPrompt.Toggle(show);
                            GP_movePrompt.Toggle(show);
                            GP_cancelPrompt.Toggle(show);
                        }

                        break;

                    case var _ when scene == characterSelect:
                        // not implemented yet
                        break;

                    case var _ when scene == game:
                        // not implemented yet
                        break;
                }

                break;
        }

        UpdateCurrentPrompts();
        
        return;
        void UpdateCurrentPrompts()
        {
            // Clear the currentPrompts list
            currentPrompts.Clear();

            // Get all fields of type ButtonPrompt
            var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.FieldType == typeof(ButtonPrompt));

            // Iterate over each field and add it to currentPrompts if it's active
            foreach (var field in fields)
            {
                var prompt = field.GetValue(this) as ButtonPrompt;
                if (prompt != null && prompt.gameObject.activeSelf) { currentPrompts.Add(prompt); }
            }
        }
    }
}
