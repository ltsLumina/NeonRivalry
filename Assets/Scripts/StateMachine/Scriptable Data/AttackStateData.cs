using UnityEngine;
using Lumina.Essentials.Attributes;

[CreateAssetMenu(fileName = "AttackStateData", menuName = "State Data/AttackState Data", order = 0)]
public class AttackStateData : ScriptableObject
{
    [SerializeField, ReadOnly] float attackTimer;
    [SerializeField] float attackDuration;
    public float AttackTimer => attackTimer; //TODO: Timers created in a scriptable object are not plausible as the value will be 0 here, and the constructor will therefore always be 0.
    public float AttackDuration => attackDuration;
}