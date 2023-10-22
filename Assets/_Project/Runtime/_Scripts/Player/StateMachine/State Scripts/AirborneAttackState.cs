#region
using Lumina.Debugging;
using UnityEngine;
#endregion

public class AirborneAttackState : State
{
    readonly Animator animator;
    readonly AttackHandler attackHandler;
    readonly Moveset moveset;

    float airborneAttackTimer;

    public AirborneAttackState(PlayerController player, AirborneAttackStateData stateData) : base(player)
    {
        animator = player.GetComponentInChildren<Animator>();
        moveset = stateData.Moveset;
        
        Debug.Assert(moveset != null, "Moveset is null in the AttackStateData. Please assign it in the inspector.");

        // Create a new instance of the attack handler.
        attackHandler = new (moveset, animator, player);
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

        // Select the attack type.
        InputManager.AttackType attackType = player.InputManager.LastAttackPressed;

        // If the attack type is not None, select the attack.
        if (attackType == InputManager.AttackType.Airborne)
        {
            attackHandler.SelectAttack(attackType);
            player.InputManager.LastAttackPressed = InputManager.AttackType.None; // Reset after usage
        }
        else { Debug.LogError("No \"(None)\" attack type was selected. Something went wrong."); }
    }

    public override void UpdateState()
    {
        if (!IsAirborne) return; // If the player is not airborne, cancel the attack.

        // If the attack animation is still playing, run logic.
        if (airborneAttackTimer < animator.GetCurrentAnimatorStateInfo(0).length)
        {
            airborneAttackTimer += Time.fixedDeltaTime;
            
            // Perform the airborne attack logic.
            if (!player.IsGrounded())
            {
                FGDebugger.Debug("Attacking in the air!", LogType.Log, StateType.AirborneAttack);
                

                // Play airborne attack animation and run logic.
            }
            // If the player lands, cancel the attack.
            else
            {
                FGDebugger.Debug("Airborne Attack cancelled!", LogType.Log, StateType.AirborneAttack);
                OnExit();
            }
        }
        else { OnExit(); }
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
