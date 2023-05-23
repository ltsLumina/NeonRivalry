#region
using UnityEngine;
using static UnityEngine.Debug;
#endregion

public class JumpState : State
{
    float jumpDuration = 0.5f;
    float jumpForce;
    float jumpTimer;

    public JumpState(JumpStateData stateData)
    {
        jumpForce = stateData.JumpForce;
    }

    public override void OnEnter(StateMachine stateMachine)
    {
        // Initiate jump animation
        Log("Entered Jump State");

        stateMachine.Player.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
    }

    public override void UpdateState(StateMachine stateMachine)
    {
        jumpTimer += Time.deltaTime;

        // Check if jump duration is not exceeded
        if (jumpTimer < jumpDuration)
        {
            // Handle jump logic, such as applying force, checking for jump height, etc.
            if (stateMachine.Player.IsGrounded())
            {
                stateMachine.PlayerRB.AddForce(Vector2.up * jumpForce);
            }
        }
        else
        {
            // Jump duration exceeded, transition to another state
            OnExit(stateMachine);
        }
    }

    public override void OnExit(StateMachine stateMachine)
    {
        stateMachine.EnterIdleState();
        // Perform any necessary cleanup or exit actions
        // For example, reset jump-related variables or animations
    }
}