#region
using System.Collections;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.Rendering;
using Logger = Lumina.Debugging.Logger;
#endregion

public class DashState : State
{
    float dashDuration;
    float dashSpeed;
    float dashSleepTime;
    float dashEndGravity;
    float groundedDashMultiplier;

    float dashDir;
    float dashTimer;
    bool dashing;
    bool dashed;

    private Sound dashSFX;

    public override StateType Type => StateType.Dash;
    public override int Priority => statePriorities[Type];

    public DashState(PlayerController player, DashStateData stateData) : base(player)
    {
        dashDuration        = stateData.DashDuration;
        dashSpeed           = stateData.DashSpeed;
        dashSleepTime       = stateData.DashSleepTime;
        dashEndGravity      = stateData.DashEndGravity;
        groundedDashMultiplier = stateData.GroundedDashMultiplier;
    }

    #region State Methods
    public override void OnEnter()
    {
        if (player.IsAbleToDash == false)
        {
            OnExit();
            return;
        }
        
        dashed = false;
        player.IsAbleToDash = false;
        player.GetComponentInChildren<SpriteRenderer>().color = new (0.2f, 1f, 0.67f); 

        Vector3 moveInput = player.InputManager.MoveInput;
        
        dashDir = (int) moveInput.x;

        player.GlobalGravity = 0f;
        //player.Rigidbody.useGravity = false;
        player.Rigidbody.velocity = Vector3.zero;

        dashSFX = new Sound(SFX.Dash);
        
        if (player.IsGrounded())
        {
            if (!player.IsAbleToPhase) return;
            if (player.IsPhasing) return;
            
            dashSFX.SetVolume(0.45f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(.4f, .6f));
            dashSFX.Play();
            dashDuration *= groundedDashMultiplier;
            dashSleepTime *= groundedDashMultiplier;
            dashDuration = 1f;
            player.StartCoroutine(HandleGroundedDashing());
            return;
        }
        dashSFX.SetVolume(0.75f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(1f, 1.1f));
        dashSFX.Play();
        player.StartCoroutine(HandleDashing());
    }

    public override void UpdateState()
    {
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            dashing   =  true;
        }

        if (dashTimer < 0)
        {
            dashing = false;
            OnExit();
        }

        if (dashing)
        {
                player.Rigidbody.velocity = dashDir * dashSpeed * Vector3.right;
        }
    }

    public override void OnExit()
    {
        if (player.IsGrounded() && player.InputManager.MoveInput.x != 0) player.StateMachine.TransitionToState(StateType.Walk);
        else if (player.IsGrounded()) player.StateMachine.TransitionToState(StateType.Idle);

        if (dashed && !player.IsGrounded()) player.StateMachine.TransitionToState(StateType.Fall);
    }
    #endregion

    IEnumerator HandleDashing()
    {
        yield return new WaitForSeconds(dashSleepTime);
        dashTimer = dashDuration;
        player.ActivateTrail = true;
        yield return new WaitForSeconds(dashTimer);
        player.Rigidbody.velocity *= 0f;
        player.GlobalGravity = dashEndGravity;
        dashed = true;

        OnExit();
        yield return new WaitForEndOfFrame();
    }
    IEnumerator HandleGroundedDashing()
    {
        yield return new WaitForSeconds(dashSleepTime);
        dashTimer = dashDuration;
        player.ActivateTrail = true;
        player.IsPhasing = true;
        yield return new WaitForSeconds(dashTimer);
        player.Rigidbody.velocity *= 0f;
        player.GlobalGravity = dashEndGravity;
        dashed = true;
        player.IsPhasing = false;

        OnExit();
        yield return new WaitForEndOfFrame();
    }
}
