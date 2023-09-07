#region
using UnityEngine;
#endregion

// This is a partial class, which allows us to split the class definition across multiple files.
// This is useful for keeping the code organized and easy to read.
// This file contains all of the input checks, such as checking if the player is moving, jumping, etc.
//TODO: In hindsight, maybe I should put this into its own class rather than as a partial class of PlayerController.
public partial class PlayerController // StateChecks.cs
{
    // -- State Checks --

    public bool IsGrounded()
    {
        bool isHit = Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit _, 1, groundLayer);

        Debugger.Debug("IsGrounded() is true", State.StateType.Idle);
 
        return isHit;
    }

    public bool IsMoving()
    {
        bool isMoving = Rigidbody.velocity.x != 0 && IsGrounded() && stateMachine.CurrentState is MoveState
        { IsMoving: true };

        Debugger.Debug("IsMoving() is true", State.StateType.Walk);

        return isMoving;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsJumping()
    {
        bool isJumping = IsGrounded() && Rigidbody.velocity.y > 0 && stateMachine.CurrentState is JumpState
        { IsJumping: true };

        Debugger.Debug("IsJumping() is true", State.StateType.Jump);

        return isJumping;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsFalling()
    {
        bool isFalling = !IsGrounded() || Rigidbody.velocity.y < 0 && stateMachine.CurrentState is FallState
        { IsFalling: true };

        Debugger.Debug($"IsFalling() is {isFalling}", State.StateType.Fall);

        return isFalling;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsAttacking()
    {
        //TODO: This is a temporary implementation. We need to check if the player is attacking some other way.
        bool isAttacking = stateMachine.CurrentState is AttackState
        { IsAttacking: true };

        Debugger.Debug("IsAttacking() is true", State.StateType.Attack);

        return isAttacking;
    }

    /// <summary>
    /// This method checks if the player is airborne, which is defined as not being grounded, jumping, or falling.
    /// </summary>
    /// <returns> True if the player is airborne, false otherwise. </returns>
    public bool IsAirborne() => !IsGrounded() || IsJumping() || IsFalling();

    /// <summary>
    /// This method checks if the player is idle.
    /// </summary>
    /// <returns>True if the player is idle, false otherwise.</returns>
    public bool IsIdle()
    {
        bool isIdle = 
            IsGrounded() 
            && !IsMoving() 
            && !IsJumping() 
            && !IsFalling() 
            && !IsAttacking();

        Debugger.Debug($"IsIdle() is {isIdle}", State.StateType.Idle);

        return isIdle;
    }

    public bool CanMove() => !IsIdle();

    public bool CanJump() => !IsAirborne();

    public bool CanAttack() => !IsAttacking();

    // -- Gizmos --

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, Vector3.one * groundDistance);
    }
}