using UnityEngine;

//TODO: Create a ability effect editor window that allows the user to create a new ability effect. Similar to the moveset editor window.
[CreateAssetMenu(fileName = "Teleport", menuName = "Move Effects/Teleport", order = 0)]
public class Teleport : MoveEffect
{
    float distance;

    //TODO: PlayerController class will be swapped with the attack system class which will include
    //      the "PerformMove()" method. It runs the animation, plays the sound, and applies the effect.
    public override void ApplyEffect(PlayerAbilities abilities, PlayerController target)
    {
        abilities.Teleport(-distance, null);
    }
}