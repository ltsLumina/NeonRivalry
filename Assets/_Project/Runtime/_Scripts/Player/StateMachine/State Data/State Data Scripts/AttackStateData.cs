#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "AttackStateData", menuName = "State Data/AttackState Data", order = 4)]
public class AttackStateData : ScriptableObject
{
    [SerializeField] Moveset moveset;
    [SerializeField] PlayerAbilities playerAbilities;

    
    public Moveset Moveset => moveset;
    public PlayerAbilities PlayerAbilities => playerAbilities;
}