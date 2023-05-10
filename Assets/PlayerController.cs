#region
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;

    Vector2 moveInput;

    // Cached References
    StateMachine stateMachine;
    public Rigidbody2D PlayerRB { get; private set; }

    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new IdleState());
        PlayerRB = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() => Move();

    // --- Movement ---

    void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    void Move() => PlayerRB.MovePosition(PlayerRB.position + moveInput * (moveSpeed * Time.fixedDeltaTime));

    // --- Jumping ---

    void OnJump(InputValue value) => stateMachine.ChangeState(new JumpState(10, 1));

    // --- Attacking ---

    void OnAttack()
    { // Change to attack state
        stateMachine.ChangeState(new AttackState());

        // Update the state machine
        stateMachine.Update();
    }
}