#region
using UnityEngine;
#endregion

public class MoveState : State
{
    float moveSpeed;
    float acceleration;
    float deceleration;
    float velocityPower;

    public override StateType Type => StateType.Walk;
    public override int Priority => statePriorities[Type];

    public bool IsMoving { get; private set; }

    public MoveState(PlayerController player, MoveStateData stateData) : base(player)
    {
        moveSpeed = stateData.MoveSpeed;
        acceleration = stateData.Acceleration;
        deceleration = stateData.Deceleration;
        velocityPower = stateData.AccelerationRate;
    }

    public override void OnEnter()
    {
        // Play the move animation.
        IsMoving = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
    }

    public override void UpdateState()
    {
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
        OnExit();
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions

        if (player.IsGrounded()) player.StateMachine.TransitionToState(player.InputManager.MoveInput.x != 0 ? StateType.Walk : StateType.Idle);

        IsMoving = false;
    }
}