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

    // -- State Specific Variables --
    float groundedAttackTimer;

    readonly AttackHandler attackHandler;
    readonly Animator animator;
    readonly Moveset moveset;

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

        player.GetComponentInChildren<SpriteRenderer>().color = Color.red;

        groundedAttackTimer = 0;

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
                FGDebugger.Debug("Player has been knocked into the air! \nCancelling the attack...", LogType.Log, StateType.Attack);

                // Cancel the attack.
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

        IsAttacking = false;
    }
}