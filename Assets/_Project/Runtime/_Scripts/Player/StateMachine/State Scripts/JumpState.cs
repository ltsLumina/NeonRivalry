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

    readonly float jumpForce;
    readonly float jumpDuration;
    float jumpTimer;

    public JumpState(PlayerController player, JumpStateData stateData) : base(player)
    {
        float playerMass = stateData.PlayerMass;
        player.Rigidbody.mass = playerMass;
        
        jumpForce = stateData.JumpForce;
        jumpDuration = stateData.JumpDuration;
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
        if (jumpTimer < jumpDuration)
        {
            // Apply jump force
            player.Rigidbody.AddForce(Vector3.up * (jumpForce / jumpDuration), ForceMode.Force);
        }
        // Player has reached the apex of their jump
        else
        {
            // Exit current state and transition to fall state
            player.StateMachine.TransitionToState(StateType.Fall);
            OnExit();
        }

        jumpTimer += Time.fixedDeltaTime;
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Jump State");
        
        IsJumping = false;
    }
}