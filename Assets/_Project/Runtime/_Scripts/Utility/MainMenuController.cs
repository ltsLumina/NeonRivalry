using UnityEngine;
using Sequence = Lumina.Essentials.Sequencer.Sequence;

public class MainMenuController : MonoBehaviour
{
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
}
