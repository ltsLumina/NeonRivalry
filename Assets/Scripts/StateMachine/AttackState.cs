using UnityEngine;

public class AttackState : StateMachine.IState
{
    public void OnEnter()
    {
        // Play the attack animation.
        Debug.Log("Attacked!");
    }

    public void UpdateState()
    {
        // Handle attack logic, such as checking for attack duration, etc.
    }

    public void OnExit()
    {
        // Perform any necessary cleanup or exit actions
    }
}