#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "MoveState Data", menuName = "Scriptable Objects/MoveState Data")]
public class MoveStateData : ScriptableObject
{
    // Define properties or fields to hold the required data

    InputManager input;

    public float moveSpeed;
    // Add other properties as needed
    public PlayerController Player { get; }

    public Rigidbody2D PlayerRB { get; }
}