using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Debugging;
using UnityEngine;

public class AttackHandler
{
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
    
    public AttackHandler(Moveset moveset, Animator animator, PlayerController player)
    {
        this.moveset  = moveset;
        this.animator = animator;
        this.player   = player;
    }
    
    /// <summary>
    ///     Returns the move that corresponds to the direction that the player is pressing.
    ///     <para></para>
    ///     If the player is not airborne,
    ///     the move that will be performed is the move that corresponds to the direction that the player is pressing.
    /// </summary>
    /// <para>Used to execute the action that corresponds to the direction that the player is pressing.</para>
    /// <returns> The move that corresponds to the direction that the player is pressing. </returns>
    /// <example> If the player is pressing Up, then the move that will be performed is the "Up" move. </example>
    public void SelectPunch() //TODO: CHECK AI PREVIOUS CHATS FOR REFACTOR
    {
        List<MoveData> punchMoves = moveset.punchMoves;

        if (punchMoves.Exists(punch => punch.type != MoveData.Type.Punch)) Debug.LogWarning("One or more punch moves are not of type \"Punch\".");

        // Reference to the player's input.
        Vector2 input = player.InputManager.MoveInput;
        
        // If the player is airborne, the move that will be performed is the "Up" move.
        MoveData.Direction directionToPerform = player.IsAirborne() 
            ? MoveData.Direction.Up 
            : GetDirectionFromInput(input);

        // Get the move that corresponds to the direction that the player is pressing.
        MoveData selectedPunch = punchMoves.FirstOrDefault(punch => punch.direction == directionToPerform);

        PerformPunch(selectedPunch, directionToPerform);
    }

    /// <summary>
    ///     Performs the selected punch action based on the given direction.
    ///     Also handles the animation and applying move effects.
    /// </summary>
    /// <param name="selectedPunch">The MoveData instance representing the punch to perform.</param>
    /// <param name="directionToPerform">The direction in which to perform the punch.</param>
    void PerformPunch(MoveData selectedPunch, MoveData.Direction directionToPerform)
    {
        // Execute the action that corresponds to the direction that the player is pressing.
        if (selectedPunch == null) return;

        // Play the animation which handles all logic for the move, such as hitboxes, hurtboxes, etc.
        PlayAnimation(selectedPunch, directionToActionMap[directionToPerform].animationIndex);

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

    #region Kick Logic
    // Placeholders for Kick methods
    void SelectKick()
    {
        // TODO: Implement this method based on how kicks are intended to work in your game.
    }

    void PerformKick(MoveData selectedKick, MoveData.Direction directionToPerform)
    {
        // TODO: Implement this method based on how kicks are intended to work in your game.
    }
    #endregion

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
    void PlayAnimation(MoveData move, int index)
    {
        // Play animation
        animator.SetInteger("SelectedPunch", index);
        animator.SetTrigger("Punch");

        // Play sound
    }
}