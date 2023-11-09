using UnityEngine;

public class DashState : State
{
    private float gravityScaleOnEnter;

    private float dashDuration;
    private float dashSpeed;
    private float dashSleepTime;
    private float dashInputBufferTime;

    public override StateType Type => StateType.Dash;
    public override int Priority => statePriorities[Type];

    public DashState(PlayerController player, DashStateData stateData) : base(player)
    {
        dashDuration = stateData.DashDuration;
        dashSpeed = stateData.DashSpeed;
        dashSleepTime = stateData.DashSleepTime;
        dashInputBufferTime = stateData.DashInputBufferTime;
    }
    public override bool CanBeInterrupted()
    {
        throw new System.NotImplementedException();
    }
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

}
