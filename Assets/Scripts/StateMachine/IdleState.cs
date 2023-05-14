using UnityEngine;

public class IdleState : State
{

    public override void OnEnter()
    {
        // Play the idle animation.
    }

    public override void UpdateState()
    {
        // Handle idle logic, such as checking for input, etc.
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
    }
}