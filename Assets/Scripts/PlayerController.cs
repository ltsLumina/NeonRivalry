#region
using UnityEngine;
using UnityEngine.InputSystem;
using static Essentials.Attributes;
#endregion

/// <summary>
/// THIS IS NOT FINAL, I AM TESTING. NONE OF THIS WORKS OR IS MEANT TO WORK.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;

    [Header("Read-Only Fields"), ReadOnly]
    [SerializeField] Vector2 moveInput;

    // Cached References
    StateMachine stateMachine;
    public Rigidbody2D PlayerRB { get; private set; }
    public Vector2 MoveInput
    {
        get => moveInput;
        private set => moveInput = value;
    }

    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.SetState(new IdleState());
        PlayerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        MoveState moveState = new MoveState(this, PlayerRB, moveSpeed);
        stateMachine.SetState(moveState);
    }

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
            stateMachine.SetState(new JumpState(jumpForce, 1f, this, PlayerRB));
    }

    void OnAttack(InputValue value)
    {
        if (value.isPressed)
            stateMachine.SetState(new AttackState(1f));
    }
}