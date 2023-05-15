#region
using UnityEngine;
#endregion

public class MoveState : State
{
    float moveSpeed;

    public MoveState(MoveStateData stateData) => moveSpeed = stateData.MoveSpeed;

    public override void OnEnter(StateMachine stateMachine)
    {
        // Play the move animation.
        Debug.Log("Entered Walk State");

        stateMachine.Player.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
    }

    //MOVEMENT ISN'T MY PROBLEM HAHAHAHAHA HAVE FUN WITH THIS HEHEHE
    public override void UpdateState(StateMachine stateMachine)
    {
        // Check for ground
        if (!stateMachine.Player.IsGrounded()) return;

        // Handle move logic
        Vector2 moveInput = stateMachine.PlayerInput.MoveInput;

        if (moveInput.x != 0)
        {
            Vector2 movement = new Vector2(moveInput.x, 0) * (moveSpeed * Time.fixedDeltaTime);
            stateMachine.PlayerRB.velocity = movement;
        }
        else { OnExit(); }
    }

    public override void OnExit(StateMachine stateMachine = null)
    {
        // Perform any necessary cleanup or exit actions
    }
}