using DG.Tweening;
using UnityEngine;
using Sequence = Lumina.Essentials.Sequencer.Sequence;

public class MainMenuController : MonoBehaviour
{
    #region Scene Management Methods
    public void LoadNextScene()
    {
        var sequence = new Sequence(this);
        sequence.WaitThenExecute(0.25f, SceneManagerExtended.LoadNextScene);
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
    #endregion
}
