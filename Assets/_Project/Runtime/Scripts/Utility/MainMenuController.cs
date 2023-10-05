using Lumina.Essentials.Sequencer;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadNextScene()
    {
        var sequence = new Sequence(this);
        sequence.WaitThenExecute(0.5f, SceneManagerExtended.LoadNextScene);
    }
    
    public void QuitGame()
    {
        var sequence = new Sequence(this);

        sequence.WaitThenExecute
        (0.35f, () =>
        {
            Application.Quit();
            Debug.LogWarning("Usually this would quit the game, but you're in the editor.");
        });
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManagerExtended.LoadScene(sceneIndex);
    }
}
