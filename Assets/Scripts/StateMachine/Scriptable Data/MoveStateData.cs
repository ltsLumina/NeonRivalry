#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "MoveStateData", menuName = "State Data/MoveState Data", order = 1)]
public class MoveStateData : ScriptableObject
{
    [SerializeField] float moveSpeed;
    public float MoveSpeed => moveSpeed;
}