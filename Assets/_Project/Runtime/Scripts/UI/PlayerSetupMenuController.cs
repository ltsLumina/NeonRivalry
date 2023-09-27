#region
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class PlayerSetupMenuController : MonoBehaviour
{
    int playerIndex;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] GameObject readyPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] Button readyButton;

    float ignoreInputTime = 1.5f;
    bool inputEnabled;
    public void SetPlayerIndex(int playerIndex)
    {
        this.playerIndex = playerIndex;
        //titleText.SetText("Player " + (playerIndex + 1));
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > ignoreInputTime) inputEnabled = true;
    }

    public void SelectColor(Material material)
    {
        if (!inputEnabled) return;

        PlayerConfigurationManager.Instance.SetPlayerColor(playerIndex, material);
        readyPanel.SetActive(true);
        readyButton.interactable = true;
        menuPanel.SetActive(false);
        readyButton.Select();
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) return;

        PlayerConfigurationManager.Instance.ReadyPlayer(playerIndex);
        readyButton.gameObject.SetActive(false);
    }
}
