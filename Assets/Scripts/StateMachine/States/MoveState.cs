#region
using UnityEngine;
#endregion

public class MoveState : State
{
    float moveSpeed;

    static StateType Type => StateType.Walk;
    public override int Priority => statePriorities[Type];

    public bool IsMoving { get; private set; }

    public MoveState(PlayerController player, MoveStateData stateData) : base(player)
    {
        moveSpeed = stateData.MoveSpeed;
    }

    public override bool CanBeInterrupted()
    {
        // return true if the player is doing anything other than moving
        return interruptibilityRules[Type];
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
        // Check for ground
        if (!player.IsGrounded()) return;

        // Handle move logic
        Vector2 moveInput = player.InputManager.MoveInput;

        if (moveInput.x != 0)
        {
            Vector2 movement = new Vector2(moveInput.x, 0) * (moveSpeed * Time.deltaTime);
            player.PlayerRB.velocity = movement;
        }
        else { OnExit(); }

    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Move State");
        IsMoving = false;
    }
}