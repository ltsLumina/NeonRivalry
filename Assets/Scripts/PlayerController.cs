#region
using UnityEngine;
using static Essentials.Attributes;
#endregion

/// <summary>
/// THIS IS NOT FINAL, I AM TESTING. NONE OF THIS WORKS OR IS MEANT TO WORK.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;

    [Header("Read-Only Fields"), SerializeField, ReadOnly]
    Vector2 moveInput;

    // Cached References
    StateMachine stateMachine;
    InputManager input;
    MoveStateData moveStateData;

    public Rigidbody2D PlayerRB { get; private set; }
    public Vector2 MoveInput
    {
        get => moveInput;
        set => moveInput = value;
    }

    void Start()
    {
        stateMachine = FindObjectOfType<StateMachine>();
        stateMachine.SetState(new IdleState());
        PlayerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        // MoveState moveState = new MoveState(this, PlayerRB, moveSpeed);
        // stateMachine.SetState(moveState);
    }

    public void HandleStateChange(State newState)
    {
        switch (newState)
        {
            case IdleState idleState:
                stateMachine.SetState(new IdleState());
                break;

            case MoveState moveState:
                stateMachine.SetState(new MoveState(moveStateData));
                break;
        }
    }
}