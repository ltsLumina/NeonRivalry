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
    [Header("Read-Only Fields")]
    [SerializeField] float idleTimeThreshold;
    [SerializeField, ReadOnly] public float idleTime;

    [Header("Ground Check"), Tooltip("The minimum distance the raycast must be from the ground."), SerializeField, ReadOnly]
     float raycastDistance = 1.022f; //With the capsule collider, this is the minimum distance between player and ground.
    [SerializeField] LayerMask groundLayer;

    [Header("Player ID"), Tooltip("The player's ID. \"1\"refers to player 1, \"2\" refers to player 2.")]
    [SerializeField, ReadOnly] int playerID;

    // Cached References
    Animator anim;
    PlayerManager playerManager;


    public int PlayerID
    {
        get => playerID;
        private set
        {
            // Clamp the playerID between 1 and 2.
            playerID = Mathf.Clamp(playerID, 1, 2);
            playerID = value; 
        }
    }
    public Rigidbody Rigidbody { get; private set; }
    public InputManager InputManager { get; private set; }
    public StateMachine StateMachine { get; private set; }

    void Awake()
    {
        Rigidbody    = GetComponent<Rigidbody>();
        InputManager = GetComponentInChildren<InputManager>();
        StateMachine = GetComponent<StateMachine>();
        playerManager = PlayerManager.Instance;
        
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
                break;

            case 2:
                PlayerManager.ChangePlayerColor(this, playerManager.PlayerColors.playerTwoColor);
                PlayerManager.SetPlayerSpawnPoint(this, playerManager.PlayerSpawns.playerTwoSpawnPoint);
                break;
        }
    }

    void OnDestroy() => PlayerManager.RemovePlayer(this);

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