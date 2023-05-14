using UnityEngine;
using static UnityEngine.Debug;

public class AttackState : State
{
    float attackTimer;
    float attackDuration;

    public AttackState(float duration)
    {
        attackDuration = duration;
    }

    public override void OnEnter()
    {
        // Play the attack animation.
        Log("Attacked!");
    }

    public override void UpdateState()
    {
        // attack, wait for attack to finish, then exit
        if (attackTimer >= attackDuration)
            OnExit();
        else
            attackTimer += Time.deltaTime;

        // Handle attack logic, such as checking for attack duration, etc.
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
    }
}