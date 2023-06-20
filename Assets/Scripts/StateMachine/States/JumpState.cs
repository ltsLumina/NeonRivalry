#region
using UnityEngine;
using static UnityEngine.Debug;
#endregion

public class JumpState : State
{
    static StateType Type => StateType.Jump;
    public override int Priority => statePriorities[Type];

    public bool IsJumping { get; private set; }

    float jumpForce;

    float jumpTimer;
    float jumpDuration = 0.5f;

    public JumpState(PlayerController player, JumpStateData stateData) : base(player)
    {
        jumpForce = stateData.JumpForce;
    }

    public override bool CanBeInterrupted()
    {
        // return true if the player is attacking or is grounded
        //Log("JumpState: CanBeInterrupted()");
        return player.IsAttacking() || player.IsGrounded() || player.IsFalling();
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
        // Handle jump logic, such as applying jump force

        if (jumpTimer < jumpDuration)
        {
            // Apply the jump force
            player.PlayerRB.AddForce(Vector2.up * (jumpForce * Time.fixedDeltaTime), ForceMode2D.Impulse);
        }
        else if (jumpTimer >= jumpDuration && player.PlayerRB.velocity.y < 0)
        {
            // Jump duration exceeded, transition to another state
            OnExit();

            // Transition to fall state
            //StateMachine.Instance.TransitionToState(StateType.Fall);
        }

        jumpTimer += Time.deltaTime;
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Jump State");
        
        IsJumping = false;
    }
}