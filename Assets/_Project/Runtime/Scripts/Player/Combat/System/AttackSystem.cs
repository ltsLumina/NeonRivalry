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

    // This method will be called by an animation event in the player's animation controller.
    // When the player attacks (presses an attack input), they enter the attack state and then depending on if OnPunch, OnKick etc. was called, the corresponding move will be performed
    // by calling this method.
    // For instance, OnPunch will call PerformMove(currentMove.punchMoves[GetPunch()], abilities) where GetPunch() returns an index depending on if the player is pressing Up, Down, Forward or Neutral.
    // among a few other things such as if the player is airborne or not.
    public void OnPunch(InputAction.CallbackContext context)
    {
        //if (move == null) return;

        // Check which move was performed.
        // For instance, if the player pressed 6P, then the move that will be performed is the second move in the punchMoves list.
        CurrentMove = GetPunch(context);

        // Apply effect(s)
        // if (abilities == null) return;
        //
        // foreach (var effect in CurrentMove.moveEffects)
        // {
        //     // Apply the effect to the player.
        //     // The effect could be something like "Teleport" or "Stun" or "Heal".
        //     effect.ApplyEffect(abilities);
        // }
    }

    MoveData GetPunch(InputAction.CallbackContext context)
    {
        List<MoveData> punchMoves = activeMoveset.punchMoves;

        if (punchMoves.Exists(punch => punch.type != MoveData.Type.Punch)) Debug.LogWarning("One or more punch moves are not of type \"Punch\".");

        if (!context.started) return null;

        Vector2  input = player.InputManager.MoveInput;
        MoveData selectedPunch;
        string   logMessage;
        int      animationIndex;

        if (player.IsAirborne())
        {
            selectedPunch  = punchMoves.FirstOrDefault(punch => punch.direction == MoveData.Direction.Up);
            logMessage     = "Up move performed.";
            animationIndex = 2;
        }
        else
        {
            if (input == Vector2.zero)
            {
                selectedPunch  = punchMoves.FirstOrDefault(punch => punch.direction == MoveData.Direction.Neutral);
                logMessage     = "Neutral move performed.";
                animationIndex = 0;
            }
            else if (input.x != 0)
            {
                selectedPunch  = punchMoves.FirstOrDefault(punch => punch.direction == MoveData.Direction.Forward);
                logMessage     = "Forward move performed.";
                animationIndex = 1;
            }
            else
            {
                selectedPunch  = punchMoves.FirstOrDefault(punch => punch.direction == MoveData.Direction.Down);
                logMessage     = "Down move performed.";
                animationIndex = 3;
            }
        }

        if (selectedPunch != null)
        {
            PlayAnimation(selectedPunch, animationIndex);
            Debug.Log(logMessage);
        }

        return selectedPunch;
    }

    void PlayAnimation(MoveData move, int index)
    {
        // Play animation
        animator.SetInteger("SelectedPunch", index);
        animator.SetTrigger("Punch");

        // Play sound
    }
}