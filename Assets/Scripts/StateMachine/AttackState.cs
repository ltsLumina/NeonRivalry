using UnityEngine;
using static UnityEngine.Debug;

public class AttackState : StateMachine.IState
{
    float attackTimer;
    float attackDuration;

    public AttackState(float duration)
    {
        attackDuration = duration;
    }

    public PlayerController Player { get; }
    public Rigidbody2D PlayerRB { get; }
    public void OnEnter()
    {
        // Play the attack animation.
        Log("Attacked!");
    }

    public void UpdateState()
    {
        // attack, wait for attack to finish, then exit
        if (attackTimer >= attackDuration)
            OnExit();
        else
            attackTimer += Time.deltaTime;

        Log(attackTimer);

        // Handle attack logic, such as checking for attack duration, etc.
    }

    public void OnExit()
    {
        // Perform any necessary cleanup or exit actions
    }
}