#region
using UnityEngine;
#endregion

//TODO: If you hold move, then exit the state, upon entering the state you wont move until you release and repress the move button.
public class MoveState : State
{
    float moveSpeed;

    public override StateType Type => StateType.Walk;
    public override int Priority => statePriorities[Type];

    public bool IsMoving { get; private set; }

    public MoveState(PlayerController player, MoveStateData stateData) : base(player)
    {
        moveSpeed = stateData.MoveSpeed;
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
        // Handle move logic
        Vector3 moveInput = player.InputManager.MoveInput;

        if (moveInput.sqrMagnitude > 0.01)
        {
            // Apply horizontal and vertical movement
            Vector3 movement = moveInput * (moveSpeed * Time.deltaTime);
            player.Rigidbody.velocity = new (movement.x, player.Rigidbody.velocity.y, movement.z);
        }
        else { OnExit(); }
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        //Debug.Log("Exited Move State");

        // Slow down the player
        Vector3 velocity = player.Rigidbody.velocity;
        velocity                  = new (velocity.x * 0.9f, velocity.y, velocity.z);
        player.Rigidbody.velocity = velocity;

        // Pass control to the idle state
        
        // TODO: If we enter the Idle state whenever we stop moving, we'll gain a lot of speed when we start moving again as OnEnter is called.
        //player.StateMachine.TransitionToState(StateType.Idle);
        
        // Reset the IsMoving flag
        IsMoving = false;
    }
}