using UnityEngine;

// This is a partial class, which allows us to split the class definition across multiple files.
// This is useful for keeping the code organized and easy to read.
// This file contains all of the input checks, such as checking if the player is moving, jumping, etc.
//TODO: In hindsight, maybe I should put this into its own class rather than as a partial class of PlayerController.
public partial class PlayerController // StateChecks.cs
{
    public bool IsMoving()
    {
        return InputManager.MoveInput != Vector2.zero && PlayerRB.velocity != Vector2.zero && IsGrounded() && stateMachine.CurrentState is MoveState
        { IsMoving: true };
    }

    public bool IsJumping()
    {
        return !IsGrounded() && PlayerRB.velocity.y > 0 && stateMachine.CurrentState is JumpState
        { IsJumping: true };
    }

    public bool IsFalling()
    {
        return !IsGrounded() && PlayerRB.velocity.y < 0 && stateMachine.CurrentState is FallState
        { IsFalling: true };
    }

    public bool IsAttacking()
    {
        return stateMachine.CurrentState is AttackState
        { IsAttacking: true };
    }

    //TODO: check if this is actually a reasonable way to check if the player is idle.
    public bool IsIdle()
    {
        return !IsMoving() && !IsJumping() && !IsFalling() && !IsAttacking();
    }
}