#region
using UnityEngine;
#endregion

public class JumpState : State
{
    float jumpDuration;
    float jumpForce;
    float jumpTimer;

    public JumpState(JumpStateData data)
    {
        Player    = data.Player;
        PlayerRB  = data.Player.GetComponent<Rigidbody2D>();
        jumpForce = data.jumpForce;
        jumpDuration = data.jumpDuration;
    }

    public PlayerController Player { get; }
    public Rigidbody2D PlayerRB { get; }
    public override void OnEnter()
    {
        // Apply jump force or initiate jump animation
        jumpTimer = 0f;
        Debug.Log("Jump enter!");
    }

    public override void UpdateState()
    {
        // Handle jump logic, such as applying force, checking for jump height, etc.
        jumpTimer += Time.deltaTime;

        PlayerRB.AddForce(Vector2.up * jumpForce);

        if (jumpTimer >= jumpDuration)
            // Exit the jump state when the jump duration is over
            OnExit();
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // For example, reset jump-related variables or animations
    }
}