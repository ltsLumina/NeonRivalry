#region
using UnityEngine;
#endregion

public class MoveState : State
{

    public bool IsMoving { get; private set; }

    public MoveState(PlayerController player, MoveStateData stateData) : base(player)
    {
        
    }

    public override void OnEnter()
    {
        // Play the move animation.
        IsMoving = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
    }

    public override void UpdateState()
    {
        // // Handle single player blocking
        // if (PlayerManager.PlayerControllerCount == 1)
        // {
        //     Vector2 moveInput = player.InputManager.MoveInput;
        //     return;
        // }
        //
        // if (IsBlocking()) return;
        //
        // return;
        //
        // bool IsBlocking()
        // {
        //     // Player must be grounded to block.
        //     if (!player.IsGrounded()) return false;
        //
        //     // Get the direction to the other player
        //     if (PlayerManager.OtherPlayer(player) == null) return false;
        //     Vector3 dirOtherPlayer = PlayerManager.OtherPlayer(player).transform.position - player.transform.position;
        //     dirOtherPlayer.Normalize();
        //
        //     // Get the direction of movement
        //     Vector2 moveInput     = player.InputManager.MoveInput;
        //     Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        //     moveDirection.Normalize();
        //
        //     // Calculate the dot product
        //     float dotProduct = Vector3.Dot(dirOtherPlayer, moveDirection);
        //
        //     // If the dot product is less than 0, the player is blocking
        //     return dotProduct < 0;
        // }
        
        // // Getting the move input from the player's input manager.
        // Vector3 moveInput = player.InputManager.MoveInput;
        //
        // // Determining the direction of the movement (left or right).
        // int moveDirection = (int) moveInput.x;
        //
        // // Calculating the target speed based on direction and move speed.
        // float targetSpeed = moveDirection * moveSpeed;
        //
        // // Calculate difference between target speed and current velocity.
        // float speedDifference = targetSpeed - player.Rigidbody.velocity.x;
        //
        // // Determine the acceleration rate based on whether the target speed is greater than 0.01 or not.
        // float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        //
        // // Calculate the final movement force to be applied on the player's rigidbody.
        // float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) * Mathf.Sign(speedDifference);
        //
        // // Apply the force to the player's rigidbody.
        // player.Rigidbody.AddForce(movement * Vector3.right);
        //
        // // Call the OnExit function after the force has been applied.
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions

        if (player.IsGrounded()) player.StateMachine.TransitionToState(player.InputManager.MoveInput.x != 0 ? StateType.Walk : StateType.Idle);

        IsMoving = false;
    }
}