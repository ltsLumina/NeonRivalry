using UnityEngine;
using Logger = Lumina.Debugging.Logger;

public class NoneState : State
{
    public NoneState(PlayerController player) : base(player)
    { }

    public override void OnEnter()
    {
        Logger.Info("'None' state selected. This state is used when there is no state to transition to.");
        player.GetComponentInChildren<SpriteRenderer>().color = Color.grey;
    }

    public override void UpdateState()
    {
        // This should prevent the player from getting soft-locked in the 'None' state.
        OnExit();
    }

    public override void OnExit()
    {
        if (player.IsGrounded() && player.InputManager.MoveInput.x != 0) player.StateMachine.TransitionToState(StateType.Walk);
        else if (player.IsGrounded()) player.StateMachine.TransitionToState(StateType.Idle);
    }
}