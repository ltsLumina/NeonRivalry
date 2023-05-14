using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerController player;
    StateMachine.CharacterState characterState;
    StateMachine stateMachine;


    [Header("State Data")]
    [SerializeField] JumpStateData jumpStateData;
    [SerializeField] MoveStateData moveStateData;

    void Start()
    {
        player       = FindObjectOfType<PlayerController>();
        stateMachine = FindObjectOfType<StateMachine>();
    }

    void Update()
    {
        stateMachine.CurrentState.UpdateState();
    }

    public void OnMove(InputValue value)
    {
        player.MoveInput = value.Get<Vector2>();
        player.HandleStateChange(new MoveState(moveStateData));
    }

    void OnJump(InputValue value)
    {
        Debug.Log("Jumped!");
        player.HandleStateChange(new JumpState(jumpStateData));
    }

    void OnAttack(InputValue value)
    {
        Debug.Log("Attacked!");
    }
}