#region
#if UNITY_EDITOR
using UnityEditor;
#endif
using JetBrains.Annotations;
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
    [Header("Player Stats"), UsedImplicitly]
    [SerializeField, ReadOnly] public int health;
    [SerializeField, ReadOnly] Healthbar healthbar;

    [Header("Read-Only Fields")]
    [SerializeField] float idleTimeThreshold;
    [SerializeField, ReadOnly] public float idleTime; //TODO: Make this private, but I should probably move it to a different script.

    [Header("Ground Check"), Tooltip("The minimum distance the ray-cast must be from the ground.")]
    [SerializeField] float raycastDistance = 1.022f;
    [SerializeField] LayerMask groundLayer;

    [Header("Player ID"), Tooltip("The player's ID. \"1\"refers to player 1, \"2\" refers to player 2.")]
    [SerializeField] [ReadOnly] int playerID;

    // Cached References
    Animator animator;

    // -- Properties --
    
    public Rigidbody Rigidbody { get; private set; }
    public InputManager InputManager { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public PlayerInput PlayerInput { get; set; }
    public HitBox HitBox { get; set; }
    public HurtBox HurtBox { get; set; }

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
        Rigidbody     = GetComponent<Rigidbody>();
        InputManager  = GetComponentInChildren<InputManager>();
        StateMachine  = GetComponent<StateMachine>();
        PlayerInput   = GetComponentInChildren<PlayerInput>();
        animator      = GetComponentInChildren<Animator>();

        HitBox  = GetComponentInChildren<HitBox>();
        HurtBox = GetComponentInChildren<HurtBox>();
    }

    // Rotate the player when spawning in to face in a direction that is more natural.
    void Start() => Initialize();

    void Update()
    {
        CheckIdle();
        
        RotateToFaceEnemy();
    }
    
    /// <summary>
    ///     Initialize the player to the correct state.
    ///     This includes setting the player's color and spawn point as well as the player's ID.
    /// </summary>
    void Initialize()
    {
        PlayerManager.AddPlayer(this);
        
        PlayerID = PlayerInput.playerIndex + 1;
        gameObject.name = $"Player {PlayerID}";
        
        // Parenting the player to the header is purely for organizational purposes.
        Transform header = GameObject.FindGameObjectWithTag("[Header] Players").transform;

        if (header == null)
        {
            Debug.LogError("Header not found. Please check if the tag is correct.");
            return;
        }

        transform.SetParent(header);

        // Rotate to face the camera.
        // This has no gameplay purpose and only serves as a visual aid.
        transform.rotation = Quaternion.Euler(0, 180, 0);

        var playerManager = PlayerManager.Instance;

        playerManager.SetPlayerSpawnPoint(this, PlayerID);
        PlayerManager.AssignHealthbarToPlayer(this, PlayerID);

        // TODO: Change this once we have a system in place to determine when the round actually starts.
        //gameObject.SetActive(false);
    }

    void RotateToFaceEnemy()
    {
        if (PlayerManager.PlayerTwo == null) return;

        var players         = PlayerManager.Players;
        var oppositePlayer = players[PlayerID == 1 ? 1 : 0];

        if (IsGrounded())
        {
            if (oppositePlayer.transform.position.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
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

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerController))]
public class PlayerControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var player = (PlayerController) target;
        var healthbar = player.Healthbar;
        if (player == null || healthbar == null) return;

        // Replace the health variable in the inspector with the healthbar's value
        // so that the healthbar's value can be changed in the inspector.
        player.health = healthbar.Value;
    }
}
#endif