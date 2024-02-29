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

    readonly float playerMass;
    readonly float jumpForce;
    readonly float jumpDuration;
    bool hasJumped;

    public JumpState(PlayerController player, JumpStateData stateData) : base(player)
    {
        playerMass = stateData.PlayerMass;
        jumpForce = stateData.JumpForce;
        jumpDuration = stateData.JumpDuration;
    }

    public override void OnEnter()
    {   
        IsJumping = true;
        hasJumped = false;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;

        player.Rigidbody.mass = playerMass;
    }

    public override void UpdateState()
    {
        if (!hasJumped)
        {
            player.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJumped = true;
        }
        // Player has reached the apex of their jump
        else if (player.Rigidbody.velocity.y < 0)
        {
            // pull them down
            player.StateMachine.TransitionToState(StateType.Fall);
            OnExit();
            return;
        }
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Jump State");
        player.Rigidbody.mass = 1f;
        IsJumping = false;
    }
}