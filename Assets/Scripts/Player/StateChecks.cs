using System;
using System.Collections.Generic;
using UnityEngine;
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
        bool isHit = Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit _, 1, groundLayer);
    
        // Uncomment this line if you want to debug the IsGrounded() method.
        // if (isHit) { Debug.Log("IsGrounded() is true"); }
    
        return isHit;
    }

    public bool IsMoving()
    {
        bool isMoving = PlayerRB.velocity.x != 0 && IsGrounded() && stateMachine.CurrentState is MoveState
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

        if (isAttacking)
        {
            Debug.LogWarning("IsAttacking() only works when the player is attacking with the mouse. This is a temporary implementation.");
            Debug.Log("IsAttacking() is true");
        }

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
        return 
            IsGrounded() 
            && !IsMoving() 
            && !IsJumping() 
            && !IsFalling() 
            && !IsAttacking();
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