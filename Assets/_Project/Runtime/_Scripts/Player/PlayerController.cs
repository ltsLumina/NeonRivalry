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
    [SerializeField, ReadOnly] public float idleTime; //TODO: Make this private, but I should probably move it to a different script.

    [Header("Ground Check"), Tooltip("The minimum distance the ray-cast must be from the ground.")]
    [SerializeField] float raycastDistance = 1.022f;
    [SerializeField] LayerMask groundLayer;

    [Header("Animation References")]
    [SerializeField] Transform characterModel;

    [Header("Player ID"), Tooltip("The player's ID. \"1\"refers to player 1, \"2\" refers to player 2.")]
    [SerializeField] [ReadOnly] int playerID;

    // Cached References
    Animator anim;
    PlayerManager playerManager;

    // -- Properties --
    
    public Rigidbody Rigidbody { get; private set; }
    public InputManager InputManager { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public PlayerInput PlayerInput { get; set; }
    
    // -- Serialized Properties --

    public int PlayerID
    {
        get => playerID;
        private set =>
            // Clamp the playerID between 1 and 2.
            playerID = Mathf.Clamp(value, 1, 2);
    }
    
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
        playerManager      = FindObjectOfType<PlayerManager>();
        
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
                PlayerManager.SetPlayerSpawnPoint(this, new (-5, 3)); // debug values; use PlayerManager.PlayerSpawnPoints[PlayerID - 1] instead
                PlayerManager.SetPlayerHealthbar(this, PlayerID);
                PlayerManager.SetPlayerInput(this, PlayerManager.PlayerInputs[PlayerID - 1]);
                break;

            case 2:
                PlayerManager.ChangePlayerColor(this, playerManager.PlayerColors.playerTwoColor);
                PlayerManager.SetPlayerSpawnPoint(this, new (5, 3)); // debug values; use PlayerManager.PlayerSpawnPoints[PlayerID - 1] instead
                PlayerManager.SetPlayerHealthbar(this, PlayerID);
                PlayerManager.SetPlayerInput(this, PlayerManager.PlayerInputs[PlayerID - 2]);
                break;

            default:
                Debug.LogError($"Invalid PlayerID: {PlayerID}. Expected either 1 or 2.");
                return;
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

    // Rotate the player to the right when spawning in to face in a direction that is more natural.
    void Start() => characterModel.Rotate(0, 75,0);

    void Update()
    {
        CheckIdle();

        RotateToFaceEnemy();
    }
    
    void RotateToFaceEnemy()
    {
        if (PlayerManager.PlayerTwo != null)
        {
            var players        = PlayerManager.Players;
            var oppositePlayer = players[PlayerID == 1 ? 1 : 0];
            var speed          = 20f;

            if (IsGrounded())
            {
                // Obtain the vector pointing from our object to the target object.
                Vector3 direction = oppositePlayer.characterModel.position - characterModel.position;

                // Zero out the y-component of the vector so the player will only rotate around the y-axis and will not tilt upwards.
                direction.y = 0;

                // Create a rotation that looks in the opposite direction of where our object is to the target (hence the negative direction).
                Quaternion rotation = Quaternion.LookRotation(-direction);
                Transform parent = oppositePlayer.transform.GetComponentInParent<PlayerController>().transform;
                
                // Gradually rotate from our current rotation to the aforementioned rotation. This smooths out the rotation so it does not snap into place.
                parent.rotation = Quaternion.Slerp
                    (parent.rotation, rotation, speed * Time.deltaTime);

                // Repeat for our character model, creating a rotation that looks in the direction of where our object is to the target.
                rotation = Quaternion.LookRotation(direction);

                // As before, gradually rotate from our current rotation to the new rotation.
                characterModel.rotation = Quaternion.Slerp(characterModel.rotation, rotation, speed * Time.deltaTime);
            }
        }
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