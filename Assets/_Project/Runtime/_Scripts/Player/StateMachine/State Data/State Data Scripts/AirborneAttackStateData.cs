#region
using UnityEngine;
#endregion


/// <summary>
/// <seealso cref="AirborneAttackState"/>
/// </summary>
[CreateAssetMenu(fileName = "AirborneAttackStateData", menuName = "State Data/AirborneAttackState Data", order = 5)]
public class AirborneAttackStateData : DefaultStateData
{
    [SerializeField] Moveset moveset;

    [SerializeField] float fallGravityMultiplier;
    [SerializeField] float jumpHaltForce;
    
    // -- Properties --

    public Moveset Moveset => moveset;
    
    public float FallGravityMultiplier => fallGravityMultiplier;
    public float JumpHaltForce => jumpHaltForce;
}
