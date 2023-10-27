#region
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class RoundScript : SingletonPersistent<RoundScript>
{
    [Header("Components")]
    //commentet out for debugging
    //public Image won2RoundPlayer1;
    //public Image won1RoundPlayer1;
    //public Image won2RoundPlayer2;
    //public Image won1RoundPlayer2;

    //these are temporary
    public static TextMeshProUGUI player1WonRoundsText;
    public static TextMeshProUGUI player2WonRoundsText;

    [Header("Rounds")]
    [SerializeField] int maxRounds;
    [SerializeField] public int currentRounds;
    [SerializeField] public int player1WonRounds;
    [SerializeField] public int player2WonRounds;

    [Header("Bools")]
    public bool p1HasWon;
    public bool p2HasWon;
    public bool timer0;

    protected override void Awake()
    {
        base.Awake();

        // Clear the static variables as they wont be reset automatically by Unity due to EnterPlaymodeOptions.
        player1WonRoundsText = null;
        player2WonRoundsText = null;
    }

    private void Start()
    {
        var roundsText1 = GameObject.Find("P1RoundsText");
        var roundsText2 = GameObject.Find("P2RoundsText");
        var text1 = roundsText1.GetComponent<TextMeshProUGUI>();
        var text2 = roundsText2.GetComponent<TextMeshProUGUI>();
        player1WonRoundsText = text1;
        player2WonRoundsText = text2;

        Debug.Log(text1, text1);
        Debug.Log(text2, text2);
    }

    void Update() => CheckingBools();

    void CheckingBools()
    {
        if (currentRounds > maxRounds || player1WonRounds >= 2 || player2WonRounds >= 2)
        {
            SceneManagerExtended.LoadNextScene();
            currentRounds = 0;
            player1WonRounds = 0;
            player2WonRounds = 0;
            player1WonRoundsText.text = string.Empty;
            player2WonRoundsText.text = string.Empty;
        }

        switch (timer0)
        {
            case true when PlayerManager.PlayerOne.Healthbar.Value > PlayerManager.PlayerTwo.Healthbar.Value && currentRounds <= maxRounds:
                player1WonRounds++;
                currentRounds++;
                player1WonRoundsText.text = $"Rounds won: \n{player1WonRounds}/2";

                //SceneManagerExtended.ReloadScene();
                break;

            case true when PlayerManager.PlayerTwo.Healthbar.Value > PlayerManager.PlayerOne.Healthbar.Value && currentRounds <= maxRounds:
                player2WonRounds++;
                currentRounds++;
                player2WonRoundsText.text = $"Rounds won: \n{player2WonRounds}/2";

                //SceneManagerExtended.ReloadScene();
                break;
        }
        //debugging :pukeemoji:
        if (Input.GetKeyDown(KeyCode.Z))
        {
            player1WonRounds++;
            currentRounds++;
            player1WonRoundsText.text = $"Rounds won: \n{player1WonRounds}/2";
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            player2WonRounds++;
            currentRounds++;
            player2WonRoundsText.text = $"Rounds won: \n{player2WonRounds}/2";  
        }
    }

    IEnumerator WinningCamera()
    {
        FindObjectOfType<LockToDollyPath>().CameraCine(PlayerManager.PlayerOne.transform);
        yield return new WaitForSeconds(1f);
        SceneManagerExtended.ReloadScene();
    }
}
