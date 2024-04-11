using System.Collections.Generic;
using UnityEngine;

public class Moveset : ScriptableObject
{
    [Header("Moves"), Tooltip("Please limit the moves to a maximum of 4 per type.")]
    public List<MoveData> PunchMoves = new ();
    public List<MoveData> KickMoves = new ();
    public List<MoveData> SlashMoves = new ();
    public List<MoveData> UniqueMoves = new ();
    public List<MoveData> AirborneMoves = new (); // List of moves that can be performed while airborne. Will only ever be one.

    public List<List<MoveData>> Moves => new ()
    { PunchMoves, KickMoves, SlashMoves, UniqueMoves, AirborneMoves };
}