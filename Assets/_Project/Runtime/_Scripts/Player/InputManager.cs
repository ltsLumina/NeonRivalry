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
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public enum AttackType
    {
        None,
        Punch,
        Kick,
        Slash,
        Airborne,
        Unique
    }

    public AttackType LastAttackPressed { get; set; } = AttackType.None;
    
    [Header("Read-Only Fields")]
    [SerializeField, ReadOnly] Vector2 moveInput;

    // Cached References
    PlayerController player;
    StateMachine stateMachine;

    // Serialized InputAction. Must be public as it can't be serialized through [SerializeField].
    public InputAction runKeyModifier;

    // temporary
    bool isRunningKeyHeld;

    public Vector2 MoveInput
    {
        get => moveInput;
        private set => moveInput = value;
    }

    void OnEnable() => runKeyModifier.Enable();

    void OnDisable() => runKeyModifier.Disable();

    void Awake()
    {
        player       = GetComponentInParent<PlayerController>();
        stateMachine = player.GetComponent<StateMachine>();
        
        // Disable the PlayerInput on the UI Navigation 
    }

    void Update()
    {
        if (runKeyModifier.triggered)
        {
            isRunningKeyHeld = true;
            Debug.Log($"isRunning: {isRunningKeyHeld}");
        }
    }

    // -- Input Handling --
    
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        
        if (context.performed && player.CanMove())
        {
            stateMachine.TransitionToState(State.StateType.Walk);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && player.CanJump())
        {
            stateMachine.TransitionToState(State.StateType.Jump);
        }
    }

    [Obsolete("Use OnPunch, OnKick, OnSlash, or OnUnique instead.")]
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && player.CanAttack())
        {
            stateMachine.TransitionToState(State.StateType.Attack);
        }
    }

    #region Attack Input Handling

    public void OnPunch(InputAction.CallbackContext context)
    {
        if (context.performed && player.CanAttack())
        {
            // If play is grounded, enter grounded Attack state, otherwise enter Airborne Attack state.
            if (player.IsGrounded())
            {
                Debug.Log("Grounded Attacking!");
                LastAttackPressed = AttackType.Punch;
                stateMachine.TransitionToState(State.StateType.Attack);
            }
            else
            {
                Debug.Log("Airborne Attacking!");
                LastAttackPressed = AttackType.Airborne;
                stateMachine.TransitionToState(State.StateType.AirborneAttack);
            }
        }
    }
    
    public void OnKick(InputAction.CallbackContext context)
    {
        if (context.performed && player.CanAttack())
        {
            // If play is grounded, enter grounded Attack state, otherwise enter Airborne Attack state.
            if (player.IsAirborne())
            {
                LastAttackPressed = AttackType.Punch;
                stateMachine.TransitionToState(State.StateType.Attack);
            }
            else
            {
                LastAttackPressed = AttackType.Airborne;
                stateMachine.TransitionToState(State.StateType.AirborneAttack);
            }
        }
    }
    
    public void OnSlash(InputAction.CallbackContext context)
    {
        if (context.performed && player.CanAttack())
        {
            // If play is grounded, enter grounded Attack state, otherwise enter Airborne Attack state.
            if (player.IsAirborne())
            {
                LastAttackPressed = AttackType.Punch;
                stateMachine.TransitionToState(State.StateType.Attack);
            }
            else
            {
                LastAttackPressed = AttackType.Airborne;
                stateMachine.TransitionToState(State.StateType.AirborneAttack);
            }
        }
    }
    
    public void OnUnique(InputAction.CallbackContext context)
    {
        if (context.performed && player.CanAttack())
        {
            // If play is grounded, enter grounded Attack state, otherwise enter Airborne Attack state.
            if (player.IsAirborne())
            {
                LastAttackPressed = AttackType.Punch;
                stateMachine.TransitionToState(State.StateType.Attack);
            }
            else
            {
                LastAttackPressed = AttackType.Airborne;
                stateMachine.TransitionToState(State.StateType.AirborneAttack);
            }
        }
    }
    
    #endregion
}