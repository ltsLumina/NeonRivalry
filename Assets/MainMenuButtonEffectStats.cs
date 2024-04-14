using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Main Menu Button Effect Stat", menuName = "ScriptableObject Template")]
public class MainMenuButtonEffectStats : ScriptableObject
{
    [SerializeField] public float scaleIncrease = 1.1f;

    [SerializeField] public List<DownbeatEvents> downbeatEvents;

    [Serializable] public struct DownbeatEvents
    {
        public float delay;
    }
}
