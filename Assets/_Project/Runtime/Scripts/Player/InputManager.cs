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
    PlayerController player;
    StateMachine stateMachine;
    PlayerInput playerInput;

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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && player.CanAttack())
        {
            stateMachine.TransitionToState(State.StateType.Attack);
        }
    }
}