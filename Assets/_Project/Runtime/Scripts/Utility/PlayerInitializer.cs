using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    
    InputDeviceManager inputDeviceManager;

    void Awake()
    {
        inputDeviceManager = FindObjectOfType<InputDeviceManager>();
    }

    void Update()
    {
        // Join both players
        if (PlayerInputManager.instance.playerCount == 0)
        {
            inputDeviceManager.JoinPlayerCharacter(playerPrefab);
        }
        else if (PlayerInputManager.instance.playerCount == 1)
        {
            inputDeviceManager.JoinPlayerCharacter(playerPrefab);
        }
    }

}
