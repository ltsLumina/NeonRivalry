using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Debugging;
using UnityEngine;

public class AttackHandler
{
    // -- Fields --

    [SerializeField] Moveset moveset;
    [SerializeField] Animator animator;
    [SerializeField] PlayerController player;

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

            default:
                Debug.LogWarning("Attack type is not valid. \nIf you got this error then I am honestly impressed.");
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