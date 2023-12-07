#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "MoveStateData", menuName = "State Data/MoveState Data", order = 1)]
public class MoveStateData : DefaultStateData
{
    [Tooltip("The speed at which the player moves.")]
    [SerializeField] float moveSpeed;
    
    [Tooltip("The rate at which the player accelerates.")]
    [SerializeField] float acceleration;

    [Tooltip("The rate at which the player decelerates.")]
    [SerializeField] float deceleration;

    [Tooltip("The power of the velocity.")]
    [SerializeField] float velocityPower;
    
    // -- Properties --
    
    public float MoveSpeed => moveSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float AccelerationRate => velocityPower;
}