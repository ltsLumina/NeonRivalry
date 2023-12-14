#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "DashStateData", menuName = "State Data/DashState Data", order = 5)]

public class DashStateData : DefaultStateData
{
    [Tooltip("Determines how long the player will be dashing.")]
    [SerializeField] private float dashDuration;

    [Tooltip("Determines how fast the player will be dashing.")]
    [SerializeField] private float dashSpeed;

    [Tooltip("The amount of time the game will freeze before initiating the dash.")]
    [SerializeField] private float dashSleepTime;

    [Tooltip("The amount of dashes the player can use before having to touch the ground to reset them")]
    [SerializeField] private int dashAmount;

    public float DashDuration => dashDuration;
    public float DashSpeed => dashSpeed;
    public float DashSleepTime => dashSleepTime;

    public int DashAmount => dashAmount;
}
