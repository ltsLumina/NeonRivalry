#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "FallStateData", menuName = "State Data/FallState Data", order = 3)]
public class FallStateData : DefaultStateData
{
    [Tooltip("The multiplier applied to gravity while falling.")]
    [SerializeField] float fallGravityMultiplier;
    
    [Tooltip("The force applied to the player while falling to halt upward momentum.")]
    [SerializeField] float jumpHaltForce;

    // -- Properties --
    
    public float FallGravityMultiplier => fallGravityMultiplier;
    public float JumpHaltForce => jumpHaltForce;
}