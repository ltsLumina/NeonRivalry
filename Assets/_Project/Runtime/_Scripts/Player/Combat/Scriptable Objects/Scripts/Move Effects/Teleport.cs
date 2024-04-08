using UnityEngine;

//TODO: Create a ability effect editor window that allows the user to create a new ability effect. Similar to the moveset editor window.
[CreateAssetMenu(fileName = "Teleport", menuName = "Move Effects/Teleport", order = 0)]
public class Teleport : MoveEffect
{
    [SerializeField] Vector2 distance;
    
    
    
    public override void ApplyEffect(PlayerController target)
    {
        PlayerAbilities.Teleport(distance, target);
    }
}