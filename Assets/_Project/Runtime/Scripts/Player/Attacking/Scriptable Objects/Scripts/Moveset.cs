using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Moveset", menuName = "Create Moveset", order = 0)]
public class Moveset : ScriptableObject
{
    [Header("Moves")]
    public List<MoveData> punchMoves = new ();
    public List<MoveData> kickMoves = new ();
    public List<MoveData> slashMoves = new ();
    public List<MoveData> uniqueMoves = new ();
}