using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Moveset", menuName = "Create Moveset", order = 0)]
public class Moveset : ScriptableObject
{
    [Header("Moves")]
    public List<MoveData> moves = new ();

    public List<MoveData> punchMoves = new ();
    public List<MoveData> kickMoves = new ();
    public List<MoveData> slashMoves = new ();
    public List<MoveData> uniqueMoves = new ();
    
//     // Array of "Punch" moves
//     [SerializeField] MoveData[] punchMoves;
//     
//     // Array of "Kick" moves
//     [SerializeField] MoveData[] kickMoves;
//     
//     // Array of "Slash" moves
//     [SerializeField] MoveData[] slashMoves;
//     
//     // Array of "Unique" moves
//     [SerializeField] MoveData[] uniqueMoves;
}