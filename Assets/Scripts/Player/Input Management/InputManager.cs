using UnityEngine;
using UnityEngine.InputSystem;
using static Essentials.Attributes;

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
    Rigidbody2D playerRB;

    public InputAction runKeyModifier;

    // temporary
    bool isRunning;
    bool jumpInputIsHeld;

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
        playerRB     = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (runKeyModifier.triggered)
        {
            isRunning = true;
            Debug.Log($"isRunning: {isRunning}");
        }
    }

    void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();

        //TODO: Walking and jumping on controller is weird, fix it.
        //TODO: Might have something to do with our ground check.
        if (player.IsGrounded() && !player.IsJumping() && !player.IsFalling())
        {
            // Regular Walk State
            stateMachine.HandleStateChange(State.StateType.Walk);
        }

        //TODO: isRunning is a temporary state for testing purposes.
        if (player.IsGrounded() && isRunning)
        {
            // Running State
            stateMachine.HandleStateChange(State.StateType.Run);
        }
    }

    void OnJump(InputValue value)
    {
        jumpInputIsHeld = value.isPressed;

        if (player.IsGrounded())
        {
            stateMachine.HandleStateChange(State.StateType.Jump);
        }
    }

    //TODO: rework this system.
    public bool JumpInputIsHeld()
    {
        return jumpInputIsHeld;
    }

    void OnAttack(InputValue value)
    {
        stateMachine.HandleStateChange(State.StateType.Attack);
    }
}