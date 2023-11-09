#region
using System.Collections;
using UnityEngine;
using Logger = Lumina.Debugging.Logger;
#endregion

public class DashState : State
{
    float dashDuration;
    float dashSpeed;
    float dashSleepTime;
    float dashInputBufferTime;

    float dashDir;
    float dashTimer;
    bool dashing;

    public override StateType Type => StateType.Dash;
    public override int Priority => statePriorities[Type];

    public DashState(PlayerController player, DashStateData stateData) : base(player)
    {
        dashDuration        = stateData.DashDuration;
        dashSpeed           = stateData.DashSpeed;
        dashSleepTime       = stateData.DashSleepTime;
        dashInputBufferTime = stateData.DashInputBufferTime;
    }

    #region State Methods
    public override void OnEnter()
    {
        player.GetComponentInChildren<SpriteRenderer>().color = new (0.2f, 1f, 0.67f);

        Vector3 moveInput = player.InputManager.MoveInput;

        dashDir = (int) moveInput.x;

        player.Rigidbody.useGravity = false;

        player.StartCoroutine(HandleDashing());
    }

    public override void UpdateState()
    {
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            dashing   =  true;
        }

        if (dashTimer < 0) dashing = false;
    }

    public override void OnExit()
    {
        Logger.Trace("Exiting Dash State", Type);
    }
        #endregion

    IEnumerator HandleDashing()
    {
        yield return new WaitForSeconds(dashSleepTime);
        dashTimer = dashDuration;
        while (dashing) player.Rigidbody.velocity = dashDir * dashSpeed * Vector3.right;
        player.Rigidbody.useGravity = true;

        OnExit();
        yield return new WaitForEndOfFrame();
    }
}
