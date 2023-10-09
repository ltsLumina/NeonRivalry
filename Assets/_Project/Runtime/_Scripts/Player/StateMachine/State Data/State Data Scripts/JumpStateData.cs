#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "JumpStateData", menuName = "State Data/JumpState Data", order = 2)]
public class JumpStateData : DefaultStateData
{
    [Tooltip("The force applied to the player when jumping. \nDefault Value: 650")]
    [SerializeField] float jumpForce;
    
    [Tooltip("The duration of the jump. \nDefault Value: 0.1")]
    [SerializeField] float jumpDuration;
    
    [Tooltip("The mass of the player while falling. \nDefault Value: 1.5")]
    [SerializeField] float playerMass;
    
    public float JumpForce => jumpForce;
    public float JumpDuration => jumpDuration;
    public float PlayerMass => playerMass;
}