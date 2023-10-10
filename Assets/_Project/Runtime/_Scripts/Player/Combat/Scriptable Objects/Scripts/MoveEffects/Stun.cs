﻿using UnityEngine;

[CreateAssetMenu(fileName = "Stun", menuName = "MoveEffects/Stun", order = 0)]
public class Stun : MoveEffect
{
    public float duration;

    public override void ApplyEffect(PlayerAbilities abilities, PlayerController target)
    {
        abilities.Stun(duration, target);
    }
}