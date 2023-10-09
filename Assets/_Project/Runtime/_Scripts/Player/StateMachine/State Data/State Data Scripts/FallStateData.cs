#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "FallStateData", menuName = "State Data/FallState Data", order = 3)]
public class FallStateData : DefaultStateData
{
    [Tooltip("The multiplier applied to gravity while falling. \nDefault Value: 7")]
    [SerializeField] float fallGravityMultiplier;
    
    [Tooltip("The force applied to the player while falling to halt upward momentum. \nDefault Value: 6")]
    [SerializeField] float jumpHaltForce;
    
    public float FallGravityMultiplier => fallGravityMultiplier;
    public float JumpHaltForce => jumpHaltForce;
}