using UnityEngine;

public class AttackState : State
{
    // -- Abstract Variables --
    public override StateType Type => StateType.Attack;
    public override int Priority => statePriorities[Type];
    public override bool Interrupted { get; set; }

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

    public override void OnEnter( )
    {
        // Play the attack animation.
        //Debug.Log("Entered Attack State");
        IsAttacking = true;

        player.GetComponentInChildren<SpriteRenderer>().color = Color.red;
    }

    public override void UpdateState()
    {
        // attack, wait for attack to finish, then exit
        if (attackTimer >= attackDuration)
            OnExit();
        else
            attackTimer += Time.deltaTime;

        // Handle attack logic
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        //Debug.Log("Exited Attack State");
        IsAttacking = false;

        player.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0.21f, 0.38f, 0.75f);
    }

    public override void OnInterrupt()
    {
        // Debug.Log("Attack interrupted!");
    }
}