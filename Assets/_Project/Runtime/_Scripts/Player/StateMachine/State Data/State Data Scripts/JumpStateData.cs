#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "JumpStateData", menuName = "State Data/JumpState Data", order = 2)]
public class JumpStateData : DefaultStateData
{
    [Tooltip("The force applied to the player when jumping.")]
    [SerializeField] float jumpHeight;

    [Tooltip("The mass of the player while falling.")]
    [SerializeField] float gravityScale;

    [SerializeField] float globalGravity;

    // -- Properties --
    
    public float JumpForce => jumpHeight;
    public float GravityScale => gravityScale;
    public float GlobalGravity => globalGravity;
}