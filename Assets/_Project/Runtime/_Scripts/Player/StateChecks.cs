#region
using Lumina.Debugging;
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
        bool raycastHit = Physics.Raycast(transform.position, Vector3.down,raycastDistance,groundLayer);
        
        FGDebugger.Trace($"IsGrounded() is {raycastHit}", State.StateType.Idle);

        return raycastHit;
    }

    public bool IsMoving()
    {
        bool isMoving = IsGrounded() && Rigidbody.velocity.x != 0 && StateMachine.CurrentState is MoveState
        { IsMoving: true };

        FGDebugger.Trace($"IsMoving() is {isMoving}", State.StateType.Walk);

        return isMoving;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsJumping()
    {
        bool isJumping = IsGrounded() && Rigidbody.velocity.y > 0 && StateMachine.CurrentState is JumpState
        { IsJumping: true };

        FGDebugger.Trace($"IsJumping() is {isJumping}", State.StateType.Jump);

        return isJumping;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsFalling()
    {
        bool isFalling = !IsGrounded() || (Rigidbody.velocity.y < 0 && StateMachine.CurrentState is FallState
        { IsFalling: true });

        FGDebugger.Trace($"IsFalling() is {isFalling}", State.StateType.Fall);

        return isFalling;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsAttacking()
    {
        //TODO: This is a temporary implementation. We need to check if the player is attacking some other way.
        bool isAttacking = StateMachine.CurrentState is AttackState
        { IsAttacking: true };

        FGDebugger.Trace($"IsAttacking() is {isAttacking}", State.StateType.Attack);

        return isAttacking;
    }

    public bool IsAirborneAttacking()
    {
        bool isAirborneAttacking = StateMachine.CurrentState is AirborneAttackState
        { IsAirborneAttacking: true };
        
        FGDebugger.Trace($"IsAirborneAttacking() is {isAirborneAttacking}", State.StateType.AirborneAttack);
        
        return isAirborneAttacking;
    }

    /// <summary>
    /// This method checks if the player is airborne, which is defined as not being grounded, jumping, or falling.
    /// </summary>
    /// <returns> True if the player is airborne, false otherwise. </returns>
    public bool IsAirborne() => !IsGrounded() || IsJumping() || IsFalling();
    
    /// <summary>
    /// This method checks if the player is idle.
    /// </summary>
    /// <returns> True if the player is idle, false otherwise.</returns>
    public bool IsIdle()
    {
        bool isIdle = IsGrounded() && !(IsMoving() || IsAirborne() || IsAttacking());

        FGDebugger.Trace($"IsIdle() is {isIdle}", State.StateType.Idle);

        return isIdle;
    }
    
    public bool CanMove() => IsIdle();

    public bool CanJump() => !IsAirborne() && !IsAttacking();
    
    public bool CanFall() => IsAirborne() && !IsAttacking();

    public bool CanAttack() => !IsAttacking();
    
    public bool CanAirborneAttack() => IsAirborne();

    // -- Gizmos --

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }
}