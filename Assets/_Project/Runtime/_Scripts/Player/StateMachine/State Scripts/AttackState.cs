﻿#region
using UnityEngine;
using Logger = Lumina.Debugging.Logger;
#endregion

/*----------------------------------------------------------------------------
 * IMPORTANT NOTE: Use Time.fixedDeltaTime instead of Time.deltaTime
 *----------------------------------------------------------------------------*/

/// <summary>
/// <seealso cref="AttackStateData"/>
/// </summary>
public class AttackState : State
{
    public bool IsAttacking { get; private set; }

    // -- State Specific Variables --
    float groundedAttackTimer;

    readonly AttackHandler attackHandler;
    readonly Animator animator;

    float attackAnimationLength;

      // -- Constructor --
      public AttackState(PlayerController player, AttackStateData stateData) : base(player)
      {
          animator = player.Animator;
          Moveset moveset = player.Character.Moveset;

          Debug.Assert(moveset != null, "Moveset is null in the Character scriptable object. Please assign it in the inspector.");

          attackHandler = new (moveset, animator, player);
      }

    public override void OnEnter()
    {
        // Play the attack animation.
        IsAttacking = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.red;

        groundedAttackTimer = 0;
    }

    public override void UpdateState()
    {
        InputManager.AttackType attackType = player.InputManager.LastAttackPressed;

        if (attackType != InputManager.AttackType.None)
        {
            if (attackHandler.SelectAttack(attackType)) player.InputManager.LastAttackPressed = InputManager.AttackType.None; // Reset after usage
            else OnExit();
        }
        
        // Have to set the attack animation length here because the animator will return the length of the idle animation instead of the attack animation.
        // So we have to set the attack animation length every frame to ensure that it is the correct value.
        attackAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        
        // Bug: On occasion, the animator will return the length of the idle animation instead of the attack animation.
        if (attackAnimationLength > 1.8f) return; // temporary fix when the attack animation length is incorrect.
        
        // If the attack duration has not been reached, continue attacking.
        groundedAttackTimer += Time.deltaTime;

        if (player.IsGrounded()) Logger.Debug("Attacking on the ground!", LogType.Log, StateType.Attack);

        // i figured it out.
        // attackAnimationLength is the length of the IDLE ANIMATION, not the attack animation. :'(
        if (groundedAttackTimer >= attackAnimationLength)
        {
            OnExit();
        }
    }

    public override void OnExit()
    {
        // Cancel the attack animation by playing the idle animation.
        //animator.Play("Idle");

        if (player.IsGrounded() && player.InputManager.MoveInput.x != 0)
            // If the player is moving, transition to the walk state.
            player.StateMachine.TransitionToState(StateType.Walk);
        // If the player is not moving, transition to the idle state.
        else player.StateMachine.TransitionToState(StateType.Idle);

        IsAttacking = false;
    }
}