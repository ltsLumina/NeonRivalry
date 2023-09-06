#region
using UnityEngine;
#endregion

public class AttackState : State
{
    // -- Abstract Variables --
    public override StateType Type => StateType.Attack;
    public override int Priority => statePriorities[Type];

    public bool IsAttacking { get; private set; }

    // -- State Specific Variables --
    // TODO: these are temporary
    private float attackTimer;
    private float attackDuration;

    // -- Constructor --
    public AttackState(PlayerController player, AttackStateData stateData) : base(player)
    {
        attackDuration = stateData.AttackDuration;
    }

    public override bool CanBeInterrupted()
    {
        // TODO: this will change later, when there are more states with higher priority.
        return interruptibilityRules[Type];
    }

    public override void OnEnter()
    {
        // Play the attack animation.
        // Debug.Log("Entered Attack State");
        IsAttacking = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.red;
    }

    public override void UpdateState()
    {
        if (player.IsGrounded())
        {
            // Perform a grounded attack if the player is on the ground.
            if (attackTimer < attackDuration)
            {
                attackTimer += Time.deltaTime;
                Debug.Log("Attacking on the ground!");

                // Play ground attack animation.
                // Exits the attack state once the animation finishes/the attack duration is over.
            }
            else
            {
                OnExit();
            }
        }
        else
        {
            // Perform an aerial attack if the player is in the air.
            if (attackTimer < attackDuration)
            {
                attackTimer += Time.deltaTime;
                Debug.Log("Attacking in the air!");

                // Play airborne attack animation.
                // Once the animation finishes/attackDuration is over, transition to the fall state.
                player.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0.21f, 0.38f, 0.75f); // Pink now refers to airborne attacks.
            }
            else
            {
                TransitionTo(StateType.Fall);
            }
        }
    }

    public override void OnExit() // Only called by the ground attack in this case, as the airborne attack transitions to the fall state.
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Attack State");

        TransitionTo(StateType.Idle);
        IsAttacking = false;
    }
}
