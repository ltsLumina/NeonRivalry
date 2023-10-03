#region
using Lumina.Essentials.Attributes;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "FallStateData", menuName = "State Data/FallState Data", order = 3)]
public class FallStateData : ScriptableObject
{
    [SerializeField, ReadOnly] float fallTimer;
    [SerializeField] float fallDuration;

    public float FallTimer => fallTimer;
    public float FallDuration => fallDuration;
}