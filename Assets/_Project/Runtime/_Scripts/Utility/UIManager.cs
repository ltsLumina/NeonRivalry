using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UIManager : MonoBehaviour
{
    [Tab("Pause Menu")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI pauseMenuTitle;
    [SerializeField] List<Button> pauseMenuButtons = new ();
    
    [Tab("Main Menu")]
    [SerializeField] List<Button> mainMenuButtons = new ();

    public GameObject PauseMenu => pauseMenu;
    public TextMeshProUGUI PauseMenuTitle
    {
        get => pauseMenuTitle;
        set => pauseMenuTitle = value;
    }
    public List<Button> PauseMenuButtons => pauseMenuButtons;

    /// <summary>
    /// <para> 0: Play </para>
    /// <para> 1: Settings </para>
    /// <para> 2: Quit </para>
    /// </summary>
    public List<Button> MainMenuButtons => mainMenuButtons;

    EventSystemSelector eventSystemSelector;

    void Awake() => eventSystemSelector = null;

    void Start()
    {
        PauseMenu.SetActive(false);
    }

    void OnEnable() => InputDeviceManager.OnPlayerJoin += () => eventSystemSelector = FindObjectOfType<EventSystemSelector>();
    
    public void SelectButtonByName(string buttonName)
    {
        GameObject button    = GameObject.Find(buttonName);
        Button     component = button.GetComponent<Button>();
        component.Select();
    }

    public void SelectButtonByReference(Button button) => button.Select();
}
