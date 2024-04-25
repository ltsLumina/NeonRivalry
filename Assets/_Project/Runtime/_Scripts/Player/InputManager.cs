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
[DisallowMultipleComponent]
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
        Unique,
        Airborne
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
    
    public static PlayerController PausingPlayer { get; private set; }
    
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

    void Start() => Enabled = true;

    // -- Input Handling --
    
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

        // Transition to move state if the player is not crouching or blocking.
        TransitionTo(context, player.CanMove, State.StateType.Walk);
    }
    
    public void OnJump(InputAction.CallbackContext context) => TransitionTo(context, player.CanJump, State.StateType.Jump);
    
    public void OnDash(InputAction.CallbackContext context) => TransitionTo(context, player.CanDash, State.StateType.Dash);

    /// <summary>
    /// Handles state transition.
    /// </summary>
    void TransitionTo(InputAction.CallbackContext context, Func<bool> condition, State.StateType stateType)
    {
        if (!Enabled) return;
        if (context.performed && condition()) stateMachine.TransitionToState(stateType);
    }

    #region Attack Input Handling
    
    public void OnPunch(InputAction.CallbackContext context) => PerformAttack(context, AttackType.Punch);
    public void OnKick(InputAction.CallbackContext context) => PerformAttack(context, AttackType.Kick);
    public void OnSlash(InputAction.CallbackContext context) => PerformAttack(context, AttackType.Slash);
    
    // Due to a bug in the Input System, the OnUnique method must be called manually inside the PlayerController script,
    // rather than being called through a UnityEvent like the other attack methods.
    public void OnUnique(InputAction.CallbackContext context) => PerformAttack(context, AttackType.Unique);

    public void OnPause(InputAction.CallbackContext context)
    {
        if (TimelinePlayer.IsPlaying) return;
        
        if (context.performed)
        {
            var menuManager = FindObjectOfType<MenuManager>();

            if (GameManager.IsPaused)
            {
                if (PausingPlayer != player) return;
                
                if (menuManager.creditsMenu.activeSelf)
                {
                    menuManager.CloseCreditsInGameScene();
                    return;
                }

                menuManager.CloseCurrentSettingsMenuInGameScene();
                GameManager.TogglePause(player);
                PausingPlayer = null;
            }
            else
            {
                menuManager.ToggleSettings();
                GameManager.TogglePause(player);
                PausingPlayer = player;
            }
        }
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