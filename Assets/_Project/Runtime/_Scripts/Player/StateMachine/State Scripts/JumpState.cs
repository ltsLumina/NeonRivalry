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

    readonly float gravityScale;
    readonly float jumpHeight;
    readonly float globalGravity;
    bool hasJumped;

    public JumpState(PlayerController player, JumpStateData stateData) : base(player)
    {
        gravityScale = stateData.GravityScale;
        jumpHeight = stateData.JumpForce;
        globalGravity = stateData.GlobalGravity;
    }

    public override void OnEnter()
    {
        PlayerController.GlobalGravity = globalGravity * -1;
        IsJumping = true;
        hasJumped = false;
        player.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
    }

    public override void UpdateState()
    {
        //if(hasJumped)
        //{
        //    float spring = 1.2f;
        //    float drag = 0.9f;

        //    var Velocity = player.Rigidbody.velocity;
        //    Velocity.y += (jumpHeight -player.transform.position.y) * spring;
        //    Velocity.y -= drag * Velocity.y;
        //    player.Rigidbody.velocity = Velocity;

        //}

        if (player.Rigidbody.velocity.y > 0)
        {
            player.Rigidbody.velocity = new Vector3(player.Rigidbody.velocity.x, player.Rigidbody.velocity.y * 0.98f, player.Rigidbody.velocity.z);
        }

        if (!hasJumped)
        {
            player.gravityScale = gravityScale;
            float jumpForce = Mathf.Sqrt(jumpHeight * (Physics.gravity.y * player.gravityScale) * -2) * player.Rigidbody.mass;
          
            //float jumpForce = Mathf.Sqrt((-2 * jumpHeight) * Physics.gravity.y);
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
        IsJumping = false;
    }
}