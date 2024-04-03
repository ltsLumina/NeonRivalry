using UnityEngine;
using Logger = Lumina.Debugging.Logger;

public class NoneState : State
{
    public override StateType Type => StateType.None;
    public override int Priority => statePriorities[Type];

    public NoneState(PlayerController player) : base(player)
    { }

    public override void OnEnter()
    {
        Logger.Info("'None' state selected. This state is used when there is no state to transition to.");
        player.GetComponentInChildren<SpriteRenderer>().color = Color.grey;
    }

    public override void UpdateState()
    {
        
    }

    public override void OnExit()
    {
        if (player.IsGrounded() && player.InputManager.MoveInput.x != 0) player.StateMachine.TransitionToState(StateType.Walk);
        else if (player.IsGrounded()) player.StateMachine.TransitionToState(StateType.Idle);
    }
}