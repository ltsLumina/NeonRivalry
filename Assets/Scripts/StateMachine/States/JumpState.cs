#region
using UnityEngine;
#endregion

//TODO: Possibly make a fall state?
public class JumpState : State
{
    public override StateType Type => StateType.Jump;
    public override int Priority => statePriorities[Type];
    public override bool Interrupted { get; set; }
    public bool IsJumping { get; private set; }

    float jumpDuration = 0.2f;
    float jumpForce;
    float jumpTimer;

    public JumpState(PlayerController player, JumpStateData stateData) : base(player)
    {
        jumpForce = stateData.JumpForce;
    }

    public override void OnEnter()
    {
        // Initiate jump animation
        //Log("Entered Jump State");
        IsJumping = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
    }

    public override void UpdateState()
    {
        jumpTimer += Time.deltaTime;

        // Check if jump duration is not exceeded
        if (jumpTimer < jumpDuration)
        {
            // Handle jump logic, such as applying force, checking for jump height, etc.
            if (player.IsGrounded())
            {
                player.PlayerRB.AddForce(Vector2.up * jumpForce);
            }
        }
        else
        {
            // Jump duration exceeded, transition to another state
            OnExit();
        }
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Jump State");
        IsJumping = false;
    }

    public override void OnInterrupt()
    {
        Debug.Log("Jump interrupted!");

        //TODO: check what the current state is, and run specific interrupt code depending on the state.

    }
}