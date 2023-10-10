using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Debugging;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public enum AttackType
    {
        Punch,
        Kick,
        Slash,
        Unique
    }
    
    // -- Fields --
    Moveset moveset;
    Animator animator;
    PlayerController player;

    // -- Constants --
    const int NeutralPunchIndex = 0;
    const int ForwardPunchIndex = 1;
    const int UpPunchIndex = 2;
    const int DownPunchIndex = 3;

    //A dictionary that maps a direction key to a tuple containing an action to perform, a log message, and an animation index.
    readonly static Dictionary<MoveData.Direction, (Action<MoveData> action, string logMessage, int animationIndex)> directionToActionMap = new ()
    { { MoveData.Direction.Neutral, (null, "Neutral move performed.", NeutralPunchIndex) },
      { MoveData.Direction.Forward, (null, "Forward move performed.", ForwardPunchIndex) },
      { MoveData.Direction.Up, (null, "Up move performed.", UpPunchIndex) },
      { MoveData.Direction.Down, (null, "Down move performed.", DownPunchIndex) } };

    /// <summary>
    ///     Returns the move that corresponds to the direction that the player is pressing.
    ///     <para></para>
    ///     If the player is not airborne,
    ///     the move that will be performed is the move that corresponds to the direction that the player is pressing.
    /// </summary>
    /// <para>Used to execute the action that corresponds to the direction that the player is pressing.</para>
    /// <returns> The move that corresponds to the direction that the player is pressing. </returns>
    /// <example> If the player is pressing Up, then the move that will be performed is the "Up" move. </example>
    public void SelectAttack(AttackType attackType)
    {
        List<MoveData> moves = attackType switch
        { AttackType.Punch  => moveset.punchMoves,
          AttackType.Kick   => moveset.kickMoves,
          AttackType.Slash  => moveset.slashMoves,
          AttackType.Unique => moveset.uniqueMoves,
          _                 => throw new ArgumentException(nameof(attackType)) };

        // Handle non-matching types in the list of moves.
        if (moves.Exists(move => move.type != (MoveData.Type) attackType)) 
            Debug.LogWarning($"One or more {attackType} moves are not of type \"{attackType}\".");

        // Reference to the player's input.
        var input = player.InputManager.MoveInput;

        // If the player is airborne, the move that will be performed is the "Up" move.
        var directionToPerform = player.IsAirborne() ? MoveData.Direction.Up : GetDirectionFromInput(input);

        // Get the move that corresponds to the direction that the player is pressing.
        var selectedMove = moves.FirstOrDefault(move => move.direction == directionToPerform);

        // Perform the move.
        PerformMove(selectedMove, directionToPerform, attackType);
    }

    /// <summary>
    ///     Performs the selected punch action based on the given direction.
    ///     Also handles the animation and applying move effects.
    /// </summary>
    /// <param name="selectedMove">The MoveData instance representing the punch to perform.</param>
    /// <param name="directionToPerform">The direction in which to perform the punch.</param>
    void PerformMove(MoveData selectedMove, MoveData.Direction directionToPerform, AttackType attackType)
    {
        {
            // Execute the action that corresponds to the direction that the player is pressing.
            if (selectedMove == null) return;

            // Play the animation which handles all logic for the move, such as hitboxes, hurtboxes, etc.
            PlayAnimation(selectedMove, directionToActionMap[directionToPerform].animationIndex, attackType);

            // Apply the move effects, if any.
            //TODO: this block will be moved into the animation event. 
            // if (selectedPunch.moveEffects != null)
            // {
            //     // Iterate through the move effects and apply them to the player, if any.
            //     foreach (MoveEffect effect in selectedPunch.moveEffects)
            //     {
            //         //effect.ApplyEffect(abilities, null);
            //         FGDebugger.Debug("Applied effect: " + effect.name, LogType.Log, StateType.Attack);
            //     }
            // }

            // Log the action that was performed.
            FGDebugger.Debug(directionToActionMap[directionToPerform].logMessage, LogType.Log, State.StateType.Attack);
        }
    }

    public void SelectPunch() => SelectAttack(AttackType.Punch);
    public void SelectKick() => SelectAttack(AttackType.Kick);
    public void SelectSlash() => SelectAttack(AttackType.Slash);
    public void SelectUnique() => SelectAttack(AttackType.Unique);

    #region Utility
    /// <summary>
    ///     Returns the direction that the player is pressing.
    ///     Does not include the "Up" direction as that is handled separately.
    ///     <see cref="SelectPunch" /> for more information.
    /// </summary>
    /// <param name="input"> The player's input. </param>
    /// <returns> The direction that the player is pressing. </returns>
    static MoveData.Direction GetDirectionFromInput(Vector2 input)
    {
        // If the player is not inputting a direction, the move that will be performed is the "Neutral" move.
        if (input == Vector2.zero) return MoveData.Direction.Neutral;

        // If the player is inputting a direction, execute the action that corresponds to the direction that the player is pressing.
        return input.x != 0 ? MoveData.Direction.Forward : MoveData.Direction.Down;
    }
    #endregion

    // If the player is inputting down, the move that will be performed is the "Down" move.
    void PlayAnimation(MoveData selectedMove, int index, AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Punch:
                animator.SetInteger("SelectedPunch", index);
                animator.SetTrigger("Punch");
                break;

            case AttackType.Kick:
                animator.SetInteger("SelectedKick", index);
                animator.SetTrigger("Kick");
                break;

            case AttackType.Slash:
                animator.SetInteger("SelectedSlash", index);
                animator.SetTrigger("Slash");
                break;

            case AttackType.Unique:
                animator.SetInteger("SelectedUnique", index);
                animator.SetTrigger("Unique");
                break;
            
            default:
                throw new ArgumentException(nameof(attackType));
        }

        // Play sound
    }
}