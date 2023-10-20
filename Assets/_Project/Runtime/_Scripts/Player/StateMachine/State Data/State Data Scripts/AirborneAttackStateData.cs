#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "AirborneAttackStateData", menuName = "State Data/AirborneAttackState Data", order = 5)]
public class AirborneAttackStateData : DefaultStateData
{
    [SerializeField] Moveset moveset;

    // -- Properties --

    public Moveset Moveset => moveset;
}
