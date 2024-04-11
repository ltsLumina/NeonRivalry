using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

[RequireComponent(typeof(Button))]
public class CharacterButton : MonoBehaviour
{
    [Header("Show On Left/Right")]
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;
    
    [Space(10)]
    
    [SerializeField] int playerIndex;
    [ShowIf("morphe", null)]
    [SerializeField] Character shelby;
    [ShowIf("shelby", null)]
    [SerializeField] Character morphe;
    [EndIf]

    public Button LeftButton => leftButton;
    public Button RightButton => rightButton;

    // -- Properties --
    
    public int PlayerIndex => playerIndex;

    public bool CharacterSelected { get; set; }

    /// <summary>
    /// Returns whichever <see cref="Character"/> field is not null.
    /// </summary>
    /// <returns></returns>
    public Character GetCharacter() => shelby != null ? shelby : morphe;

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
        if (CharacterSelector.SelectCharacter(button))
        {
            acceptSFX.Play();
        }
        
        // else
        // {
        //     CharacterSelector.DeselectCharacter(playerIndex, button);
        //     CharacterSelected = false;
        //
        //     cancelSFX.Play();
        // }
    }
}