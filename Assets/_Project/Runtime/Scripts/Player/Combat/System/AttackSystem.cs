using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackSystem : MonoBehaviour
{
    // -- Fields --
    
    [SerializeField] Moveset activeMoveset;
    [SerializeField, ReadOnly] MoveData currentMove;

    PlayerController player;
    Animator animator;
    
    // -- Properties --
    
    public Moveset ActiveMoveset
    {
        get => activeMoveset;
        set => activeMoveset = value;
    }
    public MoveData CurrentMove
    {
        get => currentMove;
        set => currentMove = value;
    }

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        animator = transform.parent.GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Called by the <see cref="PlayerInput"/> component when the player presses a punch button.
    /// </summary>
    /// <param name="context"> The context of the input action. </param>
    public void OnPunch(InputAction.CallbackContext context)
    {
        // Check which move was performed.
        // For instance, if the player pressed 6P, then the move that will be performed is the second move in the punchMoves list.
        CurrentMove = GetPunch(context);
    }

    /// <summary>
    ///     Returns the move that corresponds to the direction that the player is pressing.
    ///     <para></para>
    ///     If the player is not airborne,
    ///     the move that will be performed is the move that corresponds to the direction that the player is pressing.
    /// </summary>
    /// <param name="context"> The context of the input action.
    ///     <para>Used to execute the action that corresponds to the direction that the player is pressing.</para>
    /// </param>
    /// <returns> The move that corresponds to the direction that the player is pressing. </returns>
    /// <example> If the player is pressing Up, then the move that will be performed is the "Up" move. </example>
    MoveData GetPunch(InputAction.CallbackContext context)
    {
        List<MoveData> punchMoves = activeMoveset.punchMoves;

        if (punchMoves.Exists(punch => punch.type != MoveData.Type.Punch)) Debug.LogWarning("One or more punch moves are not of type \"Punch\".");

        if (!context.started) return null;

        // Define mapping from direction to action.
        // This mapping defines what action should be performed when a certain direction is pressed.
        Dictionary<MoveData.Direction, (Action<MoveData> action, string logMessage, int animationIndex)> directionToActionMap = new()
        { { MoveData.Direction.Neutral, (null, "Neutral move performed.", 0) },
          { MoveData.Direction.Forward, (null, "Forward move performed.", 1) },
          { MoveData.Direction.Down, (null, "Down move performed.", 3) },
          { MoveData.Direction.Up, (null, "Up move performed.", 2) } };

        // Reference to the player's input.
        Vector2 input = player.InputManager.MoveInput;
        
        // If the player is airborne, the move that will be performed is the "Up" move.
        MoveData.Direction directionToPerform = player.IsAirborne() 
            ? MoveData.Direction.Up 
            : GetDirectionFromInput(input);

        // Get the move that corresponds to the direction that the player is pressing.
        MoveData selectedPunch = punchMoves.FirstOrDefault(punch => punch.direction == directionToPerform);

        PerformPunch(selectedPunch, directionToActionMap, directionToPerform);

        return selectedPunch;
    }

    /// <summary>
    ///     Performs the selected punch action based on the given direction.
    ///     Also handles the animation and applying move effects.
    /// </summary>
    /// <param name="selectedPunch">The MoveData instance representing the punch to perform.</param>
    /// <param name="directionToActionMap">
    ///     A dictionary that maps a direction key to a tuple containing an action to perform, a
    ///     log message, and an animation index.
    /// </param>
    /// <param name="directionToPerform">The direction in which to perform the punch.</param>
    void PerformPunch(MoveData selectedPunch, IReadOnlyDictionary<MoveData.Direction, (Action<MoveData> action, string logMessage, int animationIndex)> directionToActionMap, MoveData.Direction directionToPerform)
    {
        // Execute the action that corresponds to the direction that the player is pressing.
        if (selectedPunch == null) return;

        // Play the animation which handles all logic for the move, such as hitboxes, hurtboxes, etc.
        PlayAnimation(selectedPunch, directionToActionMap[directionToPerform].animationIndex);

        // Apply the move effects, if any.
        if (selectedPunch.moveEffects != null)
        {
            // Iterate through the move effects and apply them to the player, if any.
            foreach (var effect in selectedPunch.moveEffects)
            {
                //effect.ApplyEffect(abilities);
                Debug.Log("Applied effect: " + effect.name);
            }
        }

        // Log the action that was performed.
        Debug.Log(directionToActionMap[directionToPerform].logMessage);
    }

    /// <summary>
    ///     Returns the direction that the player is pressing.
    ///     Does not include the "Up" direction as that is handled separately.
    ///     <see cref="GetPunch" /> for more information.
    /// </summary>
    /// <param name="input"> The player's input. </param>
    /// <returns> The direction that the player is pressing. </returns>
    static MoveData.Direction GetDirectionFromInput(Vector2 input)
    {
        // If the player is not inputting a direction, the move that will be performed is the "Neutral" move.
        if (input == Vector2.zero) return MoveData.Direction.Neutral;

        // If the player is inputting a direction, execute the action that corresponds to the direction that the player is pressing.
        return input.x != 0
            ? MoveData.Direction.Forward 
            : MoveData.Direction.Down;
    }

    // If the player is inputting down, the move that will be performed is the "Down" move.
    void PlayAnimation(MoveData move, int index)
    {
        // Play animation
        animator.SetInteger("SelectedPunch", index);
        animator.SetTrigger("Punch");

        // Play sound
    }
}