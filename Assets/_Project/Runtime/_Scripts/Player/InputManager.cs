#region
using System;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

/// <summary>
/// Handles all player input, such as movement, jumping, attacking, etc.
/// Partial class, see StateChecks.cs for the input checks.
/// </summary>
public class InputManager : MonoBehaviour
{
    [Header("Read-Only Fields")]
    [SerializeField, ReadOnly] Vector2 moveInput;
    
    /// <summary>
    /// Enum for Attack Types.
    /// </summary>
    public enum AttackType
    {
        None,
        Punch,
        Kick,
        Slash,
        Airborne,
        Unique
    }

    /// <summary>
    /// Gets or sets the last pressed attack type.
    /// </summary>
    public AttackType LastAttackPressed { get; set; } = AttackType.None;

    /// <summary>
    /// Gets or sets the move input.
    /// </summary>
    public Vector2 MoveInput
    {
        get => moveInput;
        private set => moveInput = value;
    }
    
    /// <summary>
    /// Whether the input manager is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    // Cached References
    PlayerController player;
    StateMachine stateMachine;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        player       = GetComponentInParent<PlayerController>();
        stateMachine = player.GetComponent<StateMachine>();
    }

    // -- Input Handling --

    /// <summary>
    /// Handles the move input from the player.
    /// <para></para>
    /// Calls a necessary transition in the player's state machine if the player
    /// is in a state where movement is a valid action.
    /// </summary>
    /// <param name="context">The context from the input system containing movement input values</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

        // Handle single player blocking
        if (PlayerManager.PlayerControllerCount < 2)
        {
            player.IsBlocking = MoveInput.x < 0;
            return;
        }
        
        if (IsBlocking()) return;

        // Transition to move state if the player is not crouching or blocking.
        TransitionTo(context, player.CanMove, State.StateType.Walk);

        return;
        bool IsBlocking()
        {
            // Get the direction to the other player
            if (PlayerManager.OtherPlayerController(player) == null) return false;
            Vector3 dirOtherPlayer = PlayerManager.OtherPlayerController(player).transform.position - player.transform.position;
            dirOtherPlayer.Normalize();

            // Get the direction of movement
            Vector3 moveDirection = new Vector3(MoveInput.x, 0, MoveInput.y);
            moveDirection.Normalize();

            // Calculate the dot product
            float dotProduct = Vector3.Dot(dirOtherPlayer, moveDirection);

            // If the dot product is less than 0, the player is blocking
            return player.IsBlocking = dotProduct < 0;
        }
    }

    /// <summary>
    /// Handles jump input.
    /// </summary>
    public void OnJump(InputAction.CallbackContext context) => TransitionTo(context, player.CanJump, State.StateType.Jump);
    
    public void OnDash(InputAction.CallbackContext context) => TransitionTo(context, player.CanDash, State.StateType.Dash);

    /// <summary>
    /// Handles state transition.
    /// </summary>
    void TransitionTo(InputAction.CallbackContext context, Func<bool> condition, State.StateType stateType)
    {
        if (!player.enabled) return;
        if (context.performed && condition()) stateMachine.TransitionToState(stateType);
    }

    #region Attack Input Handling

    /// <summary>
    /// Handles punch input.
    /// </summary>
    public void OnPunch(InputAction.CallbackContext context) => PerformAttack(context, AttackType.Punch);

    /// <summary>
    /// Handles kick input.
    /// </summary>
    public void OnKick(InputAction.CallbackContext context) => PerformAttack(context, AttackType.Kick);

    /// <summary>
    /// Handles slash input.
    /// </summary>
    public void OnSlash(InputAction.CallbackContext context) => PerformAttack(context, AttackType.Slash);

    /// <summary>
    /// Handles unique input.
    /// </summary>
    public void OnUnique(InputAction.CallbackContext context) => PerformAttack(context, AttackType.Unique);

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) GameManager.TogglePause(player);
    }
    
    /// <summary>
    /// Performs attack action.
    /// </summary>
    void PerformAttack(InputAction.CallbackContext context, AttackType attackType)
    {
        if (context.performed && Enabled)
        {
            if (player.IsGrounded() && player.CanAttack())
            {
                LastAttackPressed = attackType;
                stateMachine.TransitionToState(State.StateType.Attack);
            }
            else if (player.CanAirborneAttack())
            {
                LastAttackPressed = AttackType.Airborne;
                stateMachine.TransitionToState(State.StateType.AirborneAttack);
            }
        }
    }
    
    #endregion
}