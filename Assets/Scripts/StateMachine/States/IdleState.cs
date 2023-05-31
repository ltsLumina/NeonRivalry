using UnityEngine;

public class IdleState : State
{
    public override StateType Type => StateType.Idle;
    public override int Priority => statePriorities[Type];

    public IdleState(PlayerController player) : base(player)
    { }

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