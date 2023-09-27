#region
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endregion

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;

    
    
    // public GameObject playerSetupMenuPrefab;
    //
    // GameObject rootMenu;
    // public PlayerInput input;
    //
    // void Awake()
    // {
    //     rootMenu = GameObject.Find("UI");
    //
    //     if (rootMenu != null)
    //     {
    //         GameObject menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
    //         input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
    //         menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);
    //     }
    // }
    
    public void HopeThisWorks()
    {
        Debug.Log("Hope this works" + playerInput.playerIndex + playerInput.user.controlScheme);
    }
}
