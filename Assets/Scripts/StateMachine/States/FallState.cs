﻿using UnityEngine;

public class FallState : State
{
    public override StateType Type => StateType.Fall;
    public override int Priority => statePriorities[Type];

    public bool IsFalling { get; private set; }

    // -- State Specific Variables --
    //TODO: these are temporary
    float fallTimer;
    float fallDuration;

    // -- Constructor --
    public FallState(PlayerController player, FallStateData stateData) : base(player)
    {
        fallTimer = stateData.FallTimer;
        fallDuration = stateData.FallDuration;
    }

    public override bool CanBeInterrupted()
    {
        // return true if the player is attacking or is grounded
        return player.IsAttacking() || player.IsGrounded() || player.IsJumping();
    }

    public override void OnEnter()
    {
        // Play the attack animation.
        //Debug.Log("Entered Attack State");
        IsFalling = true;

        player.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0.58f, 0f);
    }

    public override void UpdateState()
    {
        //TODO: this is not final. This is just a placeholder.
        if (player.IsGrounded())
        {
            OnExit();
        }
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
        // Debug.Log("Exited Fall State");

        // Play land animation.
        IsFalling = false;
    }
}