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
    [SerializeField] int wonRounds1;
    [SerializeField] int wonRounds2;

    [Header("Bools")]
    public bool p1HasWon;
    public bool p2HasWon;
    public bool timer0;

    locktothepath locktothepathS;
    
    protected override void Awake()
    {
        locktothepathS = FindObjectOfType<locktothepath>();
    }


private void Update()
{
    CheckingBools();
    PlayerHasWon();
}

private void CheckingBools()
{
    if (timer0 && currentRounds <= maxRounds) { currentRounds++; SceneManagerExtended.ReloadScene(); }
    //else if (p2HasWon || p1HasWon)
    //{
    //    currentRounds++; SceneManagerExtended.ReloadScene();
    //}
    else if (currentRounds > maxRounds)
    {
        SceneManagerExtended.LoadNextScene();
            currentRounds = 0;
    }
}

public void PlayerHasWon()
{
    if (Input.GetKeyDown(KeyCode.N))
    {
        FindObjectOfType<locktothepath>().CameraCine(PlayerManager.PlayerOne.transform);
    }
}
}