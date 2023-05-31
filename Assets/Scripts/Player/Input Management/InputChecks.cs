using UnityEngine;

// This is a partial class, which allows us to split the class definition across multiple files.
// This is useful for keeping the code organized and easy to read.
// This file contains all of the input checks, such as checking if the player is moving, jumping, etc.
public partial class InputManager // InputChecks.cs
{
    bool IsMoving()
    {
        return MoveInput != Vector2.zero && playerRB.velocity != Vector2.zero && player.IsGrounded() && stateMachine.CurrentState is MoveState
        { IsMoving: true };
    }

    bool IsJumping()
    {
        return !player.IsGrounded() && playerRB.velocity.y > 0 && stateMachine.CurrentState is JumpState
        { IsJumping: true };
    }

    bool IsFalling()
    {
        return
            !player.IsGrounded() &&
            playerRB.velocity.y <
            0 /* && stateMachine.CurrentState != null && stateMachine.CurrentState.Type == State.StateType.Falling*/;
    }

    bool IsAttacking()
    {
        return stateMachine.CurrentState is AttackState
        { IsAttacking: true };
    }

    //TODO: check if this is actually a reasonable way to check if the player is idle.
    bool IsIdle() { return !IsMoving() && !IsJumping() && !IsFalling() && !IsAttacking(); }
}