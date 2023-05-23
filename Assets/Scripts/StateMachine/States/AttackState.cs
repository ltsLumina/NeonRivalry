using UnityEngine;
using static UnityEngine.Debug;

public class AttackState : State
{
    float attackTimer;
    float attackDuration;

    public AttackState(AttackStateData stateData)
    {
        attackTimer = stateData.AttackTimer;
        attackDuration = stateData.AttackDuration;
    }

    public override void OnEnter(StateMachine stateMachine)
    {
        // Play the attack animation.
        Log("Entered Attack State");

        stateMachine.Player.GetComponentInChildren<SpriteRenderer>().color = Color.red;
    }

    public override void UpdateState(StateMachine stateMachine)
    {
        // attack, wait for attack to finish, then exit
        if (attackTimer >= attackDuration)
            OnExit(stateMachine);
        else
            attackTimer += Time.deltaTime;

        // Handle attack logic
    }

    public override void OnExit(StateMachine stateMachine)
    {
        // Perform any necessary cleanup or exit actions
        stateMachine.EnterIdleState();
    }
}