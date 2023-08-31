using UnityEngine;
using UnityEngine.InputSystem;
using Lumina.Essentials.Attributes;

/// <summary>
/// Handles all player input, such as movement, jumping, attacking, etc.
/// Partial class, see StateChecks.cs for the input checks.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    [Header("Read-Only Fields")]
    [SerializeField, ReadOnly] Vector2 moveInput;

    // Cached References
    StateMachine stateMachine;
    PlayerController player;
    Rigidbody playerRB;

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

    void Start()
    {
        stateMachine = StateMachine.Instance;
        player       = GetComponent<PlayerController>();
        playerRB     = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (runKeyModifier.triggered)
        {
            isRunningKeyHeld = true;
            Debug.Log($"isRunning: {isRunningKeyHeld}");
        }
        
        // check if player is moving
        //TODO: This somehow, technically works. I don't know why or how, but it does.
        //TODO: Look into it later.
        if (MoveInput.x != 0 && stateMachine.CurrentState is not MoveState && stateMachine.CurrentState is not JumpState)
        {
            Debug.Log("Player is moving");
            stateMachine.TransitionToState(State.StateType.Walk);
        }
    }

    // -- Input Handling --

    void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();

        //TODO: CanMove does not work as intended. If I change it to IsGrounded, it works as expected.
        if (player.IsGrounded())
        {
            #region Note about future update
            // Note: I should check if the player is already moving before transitioning again.
            // However, the IsMoving bool wont update as intended if I do that.
            // The boolean is set to true the first time you enter the state. If it gets set to false while in the move state, it wont be updated to true until you leave
            // and enter the state again.
            #endregion
            stateMachine.TransitionToState(State.StateType.Walk);
        }
        else if (player.IsGrounded() && isRunningKeyHeld)
        {
            stateMachine.TransitionToState(State.StateType.Run); 
        }
    }

    void OnJump(InputValue value)
    {
        if (player.CanJump())
        {
            stateMachine.TransitionToState(State.StateType.Jump);
        }
    }

    void OnAttack(InputValue value)
    {
        if (player.CanAttack())
        {
            stateMachine.TransitionToState(State.StateType.Attack);
        }
    }
}