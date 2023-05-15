using UnityEngine;

public class IdleState : State
{

    public override void OnEnter(StateMachine stateMachine)
    {
        // Play the idle animation.
        stateMachine.Player.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    public override void UpdateState(StateMachine stateMachine)
    {
        // Handle idle logic, such as checking for input, etc.
    }

    public override void OnExit(StateMachine stateMachine)
    {
        // Perform any necessary cleanup or exit actions
    }
}