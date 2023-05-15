using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public enum StateType
    {
        Idle,
        Walk,       // Walk indicates that the player is moving.
        Run,        // Run indicates that the player is moving faster than walking.
        Jump,
        Attack,
        Block,
        HitStun,    // HitStun indicates that the player has been hit and is unable to move or attack for a short period of time.
        Knockdown,  // Knockdown indicates that the player has been knocked down and is unable to move or attack for a short period of time.
        Dead,
        None,       // None is a special state that is used to indicate that the there is no player, and therefore, no state.
    }

    public abstract void OnEnter(StateMachine stateMachine);
    public abstract void OnExit(StateMachine stateMachine);
    public abstract void UpdateState(StateMachine stateMachine);
}

//TODO: HERE'S THE PLAN:
#region HOW TO MAKE STATE MACHINE
// Here's a recommended approach:
//
// 1. Create individual state classes:
// Create separate classes for each state, such as IdleState, AttackState, JumpState, KnockdownState, etc.
// These classes should implement the IState interface or extend a base state class.
//
// 2. Implement state-specific behavior:
// Inside each state class, you define the logic and behavior specific to that state.
// For example, in the AttackState, you would handle attack animations, damage calculations, and any other actions associated with attacking.
// Similarly, in the JumpState, you would handle the jumping behavior, apply forces, and manage the jump duration.
//
// 3. State transitions:
// Within each state class, you can determine when and how to transition to other states.
// For example, in the AttackState, you might transition to the IdleState once the attack animation is complete or based on certain conditions.
// These transitions can be handled by calling methods on the state machine or using events.
//
// 4. Player controller script:
// The player controller script would then contain the state machine and handle the high-level coordination between the states.
// It would manage the state transitions, handle input, and update the current state accordingly.
// The player controller script would not contain the detailed logic of each state but would orchestrate their execution.
#endregion