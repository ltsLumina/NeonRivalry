#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "AttackStateData", menuName = "State Data/AttackState Data", order = 4)]
public class AttackStateData : DefaultStateData
{
    [SerializeField] Moveset moveset;
    
    // -- Properties --
    
    public Moveset Moveset => moveset;
}