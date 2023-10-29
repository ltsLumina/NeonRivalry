#region
using UnityEngine;
using Logger = Lumina.Debugging.Logger;
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
        bool raycastHit = Physics.Raycast(transform.position, Vector3.down, raycastDistance,groundLayer);
        
        Logger.Trace($"IsGrounded() is {raycastHit}", State.StateType.Idle);

        return raycastHit;
    }

    public bool IsMoving()
    {
        bool isMoving = IsGrounded() && Rigidbody.velocity.x != 0 && StateMachine.CurrentState is MoveState
        { IsMoving: true };

        Logger.Trace($"IsMoving() is {isMoving}", State.StateType.Walk);

        return isMoving;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsJumping()
    {
        bool isJumping = Rigidbody.velocity.y > 0 && StateMachine.CurrentState is JumpState
        { IsJumping: true };

        Logger.Trace($"IsJumping() is {isJumping}", State.StateType.Jump);

        return isJumping;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsFalling()
    {
        bool isFalling = !IsGrounded() || (Rigidbody.velocity.y < 0 && StateMachine.CurrentState is FallState
        { IsFalling: true });

        Logger.Trace($"IsFalling() is {isFalling}", State.StateType.Fall);

        return isFalling;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsAttacking()
    {
        // Don't know if the normalizedTime check is necessary, but it's here just in case.
        // It checks if the animation is still playing, and if it is, it returns true.
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f && StateMachine.CurrentState is AttackState
        { IsAttacking: true };

        Logger.Trace($"IsAttacking() is {isAttacking}", State.StateType.Attack);

        return isAttacking;
    }

    public bool IsAirborneAttacking()
    {
        bool isAirborneAttacking = StateMachine.CurrentState is AirborneAttackState
        { IsAirborneAttacking: true, IsAirborne: true};
        
        Logger.Trace($"IsAirborneAttacking() is {isAirborneAttacking}", State.StateType.AirborneAttack);
        
        return isAirborneAttacking;
    }

    /// <summary>
    /// This method checks if the player is airborne, which is defined as not being grounded, jumping, or falling.
    /// </summary>
    /// <returns> True if the player is airborne, false otherwise. </returns>
    public bool IsAirborne()
    {
        bool isAirborne = !IsGrounded() || StateMachine.CurrentState is JumpState
        { IsJumping: true } or FallState
        { IsFalling: true } or AirborneAttackState
        { IsAirborneAttacking: true, IsAirborne: true };
        
        Logger.Trace($"IsAirborne() is {isAirborne}", new [] { State.StateType.Jump , State.StateType.Fall, State.StateType.AirborneAttack });

        return isAirborne;
    }

    /// <summary>
    /// This method checks if the player is idle.
    /// </summary>
    /// <returns> True if the player is idle, false otherwise.</returns>
    public bool IsIdle()
    {
        bool isIdle = IsGrounded() && !(IsMoving() || IsAirborne() || IsAttacking() || IsAirborneAttacking());

        Logger.Trace($"IsIdle() is {isIdle}", State.StateType.Idle);

        return isIdle;
    }
    
    public bool CanMove() => IsIdle();

    public bool CanJump() => IsGrounded();

    public bool CanFall() => IsAirborne();

    public bool CanAttack() => IsGrounded() && !IsAttacking() && !IsAirborneAttacking();

    public bool CanAirborneAttack() => IsAirborne() && !IsAirborneAttacking() && !IsAttacking();

    // -- Gizmos --

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }
}