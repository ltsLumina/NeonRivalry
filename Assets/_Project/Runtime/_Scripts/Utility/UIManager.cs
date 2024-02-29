using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public GameObject PauseMenu
    {
        get => pauseMenu;
        set => pauseMenu = value;
    }

    const int Intro = 0;
    const int MainMenu = 1;
    const int CharacterSelect = 2;
    const int Game = 3;

    public void FindButtonByButtonName(string buttonName)
    {
        FindObjectOfType<EventSystemSelector>().FindButtonByButtonName(buttonName);
    }
    
    
}
