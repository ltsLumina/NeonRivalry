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

    // -- Constants --
    const int NeutralAttackIndex = 0;
    const int ForwardAttackIndex = 1;
    const int UpPunchIndex = 2;
    const int DownPunchIndex = 3;

    /// <summary>
    ///     A dictionary that defines the associations between different move directions and their corresponding actions, log
    ///     messages, and animation indexes.
    /// </summary>
    readonly static Dictionary<MoveData.Direction, (string logMessage, int animationIndex)> directionToActionMap = new ()
    { { MoveData.Direction.Neutral, ("Neutral move performed.", NeutralAttackIndex) },
      { MoveData.Direction.Forward, ("Forward move performed.", ForwardAttackIndex) },
      { MoveData.Direction.Up, ("Up move performed.", UpPunchIndex) },
      { MoveData.Direction.Down, ("Down move performed.", DownPunchIndex) } };
    
    public AttackHandler(Moveset moveset, Animator animator, PlayerController player)
    {
        this.moveset  = moveset;
        this.animator = animator;
        this.player   = player;
    }

    public void SelectAttack(InputManager.AttackType attackType)
    {
        // Get the appropriate moveset for the attack type
        List<MoveData> attackMoves;

        switch (attackType)
        {
            case InputManager.AttackType.Punch:
                attackMoves = moveset.punchMoves;
                break;

            case InputManager.AttackType.Kick:
                attackMoves = moveset.kickMoves;
                break;

            case InputManager.AttackType.Slash:
                attackMoves = moveset.slashMoves;
                break;

            case InputManager.AttackType.Unique:
                attackMoves = moveset.uniqueMoves;
                break;

            // If the attack type is none, then the default case is executed.
            case InputManager.AttackType.None:
                
            default:
                Debug.LogWarning($"The current attack type \"{attackType}\" is not valid. \nIf you got this error then I am honestly impressed.");
                return;
        }

        Vector2 input = player.InputManager.MoveInput;

        MoveData.Direction directionToPerform = player.IsAirborne() ? MoveData.Direction.Up : GetDirectionFromInput(input);

        MoveData selectedAttack = attackMoves.FirstOrDefault(move => move.direction == directionToPerform);
        PerformAttack(selectedAttack, directionToPerform, attackType);
    }

    void PerformAttack(MoveData selectedAttack, MoveData.Direction directionToPerform, InputManager.AttackType type)
    {
        if (selectedAttack == null)
        {
            Debug.LogWarning("There is no move that corresponds to the direction that the player is pressing. \nPlease assign a move in the moveset.");
            return;
        }

        PlayAnimation(selectedAttack, directionToActionMap[directionToPerform].animationIndex, type);

        FGDebugger.Debug(directionToActionMap[directionToPerform].logMessage, LogType.Log, State.StateType.Attack);
    }

    void PlayAnimation(MoveData move, int index, InputManager.AttackType type)
    {
        animator.SetInteger("Selected" + Enum.GetName(typeof(InputManager.AttackType), type), index);
        animator.SetTrigger(Enum.GetName(typeof(InputManager.AttackType), type));
    }

    #region Utility
    /// <summary>
    ///     Returns the direction that the player is pressing.
    ///     Does not include the "Up" direction as that is handled separately.
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
}