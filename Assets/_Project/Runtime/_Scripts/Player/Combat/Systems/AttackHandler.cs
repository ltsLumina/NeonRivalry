using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = Lumina.Debugging.Logger;

public class AttackHandler
{
    // -- Fields --

    readonly Moveset moveset;
    readonly Animator animator;
    readonly PlayerController player;

    /// <summary>
    ///     A dictionary that defines the associations between different move directions and their corresponding actions, log
    ///     messages, and animation indexes.
    /// </summary>
    readonly static Dictionary<MoveData.Direction, (string logMessage, int animationIndex)> directionToActionMap = new ()
    { { MoveData.Direction.Neutral, ("Neutral move performed.", (int) MoveData.Direction.Neutral) },
      { MoveData.Direction.Forward, ("Forward move performed.", (int) MoveData.Direction.Forward) },
      { MoveData.Direction.Airborne, ("Airborne move performed.", (int) MoveData.Direction.Airborne) },
      { MoveData.Direction.Down, ("Crouch move performed.", (int) MoveData.Direction.Down) } };

    /// <summary>
    /// Initializes a new instance of the <see cref="AttackHandler"/> class.
    /// </summary>
    /// <param name="moveset">The <see cref="Moveset"/> object which the player is going to use for attacks.</param>
    /// <param name="animator">The <see cref="Animator"/> component attached to the player, used to control and play back animations.</param>
    /// <param name="player">The <see cref="PlayerController"/> object which references the player controlled character.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when moveset is null.</exception>
    /// <remarks>
    /// This is used to handle the player's attacks according to the provided moveset with animations. 
    /// You need to make sure that the moveset is not null when using this handler; otherwise, an assertion failure will be triggered in Debug mode. 
    /// </remarks>
    public AttackHandler(Moveset moveset, Animator animator, PlayerController player)
    {
        this.moveset  = moveset;
        this.animator = animator;
        this.player   = player;

        Debug.Assert(moveset != null, "Moveset is null in the AttackStateData. Please assign it in the inspector.");
    }

    /// <summary>
    /// Selects an attack based on the provided <see cref="InputManager.AttackType"/>.
    /// </summary>
    /// <param name="attackType">The type of attack to be performed. This is an enumeration value of <see cref="InputManager.AttackType"/>.</param>
    /// <remarks>
    /// The method gets a list of moves associated with the given attack type by calling <see cref="GetAttackMoves"/>. 
    /// If there are no attack moves for the supplied type, the method returns.
    /// <para>A Vector2 input is then retrieved from <see cref="InputManager.MoveInput"/>.</para>
    /// The direction to perform is calculated based on the player's aerial status or the direction derived from the input.
    /// The attack to perform is determined by finding the first move in the attack moves list that matches the direction to perform (if any).
    /// The attack is then performed by calling <see cref="PerformAttack"/>.
    /// </remarks>
    /// <returns>True if the attack was selected successfully; otherwise, false.</returns>
    public bool SelectAttack(InputManager.AttackType attackType)
    {
        List<MoveData> attackMoves = GetAttackMoves(attackType);

        // If there are no attack moves available, then return without performing a move
        if (attackMoves == null) return false;

        // Get the current movement input of the player
        Vector2 input = player.InputManager.MoveInput;

        // Determine the direction of attack based on whether the player is currently airborne or not
        MoveData.Direction directionToPerform = player.IsAirborne() ? MoveData.Direction.Airborne : GetDirectionFromInput(input);
        
        Logger.Trace("Attack direction determined. It has been set to: " + directionToPerform, new[]
                         { State.StateType.Attack, State.StateType.AirborneAttack });

        // Select the appropriate attack move based on the direction of the attack
        MoveData selectedAttack = attackMoves.FirstOrDefault(move => move.direction == directionToPerform);

        // Perform the selected attack move
        return PerformAttack(selectedAttack, directionToPerform, attackType);
    }

    /// <summary>
    /// Performs attack with the given selected move, direction, and attack type.
    /// </summary>
    /// <param name="selectedAttack">The selected move data. Contains details about the move being performed.</param>
    /// <param name="directionToPerform">The direction to perform the attack.</param>
    /// <param name="type">The type of attack (determined via <see cref="InputManager.AttackType"/>).</param>
    /// <remarks>
    /// If <paramref name="selectedAttack"/> is <c>null</c>, a warning log is produced and the method returns early.
    /// After assigning direction to action map, an animation corresponding to the direction and type of attack is being played.
    /// A debug message is also logged specifying the attack type being performed in conjunction with the direction.
    /// </remarks>
    /// <returns>True if the attack was performed successfully; otherwise, false.</returns>
    bool PerformAttack(MoveData selectedAttack, MoveData.Direction directionToPerform, InputManager.AttackType type)
    {
        if (selectedAttack == null)
        {
            Logger.Log("There is no move that corresponds to the direction that the player is pressing. \nPlease assign a move in the moveset.", LogType.Warning);
            Logger.Log("Returned out of an attack early. \nThis means the player might behave unexpectedly.", LogType.Warning);
            return false;
        }

        // Get the animation index based on the direction to perform.
        // The airborne attack only has one animation, so the animation index is always 0.
        int animationIndex = type == InputManager.AttackType.Airborne ? 0 : directionToActionMap[directionToPerform].animationIndex;
        PlayAnimation(selectedAttack, animationIndex, type);

        // Logs the attack type that is being performed.
        string attackType = Enum.GetName(typeof(InputManager.AttackType), type);
        string logMessage = $"Performing {attackType} attack in the {directionToPerform} direction.";

        Logger.Debug(logMessage, LogType.Log, new[] { State.StateType.Attack, State.StateType.AirborneAttack });

        return true;
    }

    /// <summary>
    /// Plays a specific attack animation based on the specified attack type and index.
    /// </summary>
    /// <param name="move">The move data (This parameter is currently unused but it's planned to be used in the future).</param>
    /// <param name="animationIndex">The index of the attack within the attack type's sequence of animations.</param>
    /// <param name="type">The type of the attack.</param>
    /// <remarks>
    /// The method sets the animator parameters based on the type of attack and the index provided, and then triggers the attack animation.
    /// The name of the parameter in the animator must be the same as the name of the attack type.
    /// It also records this action to the Logger for traceability purposes.
    /// </remarks>
    void PlayAnimation(MoveData move, int animationIndex, InputManager.AttackType type)
    {
        string attackType = Enum.GetName(typeof(InputManager.AttackType), type);

        // For non-airborne attacks, set the selected attack integer.
        // if (type != InputManager.AttackType.Airborne)
        // {
        //     string selectedAttack = "Selected" + attackType;
        //     animator.SetInteger(selectedAttack, animationIndex);
        // }

        // Trigger the correct animation.
        // If the attack type is airborne, then the animation index is always 0.
        //animator.SetTrigger(attackType);

        // wait for current animation to finish
        var length = animator.GetCurrentAnimatorStateInfo(0).length;
        player.StartCoroutine(WaitForAnimation(length));
        
        animator.Play(move.direction + " " + attackType, 0, 0f);
        //Debug.Log($"Playing {directionToActionMap[move.direction]} {attackType} animation.");

        Logger.Trace
        ($"Attack animation played. Animator parameters set to: \n{attackType} = true \nAnimation Index = {animationIndex}", new[]
         { State.StateType.Attack, State.StateType.AirborneAttack });
    }
    
    IEnumerator WaitForAnimation(float length)
    {
        yield return new WaitForSeconds(length);
    }

    #region Query Methods
    /// <summary>
    /// Retrieves a list of attack moves based on the given attack type.
    /// </summary>
    /// <param name="attackType">The type of attack. This should be one of the <see cref="InputManager.AttackType"/> options.</param>
    /// <returns>
    /// A list of <see cref="MoveData"/> corresponding to the specified attack type, or NULL if invalid attack type is provided.
    /// </returns>
    /// <remarks>
    /// The function switches on the provided attack type and retrieves the corresponding list of moves from the `moveset` object. 
    /// If the provided attack type is `None` or is not defined in the <see cref="InputManager.AttackType"/> enumeration, 
    /// it logs a warning and returns an empty list. (null)
    /// </remarks>
    List<MoveData> GetAttackMoves(InputManager.AttackType attackType)
    {
        switch (attackType)
        {
            case InputManager.AttackType.Punch:
                return moveset.PunchMoves;

            case InputManager.AttackType.Kick:
                return moveset.KickMoves;

            case InputManager.AttackType.Slash:
                return moveset.SlashMoves;

            case InputManager.AttackType.Airborne:
                return moveset.AirborneMoves;

            case InputManager.AttackType.Unique:
                return moveset.UniqueMoves;

            case InputManager.AttackType.None:
            default:
                Debug.LogWarning($"The current attack type \"{attackType}\" is not valid. " + "\nIf you got this error then I am honestly impressed.");
                return null;
        }
    }

    /// <summary>
    ///     Returns the direction that the player is pressing.
    ///     Does not include the "Airborne" direction as that is handled separately within
    ///     the <see cref="SelectAttack" /> method.
    /// </summary>
    /// <param name="inputDirection"> The direction the player is inputting. </param>
    /// <returns> The direction that the player is pressing. </returns>
    static MoveData.Direction GetDirectionFromInput(Vector2 inputDirection) => inputDirection switch
    { _ when inputDirection   == Vector2.zero => MoveData.Direction.Neutral,
      _ when inputDirection.x != 0            => MoveData.Direction.Forward,
      _ when inputDirection.y < 0             => MoveData.Direction.Down,
      _                                       => MoveData.Direction.Neutral };
    #endregion
}