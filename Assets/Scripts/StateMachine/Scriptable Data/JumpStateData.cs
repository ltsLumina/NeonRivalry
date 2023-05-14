#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "JumpState Data", menuName = "Scriptable Objects/JumpState Data")]
public class JumpStateData : ScriptableObject
{
    // Define properties or fields to hold the required data
    PlayerController player;
    Rigidbody2D playerRB;
    InputManager input;

    public float jumpForce;
    public int jumpDuration;
    // Add other properties as needed

    public PlayerController Player
    {
        get => player;
        set => player = value;
    }

    public Rigidbody2D PlayerRB
    {
        get => playerRB;
        set => playerRB = value;
    }
}