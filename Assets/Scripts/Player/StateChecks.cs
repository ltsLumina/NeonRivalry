using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static State;
using Debug = UnityEngine.Debug;

// This is a partial class, which allows us to split the class definition across multiple files.
// This is useful for keeping the code organized and easy to read.
// This file contains all of the input checks, such as checking if the player is moving, jumping, etc.
//TODO: In hindsight, maybe I should put this into its own class rather than as a partial class of PlayerController.
public partial class PlayerController // StateChecks.cs
{
    // -- State Checks --
    
    public bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, Vector3.one * groundDistance, 0f, groundLayer);

        bool isGrounded = colliders.Length > 0;
        
        //if (isGrounded) { Debug.Log("IsGrounded() is true"); }

        return isGrounded;
    }

    public bool IsMoving()
    {
        bool isMoving = InputManager.MoveInput.x != 0 && PlayerRB.velocity.x != 0 && IsGrounded() && stateMachine.CurrentState is MoveState
        { IsMoving: true };

        //Debug.Log("IsMoving() is true");

        return isMoving;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsJumping()
    {
        bool isJumping = IsGrounded() && PlayerRB.velocity.y > 0  && stateMachine.CurrentState is JumpState
        { IsJumping: true };

        if (isJumping) { Debug.Log("IsJumping() is true"); }

        return isJumping;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsFalling()
    {
        bool isFalling = !IsGrounded() || PlayerRB.velocity.y < 0 && stateMachine.CurrentState is FallState
        { IsFalling: true };

        if (isFalling) { Debug.Log("IsFalling() is true"); }

        return isFalling;
    }


    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsAttacking()
    {
        //TODO: This is a temporary implementation. We need to check if the player is attacking some other way.
        bool isAttacking = Input.GetKeyDown(KeyCode.Mouse0) && stateMachine.CurrentState is AttackState
        { IsAttacking: true };

        if (isAttacking) { Debug.Log("IsAttacking() is true"); }

        return isAttacking;
    }


    /// <summary>
    ///     This method checks if the player is airborne, which is defined as not being grounded, jumping, or falling.
    ///     Used as shorthand for writing the above three methods.
    /// </summary>
    /// <returns> True if the player is airborne, false otherwise. </returns>
    public bool IsAirborne() => !IsGrounded() || IsJumping() || IsFalling();

    /// <summary>
    ///     This method checks if the player is disabled, which is defined as being stunned by a hit (hitstun), or in a
    ///     blocking stance.
    ///     Used as shorthand for writing the above two methods.
    /// </summary>
    /// <remarks>I just wanted a reason to name a boolean "IsDisabled" lol</remarks>
    /// <returns>True if the player is in a hitstun state or blocking, false otherwise.</returns>
    //public bool IsDisabled() => IsHitstun() || IsBlocking();
    
    public bool IsIdle()
    {
        // Create a list of conditions to check. If any of them are true, the player is not idle.
        List<Func<bool>> conditions = new ()
        { () => IsGrounded(),
          () => !IsMoving(),
          () => !IsJumping(),
          () => !IsFalling(),
          () => !IsAttacking(),
        };
        
        // Check all conditions, return false if any of them are true
        return conditions.All(condition => condition()) && stateMachine.CurrentState is IdleState;
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