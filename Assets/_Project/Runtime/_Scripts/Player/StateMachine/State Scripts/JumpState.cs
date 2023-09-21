#region
using UnityEngine;
#endregion

/*----------------------------------------------------------------------------
 * IMPORTANT NOTE: Use Time.fixedDeltaTime instead of Time.deltaTime
 *----------------------------------------------------------------------------*/
public class JumpState : State
{
    public override StateType Type => StateType.Jump;
    public override int Priority => statePriorities[Type];

    public bool IsJumping { get; private set; }

    float jumpForce;

    float jumpTimer;
    float jumpDuration;

    public JumpState(PlayerController player, JumpStateData stateData) : base(player)
    {
        jumpForce = stateData.JumpForce;
        jumpDuration = stateData.JumpDuration;
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
            player.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else if (jumpTimer >= jumpDuration /* && player.Rigidbody.velocity.y < 0 */)
        {
            // Jump duration exceeded, transition to another state
            OnExit();

            // Transition to fall state
            //TransitionTo(StateType.Fall);
            player.StateMachine.TransitionToState(StateType.Fall);
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