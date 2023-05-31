using UnityEngine;
using static Essentials.Attributes;

[CreateAssetMenu(fileName = "FallStateData", menuName = "State Data/FallState Data", order = 0)]
public class FallStateData : ScriptableObject
{
    [SerializeField, ReadOnly] float fallTimer;
    [SerializeField] float fallDuration;
    public float FallTimer => fallTimer;
    public float FallDuration => fallDuration;
}