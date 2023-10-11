#region
using Lumina.Debugging;
using UnityEngine;
#endregion

/*----------------------------------------------------------------------------
 * IMPORTANT NOTE: Use Time.fixedDeltaTime instead of Time.deltaTime
 *----------------------------------------------------------------------------*/

public class AttackState : State
{
    // -- Abstract Variables --
    public override StateType Type => StateType.Attack;
    public override int Priority => statePriorities[Type];
    
    public bool IsAttacking { get; private set; }
    public bool IsAirborne { get; private set; }

    // -- State Specific Variables --
    float groundedAttackTimer;
    float airborneAttackTimer;

    AttackHandler attackHandler;
    Animator animator;
    Moveset moveset;

      // -- Constructor --
      public AttackState(PlayerController player, AttackStateData stateData) : base(player)
      {
          animator = player.GetComponentInChildren<Animator>();
          moveset  = stateData.Moveset;

          Debug.Assert(moveset != null, "Moveset is null in the AttackStateData. Please assign it in the inspector.");

          attackHandler = new (moveset, animator, player);
      }

    public override bool CanBeInterrupted() => interruptibilityRules[Type];

    public override void OnEnter()
    {
        // Play the attack animation.
        IsAttacking = true;
        IsAirborne  = player.IsAirborne(); // Check whether the player was in air when attack started.

        player.GetComponentInChildren<SpriteRenderer>().color = IsAirborne ? new (1f, 0.21f, 0.38f, 0.75f) : Color.red;

        groundedAttackTimer = 0;
        airborneAttackTimer = 0;

        var attackType = player.InputManager.LastAttackPressed;

        if (attackType != InputManager.AttackType.None)
        {
            attackHandler.SelectAttack(attackType);
            player.InputManager.LastAttackPressed = InputManager.AttackType.None; // Reset after usage
        }
        else
        {
            Debug.LogWarning("No attack type was selected. Something went wrong.");
        }
    }

    public override void UpdateState()
    {
        // Player is grounded, perform grounded attack.
        if (!IsAirborne)
        {
            // If the attack duration has not been reached, continue attacking.
            if (groundedAttackTimer < animator.GetCurrentAnimatorStateInfo(0).length)
            {
                groundedAttackTimer += Time.fixedDeltaTime;

                if (player.IsGrounded())
                {
                    FGDebugger.Debug("Attacking on the ground!", LogType.Log, StateType.Attack);
                    
                }
                // Player has become airborne after starting the attack, cancel the attack. (e.g. player gets knocked into the air) 
                else
                {
                    IsAirborne = true;
                    FGDebugger.Debug("Player has been knocked into the air! \nCancelling the attack...", LogType.Log, StateType.Attack);
                    
                    // Cancel the attack.
                    OnExit();
                }
            }
            else { OnExit(); }
        }
        else // Player is airborne, perform airborne attack.
        {
            if (airborneAttackTimer < animator.GetCurrentAnimatorStateInfo(0).length)
            {
                airborneAttackTimer += Time.fixedDeltaTime;

                // If the player lands, cancel the attack.
                if (player.IsGrounded())
                {
                    FGDebugger.Debug("Airborne Attack cancelled!", LogType.Log, StateType.Attack);
                    OnExit();
                }
                else
                {
                    FGDebugger.Debug("Attacking in the air!", LogType.Log, StateType.Attack);

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

        IsAttacking = false;
    }
}