#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "JumpStateData", menuName = "State Data/JumpState Data", order = 2)]
public class JumpStateData : DefaultStateData
{
    [Tooltip("The force applied to the player when jumping.")]
    [SerializeField] float jumpForce;
    
    [Tooltip("The duration of the jump.")]
    [SerializeField] float jumpDuration;
    
    [Tooltip("The mass of the player while falling.")]
    [SerializeField] float playerMass;

    // -- Properties --
    
    public float JumpForce => jumpForce;
    public float JumpDuration => jumpDuration;
    public float PlayerMass => playerMass;
}