#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "JumpStateData", menuName = "State Data/JumpState Data", order = 2)]
public class JumpStateData : DefaultStateData
{
    [SerializeField] float jumpForce;
    public float JumpForce => jumpForce;
}