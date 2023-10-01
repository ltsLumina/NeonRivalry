using UnityEngine;

[CreateAssetMenu(fileName = "Moveset", menuName = "Create Moveset", order = 0)]
public class Moveset : ScriptableObject
{
    [Header("Moves")]
    
    // Array of "Punch" moves
    [SerializeField] MoveData[] punchMoves;
    
    // Array of "Kick" moves
    [SerializeField] MoveData[] kickMoves;
    
    // Array of "Slash" moves
    [SerializeField] MoveData[] slashMoves;
    
    // Array of "Unique" moves
    [SerializeField] MoveData[] uniqueMoves;
}