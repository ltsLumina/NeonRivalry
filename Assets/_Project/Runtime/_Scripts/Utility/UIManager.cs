using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UIManager : MonoBehaviour
{
    [Tab("Main Menu")]
    [SerializeField] List<Button> mainMenuButtons = new ();
    
    [Tab("Pause Menu")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI pauseMenuTitle;
    [SerializeField] List<Button> pauseMenuButtons = new ();

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

    void Start()
    {
        if (PauseMenu != null) PauseMenu.SetActive(false);
    }
    
    public void SelectButtonByName(string buttonName)
    {
        GameObject button    = GameObject.Find(buttonName);
        Button     component = button.GetComponent<Button>();
        component.Select();
    }

    public static void SelectSelectableByReference(Selectable selectable) => selectable.Select();
}
