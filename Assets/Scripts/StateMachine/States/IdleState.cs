using UnityEngine;

public class IdleState : State
{
    static StateType Type => StateType.Idle;
    public override int Priority => statePriorities[Type];

    public IdleState(PlayerController player) : base(player)
    { /*Empty Constructor as the Idle state doesn't require anything. (Yet) */ }

    public override bool CanBeInterrupted()
    {
        // return true if the player is not idle
        return interruptibilityRules[Type]; //TODO: this might change later, if necessary.
    }

    public override void OnEnter()
    {
        // Play the idle animation.
        player.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    public override void UpdateState()
    {
        // Handle idle logic, such as checking for input, etc.
        if (player.InputManager.MoveInput.x != 0)
        {
            OnExit();
        }
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Idle State");
    }
}