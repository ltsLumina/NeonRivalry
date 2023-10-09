#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "MoveStateData", menuName = "State Data/MoveState Data", order = 1)]
public class MoveStateData : DefaultStateData
{
    [Tooltip("The speed at which the player moves. \nDefault Value: 5")]
    [SerializeField] float moveSpeed;
    
    [Tooltip("The rate at which the player accelerates. \nDefault Value: 5")]
    [SerializeField] float acceleration;

    [Tooltip("The rate at which the player decelerates. \nDefault Value: 5")]
    [SerializeField] float deceleration;

    [Tooltip("The power of the velocity. \nDefault Value: 1.3")]
    [SerializeField] float velocityPower;
    public float MoveSpeed => moveSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float AccelerationRate => velocityPower;
}