using System.Collections;
using UnityEngine;

public class DashState : State
{
    private float dashDuration;
    private float dashSpeed;
    private float dashSleepTime;
    private float dashInputBufferTime;

    private float dashDir;
    private float dashTimer;
    private bool dashing;

    public override StateType Type => StateType.Dash;
    public override int Priority => statePriorities[Type];

    public DashState(PlayerController player, DashStateData stateData) : base(player)
    {
        dashDuration = stateData.DashDuration;
        dashSpeed = stateData.DashSpeed;
        dashSleepTime = stateData.DashSleepTime;
        dashInputBufferTime = stateData.DashInputBufferTime;
    }
    public override void OnEnter()
    {
        Vector3 moveInput = player.InputManager.MoveInput;

        dashDir = (int)moveInput.x;

        player.Rigidbody.useGravity = false;

        player.StartCoroutine(HandleDashing());
    }

    public override void UpdateState()
    {
        if(dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            dashing = true;
        }

        if(dashTimer < 0)
        {
            dashing = false;
        }
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator HandleDashing()
    {
        yield return new WaitForSeconds(dashSleepTime);
        dashTimer = dashDuration;
        while(dashing)
        {
            player.Rigidbody.velocity = dashDir * dashSpeed * Vector3.right;
        }
        player.Rigidbody.useGravity = true;

        OnExit();
        yield return new WaitForEndOfFrame();
    }

}
