#region
using Lumina.Debugging;
using UnityEngine;
#endregion

public class AirborneAttackState : State
{
    readonly Animator animator;

    float airborneAttackTimer;

    public AirborneAttackState(PlayerController player, AirborneAttackStateData stateData) : base(player)
    {
        animator = player.GetComponentInChildren<Animator>();
    }

    public bool IsAirborneAttacking { get; private set; }
    public bool IsAirborne { get; private set; }

    public override StateType Type => StateType.AirborneAttack;
    public override int Priority => statePriorities[Type];

    public override bool CanBeInterrupted() => false;

    public override void OnEnter()
    {
        IsAirborneAttacking = true;
        IsAirborne          = player.IsAirborne(); // Check whether the player was in air when attack started.

        player.GetComponentInChildren<SpriteRenderer>().color = new (1f, 0.21f, 0.38f, 0.75f);

        airborneAttackTimer = 0;
    }

    public override void UpdateState()
    {
        if (IsAirborne) // Player is airborne, perform airborne attack.
        {
            if (airborneAttackTimer < animator.GetCurrentAnimatorStateInfo(0).length)
            {
                airborneAttackTimer += Time.fixedDeltaTime;

                // If the player lands, cancel the attack.
                if (player.IsGrounded())
                {
                    FGDebugger.Debug("Airborne Attack cancelled!", LogType.Log, StateType.AirborneAttack);
                    OnExit();
                }
                else
                {
                    FGDebugger.Debug("Attacking in the air!", LogType.Log, StateType.AirborneAttack);

                    // Play airborne attack animation and run logic.
                }
            }
            else { OnExit(); }
        }
    }

    public override void OnExit()
    {
        // Cancel the attack animation by starting the idle animation.
        animator.Play("Idle");

        if (player.InputManager.MoveInput.x != 0 && player.IsGrounded())

            // If the player is moving, transition to the walk state.
            player.StateMachine.TransitionToState(StateType.Walk);

        else // If the player is not moving, transition to the idle state. 
            player.StateMachine.TransitionToState(StateType.Idle);

        IsAirborneAttacking = false;
    }
}
