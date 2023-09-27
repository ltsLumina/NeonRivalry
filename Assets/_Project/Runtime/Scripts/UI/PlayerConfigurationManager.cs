#region
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
#endregion

public class PlayerConfigurationManager : MonoBehaviour
{
    List<PlayerConfiguration> playerConfigs;
    [SerializeField] int MaxPlayers = 2;

    public static PlayerConfigurationManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Debug.Log("[Singleton] Trying to instantiate a second instance of a singleton class."); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new ();
        }
    }

    public void HandlePlayerJoin(PlayerInput playerInput)
    {
        Debug.Log("player joined " + playerInput.playerIndex);
        playerInput.transform.SetParent(transform);

        if (playerConfigs.All(p => p.PlayerIndex != playerInput.playerIndex)) playerConfigs.Add(new (playerInput));
    }

    public List<PlayerConfiguration> GetPlayerConfigs() => playerConfigs;

    public void SetPlayerColor(int index, Material color) => playerConfigs[index].PlayerMaterial = color;

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].IsReady = true;
        if (playerConfigs.Count == MaxPlayers && playerConfigs.All(p => p.IsReady)) SceneManager.LoadScene("Dev_CharacterSelect");
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput playerInput)
    {
        PlayerIndex = playerInput.playerIndex;
        Input       = playerInput;
    }

    public PlayerInput Input { get; private set; }
    public int PlayerIndex { get; }
    public bool IsReady { get; set; }
    public Material PlayerMaterial { get; set; }
}
