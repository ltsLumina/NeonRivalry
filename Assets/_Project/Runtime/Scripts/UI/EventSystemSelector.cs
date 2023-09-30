using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class EventSystemSelector : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField, ReadOnly] int localPlayerID;
    
    MultiplayerEventSystem eventSystem;
    Button firstSelected;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        // get the index of the player from the PlayerInput component
        localPlayerID = GetComponent<PlayerInput>().playerIndex + 1;

        GameObject button = null;
        switch (localPlayerID)
        {
            case 1:
                button = GameObject.Find("Reaper (Player 1)");
                break;

            case 2:
                button = GameObject.Find("Reaper (Player 2)");
                break;
        }
        
        if (button != null) 
        {
            firstSelected = button.GetComponent<Button>();
            if (firstSelected == null)
            {
                Debug.LogWarning($"No Button component was found on {button.name}");
            }
        }
        else 
        {
            Debug.LogWarning("No suitable GameObject was found");
        }

        eventSystem = GetComponent<MultiplayerEventSystem>();
        if (eventSystem != null && firstSelected != null)
        { 
            eventSystem.firstSelectedGameObject = firstSelected.gameObject;
        }
    }

    public void OnButtonPress()
    {
        Debug.Log($"Player {localPlayerID} pressed a button using \"{playerInput.currentControlScheme}\" control scheme!");        
    }
}