#region
using UnityEngine;
#endregion

public class JumpState : StateMachine.IState
{
    float jumpDuration;
    float jumpForce;
    float jumpTimer;

    public JumpState(float force, float duration, PlayerController player, Rigidbody2D playerRB)
    {
        jumpForce    = force;
        jumpDuration = duration;

        Player       = player;
        PlayerRB     = playerRB;
    }

    public PlayerController Player { get; }
    public Rigidbody2D PlayerRB { get; }
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

        PlayerRB.AddForce(Vector2.up * jumpForce);

        if (jumpTimer >= jumpDuration)
            // Exit the jump state when the jump duration is over
            OnExit();
    }

    public void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // For example, reset jump-related variables or animations
    }
}