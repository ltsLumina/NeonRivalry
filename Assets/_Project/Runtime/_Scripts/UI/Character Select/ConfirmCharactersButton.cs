#region
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
#endregion

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
    
        if (confirming) ChooseMap();
        else confirmationButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Once both players have selected their characters, confirm the selected characters
    /// and proceed to the game scene.
    /// </summary>
    public void ChooseMap()
    {
        var mapSelector = FindObjectOfType<MapSelector>(true);
        mapSelector.gameObject.SetActive(true);
    }

    public void Confirm()
    {
        confirmationButton.gameObject.SetActive(true);

        foreach (var player in FindObjectsOfType<MenuNavigator>())
        {
            var eventSystem = player.GetComponent<MultiplayerEventSystem>();
            eventSystem.SetSelectedGameObject(confirmationButton.gameObject);
        }
        
        MapSelector mapSelector = FindObjectOfType<MapSelector>();
        if (mapSelector == null) return;
        
        mapSelector.LoadMap();
    }

    public void PlayConfirmSound()
    {
        Sound CSConfirm = new (SFX.CSConfirm);
        CSConfirm.SetOutput(Output.SFX);
        CSConfirm.Play();
    }
}
