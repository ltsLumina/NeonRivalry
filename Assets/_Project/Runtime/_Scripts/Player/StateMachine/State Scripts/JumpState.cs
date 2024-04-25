#region
using UnityEngine;
#endregion

/*----------------------------------------------------------------------------
 * IMPORTANT NOTE: Use Time.fixedDeltaTime instead of Time.deltaTime
 *----------------------------------------------------------------------------*/
public class JumpState : State
{
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
        player.GlobalGravity = globalGravity;
        IsJumping = true;
        player.Animator.SetTrigger("HasJumped");

        hasJumped = false;
        player.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
    }

    public override void UpdateState()
    {
        // Player cannot block while jumping or falling.

        if (player.Rigidbody.velocity.y > 0)
        {
            player.Rigidbody.velocity = new (player.Rigidbody.velocity.x, player.Rigidbody.velocity.y * 0.98f, player.Rigidbody.velocity.z);
        }

        if (!hasJumped)
        {
            player.GravityScale = gravityScale;
            float jumpForce = Mathf.Sqrt(jumpHeight * (player.GlobalGravity * player.GravityScale) * -2) * player.Rigidbody.mass;

            //float jumpForce = Mathf.Sqrt((-2 * jumpHeight) * Physics.gravity.y);
            player.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);



            hasJumped = true;
        }
        //Player has reached the apex of their jump
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