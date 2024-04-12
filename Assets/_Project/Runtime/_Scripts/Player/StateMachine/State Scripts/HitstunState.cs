using UnityEngine;
using Logger = Lumina.Debugging.Logger;

public class HitstunState : State
{
    public override StateType Type => StateType.None;
    public override int Priority => statePriorities[Type];

    public HitstunState(PlayerController player) : base(player)
    {
        animator       = player.Animator;
        timeHitStunned = 0;
    }

    Animator animator;
    float timeHitStunned;

    public override void OnEnter()
    {
        Logger.Info("'HitStun' state selected.");
        player.GetComponentInChildren<SpriteRenderer>().color = Color.magenta;
    }

    public override void UpdateState()
    {
        // wait the hitstun duration
        
        if (timeHitStunned < animator.GetCurrentAnimatorStateInfo(0).length) timeHitStunned += Time.deltaTime;
        else OnExit();
    }

    public override void OnExit()
    {
        if (player.IsGrounded() && player.InputManager.MoveInput.x != 0) player.StateMachine.TransitionToState(StateType.Walk);
        else if (player.IsGrounded()) player.StateMachine.TransitionToState(StateType.Idle);
    }
}