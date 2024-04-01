using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class ConfirmCharactersButton : MonoBehaviour
{
    Button confirmationButton;
    bool confirming;

    void Start()
    {
        confirmationButton = GetComponentInChildren<Button>();
        confirmationButton.gameObject.SetActive(false);
    }

    void Update()
    {
        confirming = CharacterSelector.SelectedCharacters.Count == 2;

        if (confirming) ConfirmCharacters();
        else confirmationButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Once both players have selected their characters, confirm the selected characters
    /// and proceed to the game scene.
    /// </summary>
    void ConfirmCharacters()
    {
        confirmationButton.gameObject.SetActive(true);

        foreach (var player in FindObjectsOfType<MenuNavigator>())
        {
            var eventSystem = player.GetComponent<MultiplayerEventSystem>();
            eventSystem.SetSelectedGameObject(confirmationButton.gameObject);
        }
    }

    public void PlayConfirmSound()
    {
        Sound CSConfirm = new (SFX.CSConfirm);
        CSConfirm.SetOutput(Output.SFX);
        CSConfirm.Play();
    }
}
