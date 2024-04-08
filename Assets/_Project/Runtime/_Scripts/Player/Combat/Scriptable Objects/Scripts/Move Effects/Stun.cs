using UnityEngine;

[CreateAssetMenu(fileName = "Stun", menuName = "Move Effects/Stun", order = 0)]
public class Stun : MoveEffect
{
    public float duration;
    
    PlayerAbilities abilities = new ();

    public override void ApplyEffect(PlayerController target)
    {
        PlayerAbilities.Stun(duration, target);
    }
}
