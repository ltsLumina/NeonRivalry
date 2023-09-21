#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "FallStateData", menuName = "State Data/FallState Data", order = 3)]
public class FallStateData : DefaultStateData
{
    [SerializeField] float playerMass;
    [SerializeField] float fallGravityMultiplier;
    [SerializeField] float jumpHaltForce;

    public float PlayerMass => playerMass;
    public float FallGravityMultiplier => fallGravityMultiplier;
    public float JumpHaltForce => jumpHaltForce;
}