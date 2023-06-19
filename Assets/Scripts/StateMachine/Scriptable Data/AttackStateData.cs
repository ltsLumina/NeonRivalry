using UnityEngine;
using static Lumina.Essentials.Attributes;

[CreateAssetMenu(fileName = "AttackStateData", menuName = "State Data/AttackState Data", order = 0)]
public class AttackStateData : ScriptableObject
{
    [SerializeField, ReadOnly] float attackTimer;
    [SerializeField] float attackDuration;
    public float AttackTimer => attackTimer;
    public float AttackDuration => attackDuration;
}