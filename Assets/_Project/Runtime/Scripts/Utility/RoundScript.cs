using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoundScript : SingletonPersistent<RoundScript>
{
    [Header("Components")]
    public Image won2RoundPlayer1;
    public Image won1RoundPlayer1;
    public Image won2RoundPlayer2;
    public Image won1RoundPlayer2;

    [Header("Rounds")]
    [SerializeField] int maxRounds;
    [SerializeField] int currentRounds;
    [SerializeField] int player1WonRounds;
    [SerializeField] int player2WonRounds;

    [Header("Bools")]
    public bool p1HasWon;
    public bool p2HasWon;
    public bool timer0;


private void Update()
{
    CheckingBools();
}

private void CheckingBools()
{
    if (currentRounds > maxRounds)
        {
            SceneManagerExtended.LoadNextScene();
            currentRounds = 0;
        }

        if (timer0 
            && PlayerManager.PlayerOne.Healthbar.Value > PlayerManager.PlayerTwo.Healthbar.Value
            && currentRounds <= maxRounds)
        {player1WonRounds++; currentRounds++; SceneManagerExtended.ReloadScene(); }
        else if (timer0
            && PlayerManager.PlayerTwo.Healthbar.Value > PlayerManager.PlayerOne.Healthbar.Value
            && currentRounds <= maxRounds)
        {player2WonRounds++; currentRounds++; SceneManagerExtended.ReloadScene(); }
    }

    private IEnumerator WinningCamera()
    {
        FindObjectOfType<locktothepath>().CameraCine(PlayerManager.PlayerOne.transform);
        yield return new WaitForSeconds(1f);
        SceneManagerExtended.ReloadScene();
    }
}