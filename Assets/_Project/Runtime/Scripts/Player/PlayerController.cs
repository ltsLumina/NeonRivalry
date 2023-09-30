#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using static State;
#endregion

/// <summary>
/// The PlayerController is intended to be used as a base class for the player.
/// It is used to manage the individual player's input, movement, and state.
/// On the other hand, the PlayerManager is used to manage all players in the game including their settings, properties, and actions.
/// <seealso cref="PlayerManager"/>
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public partial class PlayerController : MonoBehaviour
{
    [Header("Player Stats"), ReadOnly]
    [SerializeField] Healthbar healthbar;
    
    [Header("Read-Only Fields")]
    [SerializeField] float idleTimeThreshold;
    [SerializeField, ReadOnly] public float idleTime;

    [Header("Ground Check"), Tooltip("The minimum distance the raycast must be from the ground.")] 
    [SerializeField, ReadOnly] float raycastDistance = 1.022f; //With the capsule collider, this is the minimum distance between player and ground.
    [SerializeField] LayerMask groundLayer;

    [Header("Player ID"), Tooltip("The player's ID. \"1\"refers to player 1, \"2\" refers to player 2.")]
    [SerializeField, ReadOnly] int playerID;

    // Cached References
    Animator anim;
    PlayerManager playerManager;
    InputDeviceManager inputDeviceManager;

    // -- Properties --
    public int PlayerID
    {
        get => playerID;
        private set
        {
            // Clamp the playerID between 1 and 2.
            playerID = Mathf.Clamp(value, 1, 2);
        }
    }
    public Rigidbody Rigidbody { get; private set; }
    public InputManager InputManager { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public PlayerInput PlayerInput { get; set; }
    
    public Healthbar Healthbar
    {
        get => healthbar;
        set => healthbar = value;
    }

    void Awake()
    {
        // Get the player's rigidbody, input manager, and state machine.
        Rigidbody          = GetComponent<Rigidbody>();
        InputManager       = GetComponentInChildren<InputManager>();
        StateMachine       = GetComponent<StateMachine>();
        playerManager      = PlayerManager.Instance;
        inputDeviceManager = FindObjectOfType<InputDeviceManager>();
        
        Initialize();
    }

    /// <summary>
    ///     Initialize the player to the correct state.
    ///     This includes setting the player's color and spawn point as well as the player's ID.
    /// </summary>
    void Initialize()
    {
        PlayerManager.AddPlayer(this);

        // Get the player's ID.
        var playerInputManager = FindObjectOfType<PlayerInputManager>();
        PlayerID = playerInputManager.playerCount;

        // Set the player's name and parent.
        gameObject.name = $"Player {PlayerID}";
        Transform header = GameObject.FindGameObjectWithTag("[Header] Players").transform;

        // Check if Header is found
        if (header == null)
        {
            Debug.LogError("Header not found. Please check if the tag is correct.");
            return;
        }

        // Set the player's parent to the header for better organization.
        transform.SetParent(header);
        
        // Change the colour and spawn position of the player.
        switch (PlayerID)
        {
            case 1:
                PlayerManager.ChangePlayerColor(this, playerManager.PlayerColors.playerOneColor);
                PlayerManager.SetPlayerSpawnPoint(this, playerManager.PlayerSpawns.playerOneSpawnPoint);
                PlayerManager.SetPlayerHealthbar(this, PlayerID);
                PlayerManager.SetPlayerInput(this, inputDeviceManager.PlayerInputs[0]);
                break;

            case 2:
                PlayerManager.ChangePlayerColor(this, playerManager.PlayerColors.playerTwoColor);
                PlayerManager.SetPlayerSpawnPoint(this, playerManager.PlayerSpawns.playerTwoSpawnPoint);
                PlayerManager.SetPlayerHealthbar(this, PlayerID);
                PlayerManager.SetPlayerInput(this, inputDeviceManager.PlayerInputs[1]);
                break;
        }
    }
    

    void OnDisable()
    {
        Terminate();

        return;
        void Terminate()
        {
            PlayerManager.RemovePlayer(this);

            const string warningMessage = "The player has been disabled! " + "Please refrain from disabling the player and opt to destroy it instead. \n" +
                                          "If the object was destroyed correctly, please ignore this message.";

            Debug.LogWarning(warningMessage);
        }
    }

    void OnDestroy()
    {
        //inputDeviceManager.OnPlayerLeft(PlayerID);
    }

    void Update()
    {
        CheckIdle();
    }

    // -- State Checks --
    // Related: StateChecks.cs

    void CheckIdle()
    {
        // Check if the player is idle.
        if (IsIdle())
        {
            // If the player is idle, we add to the idle time.
            idleTime += Time.deltaTime;

            // If the idle time is greater than the threshold, we transition to the idle state.
            if (idleTime >= idleTimeThreshold)
            {
                StateMachine.TransitionToState(StateType.Idle);
            }
        }
        else { idleTime = 0; }
    }
}