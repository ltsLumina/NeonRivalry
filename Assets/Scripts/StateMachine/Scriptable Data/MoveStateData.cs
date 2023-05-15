using UnityEngine;

[CreateAssetMenu(fileName = "MoveStateData", menuName = "State Data/MoveState Data", order = 0)]
public class MoveStateData : ScriptableObject
{
    [SerializeField] float moveSpeed;
    public float MoveSpeed => moveSpeed;
}