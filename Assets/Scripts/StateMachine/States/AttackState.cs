using System;
using UnityEngine;

public class AttackState : State
{
    // -- Abstract Variables --
    static StateType Type => StateType.Attack;
    public override int Priority => statePriorities[Type];

    public bool IsAttacking { get; private set; }

    // -- State Specific Variables --
    //TODO: these are temporary
    float attackTimer;
    float attackDuration;

    // -- Constructor --
    public AttackState(PlayerController player, AttackStateData stateData) : base(player)
    {
        attackTimer = stateData.AttackTimer;
        attackDuration = stateData.AttackDuration;
    }

    public override bool CanBeInterrupted() => interruptibilityRules[Type]; //TODO: this will change later, when there are more states with higher priority.

    public override void OnEnter( )
    {
        // Play the attack animation.
        //Debug.Log("Entered Attack State");
        IsAttacking = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.red;
    }

    public override void UpdateState()
    {
        //TODO: Currently the ground and air attacks are the same, but this will change later.
        if (player.IsGrounded() && !player.IsFalling())
        { // If the player is on the ground, perform a grounded attack.
            // attack, wait for attack to finish, then exit
            if (attackTimer >= attackDuration)
            {
                OnExit();
            }
            else
            {
                attackTimer += Time.deltaTime;
                Debug.Log("Attacking on the ground!");
            }
        }
        else
        { // If the player is in the air, perform an aerial attack.
            // attack, wait for attack to finish, then exit
            if (attackTimer >= attackDuration)
            {
                OnExit();
            }
            else
            {
                attackTimer += Time.deltaTime;
                Debug.Log("Attacking in the air!");
            }
        }
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        //Debug.Log("Exited Attack State");
        IsAttacking = false;

        player.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0.21f, 0.38f, 0.75f);
    }
}