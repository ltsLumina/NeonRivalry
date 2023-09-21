using UnityEngine;

/*----------------------------------------------------------------------------
 * IMPORTANT NOTE: Use Time.fixedDeltaTime instead of Time.deltaTime
 *----------------------------------------------------------------------------*/
public class FallState : State
{
    public override StateType Type => StateType.Fall;
    public override int Priority => statePriorities[Type];

    public bool IsFalling { get; private set; }

    // -- State Specific Variables --
    //TODO: these are temporary
    float playerMass;
    float fallGravityMultiplier;
    float jumpHaltForce;

    // -- Constructor --
    public FallState(PlayerController player, FallStateData stateData) : base(player)
    {
        playerMass = stateData.PlayerMass;
        fallGravityMultiplier = stateData.FallGravityMultiplier;
        jumpHaltForce = stateData.JumpHaltForce;
    }

    public override bool CanBeInterrupted()
    {
        // return true if the player is attacking or is grounded
        return player.IsAttacking() || player.IsGrounded() || player.IsJumping();
    }

    public override void OnEnter()
    {
        // Play the attack animation.
        //Debug.Log("Entered Attack State");
        IsFalling = true;
        player.PlayerRB.mass = playerMass;
        player.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0.58f, 0f);
    }

    public override void UpdateState()
    {
        bool hasAppliedFallForce = false;
        bool hasAppliedJumpHalt = false;
        //TODO: this is not final. This is just a placeholder.
        if (player.IsGrounded())
        {
            hasAppliedFallForce = false;
            hasAppliedJumpHalt = false;
            OnExit();
        }
        if (player.PlayerRB.velocity.y < 0 && !hasAppliedFallForce )
        {
            hasAppliedFallForce = true;
            player.PlayerRB.AddForce(fallGravityMultiplier * Vector3.down);
        }
        if (player.PlayerRB.velocity.y > 0 && !hasAppliedJumpHalt)
        {
            hasAppliedJumpHalt |= true;
            player.PlayerRB.AddForce(jumpHaltForce * Vector3.down);
        }
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Fall State");

        //TransitionTo(player.InputManager.MoveInput.x != 0 ? StateType.Walk : StateType.Idle);
        player.StateMachine.TransitionToState(player.InputManager.MoveInput.x != 0 ? StateType.Walk : StateType.Idle);
        
        // Play land animation.
        IsFalling = false;
    }
}