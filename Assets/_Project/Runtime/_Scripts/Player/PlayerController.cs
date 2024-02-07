#region
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
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
    [SerializeField, ReadOnly] int playerID;
    
    [Header("Player Components")]
    [SerializeField] Animator combatAnimator;
    [SerializeField] Animator movementAnimator;

    

    // Cached References
    float moveSpeed = 3f;
    float acceleration = 8f;
    float deceleration = 10f;
    float velocityPower = 1.4f;

    // -- Properties --
    
    public Rigidbody Rigidbody { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public InputManager InputManager { get; private set; }
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
        StateMachine  = GetComponent<StateMachine>();
        InputManager  = GetComponentInChildren<InputManager>();
        PlayerInput   = GetComponentInChildren<PlayerInput>();

        HitBox  = GetComponentInChildren<HitBox>();
        HurtBox = GetComponentInChildren<HurtBox>();
    }

    // Rotate the player when spawning in to face in a direction that is more natural.
    void Start() => Initialize();

    void Update()
    {
        CheckIdle();
        
        RotateToFaceEnemy();

        //TODO: Temporary fix to test new state machine.
        if (StateMachine.CurrentState is not AttackState or AirborneAttackState)
        {
            if (IsGrounded())
            {
                DEBUGMovement();
            } 
        }
    }

    void DEBUGMovement()
    {
        // Getting the move input from the player's input manager.
        Vector3 moveInput = InputManager.MoveInput;

        // Animating the player based on the move input.
        movementAnimator.SetBool("Walk_Forward", moveInput.x > 0);
        movementAnimator.SetBool("Walk_Backward", moveInput.x < 0);

        // Determining the direction of the movement (left or right).
        int moveDirection = (int) moveInput.x;

        // Calculating the target speed based on direction and move speed.
        float targetSpeed = moveDirection * moveSpeed;

        // Calculate difference between target speed and current velocity.
        float speedDifference = targetSpeed - Rigidbody.velocity.x;

        // Determine the acceleration rate based on whether the target speed is greater than 0.01 or not.
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        // Calculate the final movement force to be applied on the player's rigidbody.
        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) * Mathf.Sign(speedDifference);

        // Apply the force to the player's rigidbody.
        Rigidbody.AddForce(movement * Vector3.right);
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
        const string headerTag = "[Header] Players";
        Transform header = GameObject.FindGameObjectWithTag(headerTag).transform;

        if (header == null)
        {
            Debug.LogError("Header not found. Please check if the tag is correct.");
            return;
        }

        transform.SetParent(header);

        // Rotate to face the camera.
        // This has no gameplay purpose and only serves as a visual aid.
        var model = transform.GetComponentInChildren<Animator>().transform;
        model.rotation = Quaternion.Euler(0, PlayerID == 1 ? 120 : 240, 0);

        var playerManager = PlayerManager.Instance;

        playerManager.SetPlayerSpawnPoint(this, PlayerID);
        PlayerManager.AssignHealthbarToPlayer(this, PlayerID);

        // TODO: Change this once we have a system in place to determine when the round actually starts.
        //gameObject.SetActive(false);
    }

    void RotateToFaceEnemy()
    {
        if (PlayerManager.PlayerTwo == null) return;
        List<PlayerController> players        = PlayerManager.Players;
        var                    oppositePlayer = players[PlayerID == 1 ? 1 : 0];
        var                    model          = transform.GetComponentInChildren<Animator>().transform;

        Quaternion targetRotation;

        if (oppositePlayer.transform.position.x > transform.position.x)
        {
            model.localScale = new (1, 1, 1);
            targetRotation   = Quaternion.Euler(0, 70, 0);
        }
        else
        {
            model.localScale = new (-1, 1, 1);
            targetRotation   = Quaternion.Euler(0, -70, 0);
        }

        // Lerp rotation over time
        float rotationSpeed = 0.75f; // Adjust this value to change the speed of rotation
        model.rotation = Quaternion.Lerp(model.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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