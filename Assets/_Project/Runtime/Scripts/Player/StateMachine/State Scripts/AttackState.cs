#region
using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Debugging;
using UnityEngine;
#endregion

/*----------------------------------------------------------------------------
 * IMPORTANT NOTE: Use Time.fixedDeltaTime instead of Time.deltaTime
 *----------------------------------------------------------------------------*/

public class AttackState : State
{
    // -- Abstract Variables --
    public override StateType Type => StateType.Attack;
    public override int Priority => statePriorities[Type];
    
    public bool IsAttacking { get; private set; }
    public bool IsAirborne { get; private set; }

    // -- State Specific Variables --
    float groundedAttackTimer;
    float airborneAttackTimer;

    //A dictionary that maps a direction key to a tuple containing an action to perform, a log message, and an animation index.
    readonly static Dictionary<MoveData.Direction, (Action<MoveData> action, string logMessage, int animationIndex)> directionToActionMap = new ()
    { { MoveData.Direction.Neutral, (null, "Neutral move performed.", 0) },
      { MoveData.Direction.Forward, (null, "Forward move performed.", 1) },
      { MoveData.Direction.Up, (null, "Up move performed.", 2) },
      { MoveData.Direction.Down, (null, "Down move performed.", 3) } };
    
    Animator animator;
    Moveset moveset;
    
    // -- Constructor --
    public AttackState(PlayerController player, AttackStateData stateData) : base(player)
    {
        animator = player.GetComponentInChildren<Animator>();
        moveset  = stateData.Moveset;

        Debug.Assert(moveset != null, "Moveset is null in the AttackStateData. Please assign it in the inspector.");
    }

    public override bool CanBeInterrupted() => interruptibilityRules[Type];

    public override void OnEnter()
    {
        // Play the attack animation.
        IsAttacking = true;
        IsAirborne  = !player.IsGrounded(); // Check whether the player was in air when attack started.

        player.GetComponentInChildren<SpriteRenderer>().color = IsAirborne ? new (1f, 0.21f, 0.38f, 0.75f) : Color.red;

        groundedAttackTimer = 0;
        airborneAttackTimer = 0;

        SelectPunch();
    }

    public override void UpdateState()
    {
        // Player is grounded, perform grounded attack.
        if (!IsAirborne)
        {
            // If the attack duration has not been reached, continue attacking.
            if (groundedAttackTimer < animator.GetCurrentAnimatorStateInfo(0).length)
            {
                groundedAttackTimer += Time.fixedDeltaTime;

                if (player.IsGrounded()) FGDebugger.Debug("Attacking on the ground!", LogType.Log, StateType.Attack);

                // Play ground attack animation and logic.
                else IsAirborne = true;
            }
            else { OnExit(); }
        }
        else // Player is airborne, perform airborne attack.
        {
            if (airborneAttackTimer < animator.GetCurrentAnimatorStateInfo(0).length)
            {
                airborneAttackTimer += Time.fixedDeltaTime;

                // If the player lands, cancel the attack.
                if (player.IsGrounded())
                {
                    FGDebugger.Debug("Airborne Attack cancelled!", LogType.Log, StateType.Attack);
                    OnExit();
                }
                else
                {
                    FGDebugger.Debug("Attacking in the air!", LogType.Log, StateType.Attack);

                    // Play airborne attack animation and run logic.
                }
            }
            else { OnExit(); }
        }
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
    void SelectPunch()
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
        if (selectedPunch.moveEffects != null)
        {
            // Iterate through the move effects and apply them to the player, if any.
            foreach (var effect in selectedPunch.moveEffects)
            {
                //effect.ApplyEffect(abilities);
                FGDebugger.Debug("Applied effect: " + effect.name, LogType.Log, StateType.Attack);
            }
        }

        // Log the action that was performed.
        FGDebugger.Debug(directionToActionMap[directionToPerform].logMessage, LogType.Log, StateType.Attack);
    }

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

    public override void OnExit() 
    {
        //TransitionTo(player.InputManager.MoveInput.x != 0 ? StateType.Walk : StateType.Idle);
        player.StateMachine.TransitionToState(player.InputManager.MoveInput.x != 0 ? StateType.Walk : StateType.Idle);
        IsAttacking = false;
    }
}