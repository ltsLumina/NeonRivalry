#region
using UnityEngine;
#endregion

//TODO: If you hold move, then exit the state, upon entering the state you wont move until you release and repress the move button.
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

    public override bool CanBeInterrupted()
    {
        // return true if the player is doing anything other than moving
        return player.IsAttacking() || player.IsAirborne();
    }

    public override void OnEnter()
    {
        // Play the move animation.
        // Log("Entered Walk State");
        IsMoving = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
    }
    
    public override void UpdateState()
    {
        // Handle move logic
        Vector3 moveInput = player.InputManager.MoveInput;
        int moveDirection = (int)moveInput.x;

        float targetSpeed = moveDirection * moveSpeed;

        float speedDifference = targetSpeed - player.Rigidbody.velocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) * Mathf.Sign(speedDifference);

        player.Rigidbody.AddForce(movement * Vector3.right);

        OnExit();
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        
        //TODO: I don't think I like this. If you move from side to side quickly, you will be switch to idle for a frame and then back to walk, which just feels off.
        // if (player.InputManager.MoveInput.x == 0)
        // {
        //     player.StateMachine.TransitionToState(StateType.Idle);
        // }

        IsMoving = false;
    }
}