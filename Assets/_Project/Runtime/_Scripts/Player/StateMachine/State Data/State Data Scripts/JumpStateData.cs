#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "JumpStateData", menuName = "State Data/JumpState Data", order = 2)]
public class JumpStateData : DefaultStateData
{
    [SerializeField] float jumpForce;
    [SerializeField] float jumpDuration;
    public float JumpForce => jumpForce;
    public float JumpDuration => jumpDuration;
}