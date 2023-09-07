#region
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
    }

    // -- Input Handling --

    void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();

        //TODO: CanMove does not work as intended. If I change it to IsGrounded, it works as expected.
        if (player.IsGrounded() && !player.IsAttacking())
        {
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

        stateMachine.TransitionToState(State.StateType.Attack);
    }
}