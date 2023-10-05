using System.Collections.Generic;
using UnityEngine;

public class Moveset : ScriptableObject
{
    [Header("Moves"), Tooltip("Please limit the moves to a maximum of 4 per type.")]
    public List<MoveData> punchMoves = new ();
    public List<MoveData> kickMoves = new ();
    public List<MoveData> slashMoves = new ();
    public List<MoveData> uniqueMoves = new ();
}