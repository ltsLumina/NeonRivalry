using UnityEngine;

public class IdleState : State
{
    public IdleState(PlayerController player) : base(player)
    { /*Empty Constructor as the Idle state doesn't require anything. (Yet) */ }

    public override void OnEnter()
    {
        // Play the idle animation.
        player.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    public override void UpdateState()
    {
        // Handle idle logic, such as checking for input, etc.
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Idle State");
    }
}