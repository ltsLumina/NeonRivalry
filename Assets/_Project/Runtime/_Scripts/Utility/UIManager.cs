using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] List<Button> mainMenuButtons = new ();

    public GameObject PauseMenu => pauseMenu;
    
    /// <summary>
    /// <para> 0: Play </para>
    /// <para> 1: Settings </para>
    /// <para> 2: Quit </para>
    /// </summary>
    public List<Button> MainMenuButtons => mainMenuButtons;

    EventSystemSelector eventSystemSelector;

    void Awake() => eventSystemSelector = null;

    void OnEnable() => InputDeviceManager.OnPlayerJoin += () => eventSystemSelector = FindObjectOfType<EventSystemSelector>();
    
    public void SelectButtonByName(string buttonName) => eventSystemSelector.SelectButtonByName(buttonName);
    public void SelectButtonByReference(Button button) => eventSystemSelector.SelectButtonByReference(button);
}
