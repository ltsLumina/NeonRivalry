using UnityEngine;
using UnityEngine.InputSystem;
using static Essentials.Attributes;

/// <summary>
/// Handles all player input, such as movement, jumping, attacking, etc.
/// Partial class, see InputChecks.cs for the input checks.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public partial class InputManager : MonoBehaviour
{
    [SerializeField] float idleTimeThreshold;

    [Header("Read-Only Fields")]
    [SerializeField, ReadOnly] Vector2 moveInput;
    [SerializeField, ReadOnly] float idleTime;

    // Cached References
    StateMachine stateMachine;
    PlayerController player;
    Rigidbody2D playerRB;

    public InputAction runKeyModifier;

    // temporary
    bool isRunning;

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
        CheckIdle();

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
        if (player.IsGrounded())
        {
            // Regular Walk State
            stateMachine.HandleStateChange(State.StateType.Walk);
        }

        if (player.IsGrounded() && isRunning)
        {
            // Running State
            stateMachine.HandleStateChange(State.StateType.Run);
        }
    }

    void OnJump(InputValue value) { stateMachine.HandleStateChange(State.StateType.Jump); }

    void OnAttack(InputValue value) { stateMachine.HandleStateChange(State.StateType.Attack); }

    void CheckIdle()
    {
        // Check if the player is idle.
        if (IsIdle())
        {
            idleTime += Time.deltaTime;

            // If the player has been idle for longer than the threshold, trigger the idle state.
            if (idleTime > idleTimeThreshold)
            {
                stateMachine.HandleStateChange(State.StateType.Idle);
                idleTime = 0;
            }
        }
        else { idleTime = 0; }
    }
}