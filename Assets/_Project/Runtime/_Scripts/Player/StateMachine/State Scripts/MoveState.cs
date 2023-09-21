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

    //MOVEMENT ISN'T MY PROBLEM HAHAHAHAHA HAVE FUN WITH THIS HEHEHE
    public override void UpdateState()
    {
        Debug.Log("Update move state was called");
        // Handle move logic
        Vector3 moveInput = player.InputManager.MoveInput;
        int moveDirection = (int)moveInput.x;

        float targetSpeed = moveDirection * moveSpeed;

        float speedDifference = targetSpeed - player.PlayerRB.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) * Mathf.Sign(speedDifference);

        player.PlayerRB.AddForce(movement * Vector3.right);

        OnExit();
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        //Debug.Log("Exited Move State");

        // Slow down the player
        // Vector3 velocity = player.PlayerRB.velocity;
        // velocity = new (velocity.x * 0.9f, velocity.y, velocity.z);
        // player.PlayerRB.velocity = velocity;

        // Pass control to the idle state
        
        // TODO: If we enter the Idle state whenever we stop moving, we'll gain a lot of speed when we start moving again as OnEnter is called.
        //player.StateMachine.TransitionToState(StateType.Idle);
        
        // Reset the IsMoving flag
        IsMoving = false;
    }
}