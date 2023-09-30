using UnityEngine;

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
        if (inputDeviceManager.PlayerInputs.Count == 0)
        {
            inputDeviceManager.JoinPlayerCharacter(playerPrefab);
        }
        else if (inputDeviceManager.PlayerInputs.Count == 1)
        {
            inputDeviceManager.JoinPlayerCharacter(playerPrefab);
        }
    }

}
