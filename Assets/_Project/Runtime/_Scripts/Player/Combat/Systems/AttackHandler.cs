using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Debugging;
using UnityEngine;

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
      { MoveData.Direction.Horizontal, ("Horizontal move performed.", (int) MoveData.Direction.Horizontal) },
      { MoveData.Direction.Airborne, ("Airborne move performed.", (int) MoveData.Direction.Airborne) },
      { MoveData.Direction.Crouch, ("Crouch move performed.", (int) MoveData.Direction.Crouch) } };

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
    /// A Vector2 input is then retrieved from <see cref="InputManager.MoveInput"/>.
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
            Debug.LogWarning("There is no move that corresponds to the direction that the player is pressing. \nPlease assign a move in the moveset.");
            return false;
        }

        // Get the animation index based on the direction to perform.
        // The airborne attack only has one animation, so the animation index is always 0.
        int animationIndex = type == InputManager.AttackType.Airborne ? 0 : directionToActionMap[directionToPerform].animationIndex;
        PlayAnimation(selectedAttack, animationIndex, type);

        // Logs the attack type that is being performed.
        string attackType = Enum.GetName(typeof(InputManager.AttackType), type);
        string logMessage = $"Performing {attackType} attack in the {directionToPerform} direction.";

        FGDebugger.Debug
        (logMessage, LogType.Log, new[]
         { State.StateType.Attack, State.StateType.AirborneAttack });

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
    /// It also records this action to the FGDebugger for traceability purposes.
    /// </remarks>
    void PlayAnimation(MoveData move, int animationIndex, InputManager.AttackType type)
    {
        string attackType = Enum.GetName(typeof(InputManager.AttackType), type);

        // For non-airborne attacks, set the selected attack integer.
        if (type != InputManager.AttackType.Airborne)
        {
            string selectedAttack = "Selected" + attackType;
            animator.SetInteger(selectedAttack, animationIndex);
        }

        // Trigger the correct animation.
        // If the attack type is airborne, then the animation index is always 0.
        animator.SetTrigger(attackType);

        FGDebugger.Trace
        ($"Attack animation played. Animator parameters set to: \n{attackType} = true \nAnimation Index = {animationIndex}", new[]
         { State.StateType.Attack, State.StateType.AirborneAttack });
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
                return moveset.punchMoves;

            case InputManager.AttackType.Kick:
                return moveset.kickMoves;

            case InputManager.AttackType.Slash:
                return moveset.slashMoves;

            case InputManager.AttackType.Airborne:
                return moveset.airborneMoves;

            case InputManager.AttackType.Unique:
                return moveset.uniqueMoves;

            case InputManager.AttackType.None:
            default:
                Debug.LogWarning($"The current attack type \"{attackType}\" is not valid. " + "\nIf you got this error then I am honestly impressed.");
                return new ();
        }
    }
    
    /// <summary>
    ///     Returns the direction that the player is pressing.
    ///     Does not include the "Airborne" direction as that is handled separately.
    /// </summary>
    /// <param name="input"> The player's input. </param>
    /// <returns> The direction that the player is pressing. </returns>
    static MoveData.Direction GetDirectionFromInput(Vector2 input)
    {
        // If the player is not inputting a direction, the move that will be performed is the "Neutral" move.
        if (input == Vector2.zero) return MoveData.Direction.Neutral;

        // If the player is inputting a direction, execute the action that corresponds to the direction that the player is pressing.
        return input.x != 0 ? MoveData.Direction.Horizontal : MoveData.Direction.Crouch;
    }
    #endregion
}