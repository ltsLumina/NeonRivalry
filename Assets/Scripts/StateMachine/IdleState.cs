using UnityEngine;

public class IdleState : StateMachine.IState
{
    public PlayerController Player { get; }
    public Rigidbody2D PlayerRB { get; }
    public void OnEnter()
    {
        // Play the idle animation.
    }

    public void UpdateState()
    {
        // Handle idle logic, such as checking for input, etc.
    }

    public void OnExit()
    {
        // Perform any necessary cleanup or exit actions
    }
}