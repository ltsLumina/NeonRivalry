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
    float dashEndGravity;

    float dashDir;
    float dashTimer;
    bool dashing;
    bool dashed;

    public override StateType Type => StateType.Dash;
    public override int Priority => statePriorities[Type];

    public DashState(PlayerController player, DashStateData stateData) : base(player)
    {
        dashDuration        = stateData.DashDuration;
        dashSpeed           = stateData.DashSpeed;
        dashSleepTime       = stateData.DashSleepTime;
        dashEndGravity      = stateData.DashEndGravity;
    }

    #region State Methods
    public override void OnEnter()
    {
        dashed = false;
        if (player.IsGrounded())
        {
            OnExit();
            return;
        }
        player.GetComponentInChildren<SpriteRenderer>().color = new (0.2f, 1f, 0.67f);

        Vector3 moveInput = player.InputManager.MoveInput;

        dashDir = (int) moveInput.x;

        player.GlobalGravity = 0f;
        //player.Rigidbody.useGravity = false;
        player.Rigidbody.velocity = Vector3.zero;

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
                Debug.LogWarning("Should be Dashing");
        }
    }

    public override void OnExit()
    {
        if (player.IsGrounded() && player.InputManager.MoveInput.x != 0) player.StateMachine.TransitionToState(StateType.Walk);
        else if (player.IsGrounded()) player.StateMachine.TransitionToState(StateType.Idle);

        if (dashed) player.StateMachine.TransitionToState(StateType.Fall);
    }
    #endregion

    IEnumerator HandleDashing()
    {
        Debug.LogWarning("Entered dash routine");
        yield return new WaitForSeconds(dashSleepTime);
        Debug.LogWarning("Dashsleep is over");
        dashTimer = dashDuration;
        Debug.LogWarning("Setting dash timer");
        yield return new WaitForSeconds(dashTimer);
        player.Rigidbody.velocity *= 0f;
        player.GlobalGravity = dashEndGravity;
        //player.Rigidbody.useGravity = true;
        dashed = true;

        OnExit();
        yield return new WaitForEndOfFrame();
    }
}
