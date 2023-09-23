#region
using Lumina.Debugging;
using UnityEngine;
#endregion

public class AttackState : State
{
    // -- Abstract Variables --
    public override StateType Type => StateType.Attack;
    public override int Priority => statePriorities[Type];
    
    public bool IsAttacking { get; private set; }
    public bool IsAirborne { get; private set; }

    // -- State Specific Variables --
    float groundedAttackTimer;
    float groundedAttackDuration;
    float airborneAttackTimer;
    float airborneAttackDuration;

    // -- Constructor --
    public AttackState(PlayerController player, AttackStateData stateData) : base(player)
    {
        groundedAttackDuration = stateData.AttackDuration;
        airborneAttackDuration = stateData.AttackDuration; //TODO Swap out variables
    }

    public override bool CanBeInterrupted()
    {
        return interruptibilityRules[Type];
    }

    public override void OnEnter()
    {
        // Play the attack animation.
        IsAttacking = true;
        IsAirborne  = !player.IsGrounded(); // Check whether the player was in air when attack started.

        player.GetComponentInChildren<SpriteRenderer>().color = IsAirborne ? new (1f, 0.21f, 0.38f, 0.75f) : Color.red;

        groundedAttackTimer = 0;
        airborneAttackTimer = 0;
    }

    public override void UpdateState()
    {
        // Player is grounded, perform grounded attack.
        if (!IsAirborne)
        {
            // If the attack duration has not been reached, continue attacking.
            if (groundedAttackTimer < groundedAttackDuration)
            {
                groundedAttackTimer += Time.deltaTime;

                if (player.IsGrounded()) StateDebugger.Debug("Attacking on the ground!", StateType.Attack);

                // Play ground attack animation and logic.
                else IsAirborne = true;
            }
            else { OnExit(); }
        }
        else // Player is airborne, perform airborne attack.
        {
            if (airborneAttackTimer < airborneAttackDuration)
            {
                airborneAttackTimer += Time.deltaTime;

                // If the player lands, cancel the attack.
                if (player.IsGrounded())
                {
                    StateDebugger.Debug("Airborne Attack cancelled!", StateType.Attack);
                    OnExit();
                }
                else
                {
                    StateDebugger.Debug("Attacking in the air!", StateType.Attack);

                    // Play airborne attack animation and run logic.
                }
            }
            else { OnExit(); }
        }
    }

    public override void OnExit() 
    {
        //TransitionTo(player.InputManager.MoveInput.x != 0 ? StateType.Walk : StateType.Idle);
        player.StateMachine.TransitionToState(player.InputManager.MoveInput.x != 0 ? StateType.Walk : StateType.Idle);
        IsAttacking = false;
    }
}