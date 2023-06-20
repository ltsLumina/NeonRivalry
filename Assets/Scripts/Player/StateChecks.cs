using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This is a partial class, which allows us to split the class definition across multiple files.
// This is useful for keeping the code organized and easy to read.
// This file contains all of the input checks, such as checking if the player is moving, jumping, etc.
//TODO: In hindsight, maybe I should put this into its own class rather than as a partial class of PlayerController.
public partial class PlayerController // StateChecks.cs
{
    public bool IsGrounded()
    {
        // Perform an overlap check using the player's collider
        // and a layer mask that includes the ground or platform layer.
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, Vector3.one * groundDistance, 0f, groundLayer);

        // Check if any colliders are found (indicating the player is grounded)
        return colliders.Length > 0;
    }
    
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

    //TODO: check if this is actually a reasonable way to check if the player is idle.
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
        return conditions.All(condition => condition());
    }
}