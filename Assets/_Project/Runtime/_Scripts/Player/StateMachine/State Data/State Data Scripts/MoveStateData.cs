#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "MoveStateData", menuName = "State Data/MoveState Data", order = 1)]
public class MoveStateData : DefaultStateData
{
    [SerializeField, Tooltip("The speed at which the player moves. \nDefault Value: 10")]
    float moveSpeed;
    [SerializeField, Tooltip("The rate at which the player accelerates. \nDefault Value: 3")]
    float acceleration;
    [SerializeField, Tooltip("The rate at which the player decelerates. \nDefault Value: 5")]
    float deceleration;
    [SerializeField, Tooltip("The power of the velocity. \nDefault Value: 1.3")]
    float velocityPower;
    public float MoveSpeed => moveSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float AccelerationRate => velocityPower;
}