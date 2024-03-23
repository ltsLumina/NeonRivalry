using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

[RequireComponent(typeof(Button))]
public class CharacterButton : MonoBehaviour
{
    [SerializeField] int playerIndex;
    [ShowIf("dorathy", null)]
    [SerializeField] Character shelby;
    [ShowIf("shelby", null)]
    [SerializeField] Character dorathy;

    // -- Properties --
    
    public int PlayerIndex => playerIndex;

    public bool CharacterSelected { get; set; }

    /// <summary>
    /// Returns whichever <see cref="Character"/> field is not null.
    /// </summary>
    /// <returns></returns>
    public Character GetCharacter() => shelby != null ? shelby : dorathy;

    // -- Fields --

    Button button;
    
    // -- Audio --
    Sound acceptSFX;
    Sound cancelSFX;

    void Start()
    {
        acceptSFX = new (SFX.Accept);
        cancelSFX = new (SFX.Cancel);
        
        acceptSFX.SetOutput(Output.SFX);
        cancelSFX.SetOutput(Output.SFX);
    }

    void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnDisable()
    {
        button.TryGetComponent(out Button b);
        b.onClick.RemoveListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        if (!CharacterSelected)
        {
            CharacterSelector.SelectCharacter(button);
            CharacterSelected = true;
            
            acceptSFX.Play();
        }
        else
        {
            CharacterSelector.DeselectCharacter(playerIndex, button);
            CharacterSelected = false;

            cancelSFX.Play();
        }
    }
}