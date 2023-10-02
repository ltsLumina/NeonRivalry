using UnityEngine;

[CreateAssetMenu(fileName = "Teleport", menuName = "Move Effects/Teleport", order = 0)]
public class Teleport : MoveEffect
{
    public float distance;

    //TODO: PlayerController class will be swapped with the attack system class which will include
    //      the "PerformMove()" method. It runs the animation, plays the sound, and applies the effect.
    public override void ApplyEffect(PlayerAbilities abilities)
    {
        abilities.Teleport(-distance, PlayerManager.PlayerOne); // assuming negative value will move character backward
    }
}

//TODO: Psuedo-code for the attack system class
// This class handles the execution of moves.
public class AttackSystem
{
    MoveData currentMove;
    Moveset moves;
    

    // This method will be called by an animation event in the player's animation controller.
    // When the player attacks (presses an attack input), they enter the attack state and then depending on if OnPunch, OnKick etc. was called, the corresponding move will be performed
    // by calling this method.

    // For instance, OnPunch will call PerformMove(currentMove.punchMoves[GetPunch()], abilities) where GetPunch() returns an index depending on if the player is pressing Up, Down, Forward or Neutral.
    // among a few other things such as if the player is airborne or not.
    public void OnPunch(MoveData move, PlayerAbilities abilities)
    {
        if (move == null) return;

        // Check which move was performed.
        // For instance, if the player pressed 6P, then the move that will be performed is the second move in the punchMoves list.
        //currentMove = moves.punchMoves[GetPunch()];
        
        // In this example, currentMove would be set to the second move in the punchMoves list.
        
        // Play animation
        // Play sound

        // Apply effect(s)
        foreach (var effect in currentMove.moveEffects)
        {
            // Apply the effect to the player.
            // The effect could be something like "Teleport" or "Stun" or "Heal".
            effect.ApplyEffect(abilities);
        }
    }
}

//TODO: Psuedo-code for a class that holds all the possible player abilities.
//      This class will be used by the PlayerController class.
public class PlayerAbilities : MonoBehaviour
{
    public void Teleport(float distance, PlayerController target)
    {
        Vector3 newPos = target.transform.position + new Vector3(distance, 0, 0);
        target.transform.position = newPos;
    }

    public void Stun(float duration)
    {
        // Apply a stun to this player, lasting for the specified duration.
    }

    public void Heal(int amount)
    {
        // Heal this player.
    }

    // any other ability methods here
}
