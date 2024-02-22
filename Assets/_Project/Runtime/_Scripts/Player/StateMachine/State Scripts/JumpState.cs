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
    float jumpTimer;

    public JumpState(PlayerController player, JumpStateData stateData) : base(player)
    {
        playerMass = stateData.PlayerMass;
        jumpForce = stateData.JumpForce;
        jumpDuration = stateData.JumpDuration;
    }

    public override void OnEnter()
    {
        IsJumping = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;

        player.Rigidbody.mass = playerMass;
    }

    public override void UpdateState()
    {
        if (jumpTimer < jumpDuration)
        {
            // Alternative: (that i prefer)
            float pow   = 1 - Mathf.Pow(2 * jumpTimer / jumpDuration - 1, 2);
            float force = jumpForce              * pow;
            player.Rigidbody.AddForce(Vector3.up * force, ForceMode.Force);
            
            //player.Rigidbody.AddForce(Vector3.up * (jumpForce / jumpDuration), ForceMode.Force);
        }
        // Player has reached the apex of their jump
        else
        {
            // pull them down
            player.StateMachine.TransitionToState(StateType.Fall);
            OnExit();
            return;
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