using UnityEngine;

[CreateAssetMenu(fileName = "Fade", menuName = "Move Effects/Fade", order = 0)]
public class Fade : MoveEffect
{
    public float duration;

    public override void ApplyEffect(PlayerController target)
    {
        PlayerAbilities.Fade(target);
    }
}
