using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManagerExtended.LoadNextScene();
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Usually this would quit the game, but you're in the editor.");
    }
}
