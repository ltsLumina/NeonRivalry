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
    
    [Tooltip("The amount of time the player must be in the air before they can attack.")]
    [SerializeField] float requiredAirTime;
    
    // -- Properties --

    public Moveset Moveset => moveset;
    
    public float FallGravityMultiplier => fallGravityMultiplier;
    public float JumpHaltForce => jumpHaltForce;
    
    public float RequiredAirTime => requiredAirTime;
}
