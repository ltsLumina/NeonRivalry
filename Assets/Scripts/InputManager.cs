using UnityEngine;
using UnityEngine.InputSystem;
using static Essentials.Attributes;

public class InputManager : MonoBehaviour
{
    [Header("Read-Only Fields"), SerializeField, ReadOnly]
    Vector2 moveInput;

    PlayerController player;
    StateMachine stateMachine;

    public Vector2 MoveInput
    {
        get => moveInput;
        private set => moveInput = value;
    }

    void Start()
    {
        player       = FindObjectOfType<PlayerController>();
        stateMachine = FindObjectOfType<StateMachine>();
    }

    void Update() => stateMachine.CurrentState.UpdateState(stateMachine);

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
        stateMachine.HandleStateChange(State.StateType.Walk);
    }

    void OnJump(InputValue value)
    {
        stateMachine.HandleStateChange(State.StateType.Jump);
    }

    void OnAttack(InputValue value)
    {
        stateMachine.HandleStateChange(State.StateType.Attack);
    }
}