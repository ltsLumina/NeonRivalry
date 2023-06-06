#region
using UnityEngine;
#endregion

public class JumpState : State
{
    static StateType Type => StateType.Jump;
    public override int Priority => statePriorities[Type];

    public bool IsJumping { get; private set; }

    float jumpForce;

    float jumpTimer;
    float jumpDuration = 0.75f;
    float preJumpDuration = 0.2f;
    float variableJumpHeightFactor = 0.5f;

    public JumpState(PlayerController player, JumpStateData stateData) : base(player)
    {
        jumpForce = stateData.JumpForce;
    }

    public override bool CanBeInterrupted()
    {
        // return true if the player is attacking or is grounded
        Debug.Log("JumpState: CanBeInterrupted()");
        return interruptibilityRules[Type];
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
        if (jumpTimer < preJumpDuration)
        {
            // Handle pre-jump logic, such as preparing for the jump animation or charging up the jump
            // This phase can be used for anticipation or other pre-jump actions

            // Example: Charging up the jump
            if (player.InputManager.JumpInputIsHeld())
            {
                // Charge up the jump force or perform any other actions
                // You can store the charged jump force in a variable and use it in the jump phase below
                // Example: chargedJumpForce = CalculateChargedJumpForce();
            }
        }
        else if (jumpTimer < jumpDuration)
        {
            // Handle jump logic, such as applying jump force, controlling jump height, etc.

            // Example: Applying jump force
            if (player.IsGrounded())
            {
                // Apply the jump force
                player.PlayerRB.AddForce(Vector2.up * (jumpForce * Time.deltaTime), ForceMode2D.Impulse);
            }
            else if (!player.InputManager.JumpInputIsHeld())
            {
                // Allow for variable jump height by reducing the upward velocity when the jump button is released
                // Example: Reduce the upward velocity by a certain factor
                player.PlayerRB.velocity = new (player.PlayerRB.velocity.x, player.PlayerRB.velocity.y * variableJumpHeightFactor);
            }
        }
        else
        {
            // Jump duration exceeded, transition to another state
            OnExit();
        }

        jumpTimer += Time.deltaTime;
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Jump State");
        
        IsJumping = false;
        
        // transition to fall state
        StateMachine.Instance.TransitionToState(StateType.Fall);
    }
}