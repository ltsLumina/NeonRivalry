using UnityEngine;

//TODO: Create a ability effect editor window that allows the user to create a new ability effect. Similar to the moveset editor window.
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
