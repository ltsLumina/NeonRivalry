#region
using UnityEngine;
#endregion

public class JumpState : StateMachine.IState
{
    float jumpDuration;
    float jumpForce;
    float jumpTimer;

    public JumpState(float force, float duration)
    {
        jumpForce    = force;
        jumpDuration = duration;
    }

    public void OnEnter()
    {
        // Apply jump force or initiate jump animation
        jumpTimer = 0f;
        Debug.Log("Jump enter!");
    }

    public void UpdateState()
    {
        // Handle jump logic, such as applying force, checking for jump height, etc.
        jumpTimer += Time.deltaTime;

        if (jumpTimer >= jumpDuration)
            // Exit the jump state when the jump duration is over
            OnExit();

        Debug.Log("Jump exit!");
    }

    public void OnExit() => Debug.Log("exited state");

    // Perform any necessary cleanup or exit actions
    // For example, reset jump-related variables or animations
}